#ifndef TRANSF_SHADER
#define TRANSF_SHADER

// =============== CONSTANTS ===========================
#define X_AXIS 0
#define Y_AXIS 1
#define Z_AXIS 2
#define FLOAT_EPS 1e-8

// =============== DATA STRUCTURES ====================
struct v2f {
    float4 vertex : SV_POSITION;
    float3 normal : TEXCOORD0;
};

// ============== FUNCTIONS ==========================
// Align x or y axis to z axis to enable transformation on all axes
v2f DoZAxisRotation(v2f v, int toAxis, inout float4 maxExtents) {
    // Rotation matrix from Z to Y Axis
    float4x4 ZtoYAxis = {
       1,0,0,0,
       0,0,-1,0,
       0,1,0,0,
       0,0,0,1
    };

    // Rotation matrix from Z to X Axis
    float4x4 ZtoXAxis = {
        0,0,1,0,
        0,1,0,0,
        -1,0,0,0,
        0,0,0,1
    };

    // First of all, check if a pre-rotation is required in order to align
    // one axis with the z axis
    if( toAxis == X_AXIS ) {
        v.vertex = mul(ZtoXAxis, v.vertex);
        maxExtents = mul(ZtoXAxis, maxExtents);
    }
    else if( toAxis == Y_AXIS ) {
        v.vertex = mul(ZtoYAxis, v.vertex);
        maxExtents = mul(ZtoYAxis, maxExtents);
    }

    return v;
}

v2f RestoreZAxis(v2f v, int fromAxis, inout float4 maxExtents) {
    // Rotation matrix from Z to Y Axis
    float4x4 YtoZAxis = {
      1,     0,     0,     0,
      0,     0,     1,     0,
      0,    -1,     0,     0,
      0,     0,     0,     1
    };

    // Rotation matrix from Z to X Axis
    float4x4 XtoZAxis = {
         0,     0,    -1,     0,
         0,     1,     0,     0,
         1,     0,     0,     0,
         0,     0,     0,     1
    };

    // Rollback axis if pre-rotated
    if( fromAxis == X_AXIS ) {
        v.vertex = mul(XtoZAxis, v.vertex);
        maxExtents = mul(XtoZAxis, maxExtents);
    }
    else if( fromAxis == Y_AXIS ) {
        v.vertex = mul(YtoZAxis, v.vertex);
        maxExtents = mul(YtoZAxis, maxExtents);
    }

    return v;
}

// Align x or z axis to y axis to enable transformation on all axes (for bending!)
v2f DoYAxisRotation(v2f v, int toAxis, inout float4 maxExtents) {
    // Rotation matrix from Y to X Axis
    float4x4 YtoXAxis = {
       0, -1, 0, 0,
       1, 0, 0, 0,
       0, 0, 1, 0,
       0, 0, 0, 1
    };

    // Rotation matrix from Y to Z Axis
    float4x4 YtoZAxis = {
        1, 0, 0, 0,
        0, 0, 1, 0,
        0, -1, 0, 0,
        0, 0, 0, 1
    };

    // First of all, check if a pre-rotation is required in order to align
    // one axis with the y axis
    if( toAxis == X_AXIS ) {
        v.vertex = mul(YtoXAxis, v.vertex);
        maxExtents = mul(YtoXAxis, maxExtents);
    }
    else if( toAxis == Z_AXIS ) {
        v.vertex = mul(YtoZAxis, v.vertex);
        maxExtents = mul(YtoZAxis, maxExtents);
    }

    return v;
}

v2f RestoreYAxis(v2f v, int fromAxis, inout float4 maxExtents) {
    // Rotation x axis back
    float4x4 XtoYAxis = {
      0, 1, 0, 0,
      -1,  0, 0, 0,
      0,  0, 1, 0,
      0,  0, 0, 1
    };

    // Rotation z axis back
    float4x4 ZtoYAxis = {
        1, 0, 0, 0,
        0, 0, -1, 0,
        0, 1, 0, 0,
        0, 0, 0, 1
    };

    // Rollback axis if pre-rotated
    if( fromAxis == X_AXIS ) {
        v.vertex = mul(XtoYAxis, v.vertex);
        maxExtents = mul(XtoYAxis, maxExtents);
    }
    else if( fromAxis == Z_AXIS ) {
        v.vertex = mul(ZtoYAxis, v.vertex);
        maxExtents = mul(ZtoYAxis, maxExtents);
    }

    return v;
}

// 1) (z-)TWIST
// Do rotation along z axis, where theta depends on a function of z and is not fixed.
// In this case, the extreme angles are fixed, thus
// f(z) = lerp(0,z,z*,0,theta*)
// where z* is max z (_MaxExtent) and theta* is angle set by user
inline v2f DoTwist( v2f v, int _TwistAxis, float _TwistAngle, float4 _MaxExtents )
{
    v2f o;

    v = DoZAxisRotation(v, _TwistAxis, _MaxExtents);

    // Setup
    float x = v.vertex.x;    float nx = v.normal.x;
    float y = v.vertex.y;    float ny = v.normal.y;
    float z = v.vertex.z;    float nz = v.normal.z;
    float w = v.vertex.w;

    // theta = f(z)
    float theta = (z / _MaxExtents.z) * radians(_TwistAngle); // NB! Angle is in degrees!
    // dtheta = f'(z)
    // NB! If you change f(z) remember also to change its derivative
    float dtheta = (1.0f / _MaxExtents.z) * radians(_TwistAngle);

    float c = cos(theta);
    float s = sin(theta);

    // Vertex transformation
    o.vertex.x = x*c - y*s;
    o.vertex.y = x*s + y*c;
    o.vertex.z = z;
    o.vertex.w = w;

    // Normal transformation
    o.normal.x = c * nx - s * ny;
    o.normal.y = s * nx + c * ny;
    o.normal.z = y * dtheta * nx - x * dtheta * ny + nz;
    o.normal = normalize(o.normal); // NB: don't forget to normalize in the end!

    // Restore back to original axis and return
    o = RestoreZAxis(o, _TwistAxis, _MaxExtents);
    return o;
}

// 2) (z-)STRETCH & SQUASH
// Stretch or squash depending on amount.
// Amount > 0 ==> STRETCH: scale z-axis linearly and inversely scale x/y axes quadratically
// Amount < 0 ==> SQUASH: the opposite
inline v2f DoStretch( v2f v, int _StretchAxis, float _StretchAmount, float _StretchStrength, float4 _MaxExtents ) {
    v2f o;

    v = DoZAxisRotation(v, _StretchAxis, _MaxExtents);

    // Setup
    float x = v.vertex.x;    float nx = v.normal.x;
    float y = v.vertex.y;    float ny = v.normal.y;
    float z = v.vertex.z;    float nz = v.normal.z;
    float w = v.vertex.w;

    // NB Vertex and normal transformation depend on the sign of the amount!

    // _StretchAmount > 0 ==> stretch
    if( _StretchAmount > 0 )
    {
        // Vertex transformation
        //x && y reduce ..
        o.vertex.x = x / ( 1.0 + _StretchAmount * _StretchStrength) ;
        o.vertex.y = y / ( 1.0 + _StretchAmount * _StretchStrength) ;

        // while z increase
        o.vertex.z = z * (1 + _StretchAmount);

        // Normal transformation
        // J =
        //[ 1/(a*s + 1),           0,     0]
        //[           0, 1/(a*s + 1),     0]
        //[           0,           0, a + 1]

        // thus the normal matrix is given by N = |J| * inv(transpose(J)) =
        //[ (a + 1)/(a*s + 1),                 0,             0]
        //[                 0, (a + 1)/(a*s + 1),             0]
        //[                 0,                 0, 1/(a*s + 1)^2]

        o.normal.x = nx * (_StretchAmount + 1.0f) / ( 1.0 + _StretchAmount * _StretchStrength);
        o.normal.y = ny * (_StretchAmount + 1.0f) / ( 1.0 + _StretchAmount * _StretchStrength);
        o.normal.z = nz / ( ( 1.0 + _StretchAmount * _StretchStrength) * ( 1.0 + _StretchAmount * _StretchStrength) );

    } else {   // _StretchAmount < 0 ==> squash
        // x & y scale out
        o.vertex.x = x * (1.0 - _StretchAmount * _StretchStrength); // NB _StretchAmount < 0 thus scaling factor > 1!
        o.vertex.y = y * (1.0 - _StretchAmount * _StretchStrength);

        // while z reduce
        o.vertex.z = -z / (_StretchAmount - 1.0);

        // Normal transformation
        // J =
        //[ 1 - a*s,       0,          0]
        //[       0, 1 - a*s,          0]
        //[       0,       0, -1/(a - 1)]

        // thus the normal matrix is given by N = |J| * inv(transpose(J)) =
        //[ (a*s - 1)/(a - 1),                 0,           0]
        //[                 0, (a*s - 1)/(a - 1),           0]
        //[                 0,                 0, (a*s - 1)^2]

        o.normal.x = (nx * (_StretchAmount * _StretchStrength - 1.0f)) / (_StretchAmount - 1.0f);
        o.normal.y = (ny * (_StretchAmount * _StretchStrength - 1.0f)) / (_StretchAmount - 1.0f);
        o.normal.z = nz * (_StretchAmount * _StretchStrength - 1.0f) * (_StretchAmount * _StretchStrength - 1.0f);
    }

    o.normal = normalize(o.normal);

    o.vertex.w = w;

    // Restore axis and return
    o = RestoreZAxis(o, _StretchAxis, _MaxExtents);
    return o;
}

// Retrieve local coordinates from percentage of mesh measure along one axis
inline float percentageYToLocalCoords( float p, float e ) {
    return 2 * e * p - e;
}

// 3) (y-)BEND
// Bend linearly at a rate k [rad/m] from point y0
// Note that we are following Barr convention, thus this transformation is around y-axis and not z-axis by default!
// All y passed as arguments are to be considered as percentages aka in [0,1] of the mesh measures
inline v2f DoBend( v2f v, int _BendAxis, float _YMin, float _YMax, float _Y0, float k, float4 _MaxExtents ) {
    // If no bending is required actually (k = 0), just return v
    if( k < FLOAT_EPS && k > -FLOAT_EPS )
        return v;

    v2f o;

    v = DoYAxisRotation(v, _BendAxis, _MaxExtents);

    // Setup
    float x = v.vertex.x;    float nx = v.normal.x;
    float y = v.vertex.y;    float ny = v.normal.y;
    float z = v.vertex.z;    float nz = v.normal.z;
    float w = v.vertex.w;

    // get y from percentages
    float ymin = percentageYToLocalCoords(_YMin, _MaxExtents.y);
    float ymax = percentageYToLocalCoords(_YMax, _MaxExtents.y);
    float y0 = percentageYToLocalCoords(_Y0, _MaxExtents.y);

    float yhat = clamp(y, ymin, ymax);
    float theta = k * (yhat - y0);
    float c = cos(theta), s = sin(theta);
    float ik = 1.0/k;

    o.vertex.x = x;

    o.vertex.y = -s * (z - ik) + y0;
    if( y < ymin ) o.vertex.y += c * (y-ymin);
    else if( y > ymax ) o.vertex.y += c * (y-ymax);

    o.vertex.z = c * (z-ik) + ik;
    if( y < ymin ) o.vertex.z += s * (y-ymin);
    else if( y > ymax ) o.vertex.z += s * (y-ymax);

    o.vertex.w = w;

    o.normal = v.normal;

    o = RestoreYAxis(o, _BendAxis, _MaxExtents);
    return o;
}

#endif
