#ifndef TRANSF_SHADER
#define TRANSF_SHADER

// =============== CONSTANTS ===========================
#define X_AXIS 0
#define Y_AXIS 1
#define Z_AXIS 2

// =============== DATA STRUCTURES ====================
struct v2f {
    float4 vertex : SV_POSITION;
    float3 normal : TEXCOORD0;
};

// ============== FUNCTIONS ==========================
// Align x or y axis to z axis to enable transformation on all axes
v2f DoZAxisRotation(v2f v, int toAxis, float4 maxExtents) {
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

v2f RestoreZAxis(v2f v, int fromAxis, float4 maxExtents) {
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
    float x = v.vertex.x;
    float y = v.vertex.y;
    float z = v.vertex.z;
    float w = v.vertex.w;

    float theta = (z / _MaxExtents.z) * radians(_TwistAngle); // NB! Angle is in degrees!
    float c = cos(theta);
    float s = sin(theta);

    // Do transformation!
    o.vertex.x = x*c - y*s;
    o.vertex.y = x*s + y*c;
    o.vertex.z = z;
    o.vertex.w = w;

    o = RestoreZAxis(o, _TwistAxis, _MaxExtents);

    // TODO: normals if needed.
    o.normal = v.normal;
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
    float x = v.vertex.x;
    float y = v.vertex.y;
    float z = v.vertex.z;
    float w = v.vertex.w;

    // _StretchAmount > 0 ==> stretch
    if( _StretchAmount > 0 )
    {
        //x && y reduce ..
        o.vertex.x = x / ( 1.0 + _StretchAmount * _StretchStrength) ;
        o.vertex.y = y / ( 1.0 + _StretchAmount * _StretchStrength) ;

        // while z increase
        o.vertex.z = z * (1 + _StretchAmount);
    } else {   // _StretchAmount < 0 ==> squash
        // x & y scale out
        o.vertex.x = x * (1.0 - _StretchAmount * _StretchStrength); // NB _StretchAmount < 0 thus scaling factor > 1!
        o.vertex.y = y * (1.0 - _StretchAmount * _StretchStrength);

        // while z reduce
        o.vertex.z = -z / (_StretchAmount - 1.0);
    }

    o.vertex.w = w;

    o = RestoreZAxis(o, _StretchAxis, _MaxExtents);

    // TODO: normals if needed.
    o.normal = v.normal;
    return o;
}

#endif
