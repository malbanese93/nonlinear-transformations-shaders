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

struct TwistData {
    int _TwistAxis;
    float _TwistAngle;
};

// ============== FUNCTIONS ==========================
// 1) (z-)TWIST
// Do rotation along z axis, where theta depends on a function of z and is not fixed.
// In this case, the extreme angles are fixed, thus
// f(z) = lerp(0,z,z*,0,theta*)
// where z* is max z (_MaxExtent) and theta* is angle set by user
inline v2f DoTwist( v2f v, int _TwistAxis, float _TwistAngle, float4 _MaxExtents )
{
    v2f o;

    // Rotation matrix from Z to Y Axis
    float4x4 ZtoYAxis = {
       1,0,0,0,
       0,0,-1,0,
       0,1,0,0,
       0,0,0,1
    };
    // Rotation matrix from Y to Z Axis
    float4x4 YtoZAxis = {
        1,0,0,0,
        0,0,1,0,
        0,-1,0,0,
        0,0,0,1
    };

    // Rotation matrix from Z to X Axis
    float4x4 ZtoXAxis = {
        0,0,1,0,
        0,1,0,0,
        -1,0,0,0,
        0,0,0,1
    };
    // Rotation matrix from X to Z Axis
    float4x4 XtoZAxis = {
        0,0,-1,0,
        0,1,0,0,
        1,0,0,0,
        0,0,0,1
    };

    // First of all, check if a pre-rotation is required in order to align
    // one axis with the z axis
    if( _TwistAxis == X_AXIS )
        v.vertex = mul(ZtoXAxis, v.vertex);
    else if( _TwistAxis == Y_AXIS )
        v.vertex = mul(ZtoYAxis, v.vertex);

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

    // Rollback axis if pre-rotated
    if( _TwistAxis == X_AXIS )
        o.vertex = mul(XtoZAxis, o.vertex);
    else if( _TwistAxis == Y_AXIS )
        o.vertex = mul(YtoZAxis, o.vertex);

    // Normals
    // IGNORED FOR NOW
    o.normal = v.normal;

    /*float nx = v.normal.x;
    float ny = v.normal.y;
    float nz = v.normal.z;

    o.normal.x = c*nx - s*ny;
    o.normal.y = s*nx + c*ny;
    o.normal.z = y*dtheta*nx - x*dtheta*ny + nz;*/

    //o.normal = UnityObjectToWorldNormal(normal);
    return o;
}

#endif
