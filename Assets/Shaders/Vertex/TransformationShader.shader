// Applies all transformations to loaded mesh
// THE ORDER IS THE FOLLOWING:
// 1) TWIST 	(default axis: z)
// 2) STRETCH 	(default axis: z)
// 3) BEND    	(default axis: y)
// Mainly based on "Global and local deformations of solid primitives" paper by Alan H. Barr

// By applying all transformations in a single shader file we can check all vertices once per pass
Shader "Custom/TransformationShader"
{
	Properties
    {
		// Mesh max values for bounding box. Equal to two times max extents.
		[HideInInspector]_MaxExtents("Max extents", Vector) = (0,0,0,0)

		// === TWIST ===
		[Header(Twist)]
		// _TwistAngle: Angle of rotation at the extremes (in degrees)
		_TwistAngleX("Twist Angle X", Range(-360,360)) = 0
		_TwistAngleY("Twist Angle Y", Range(-360,360)) = 0
		_TwistAngleZ("Twist Angle Z", Range(-360,360)) = 0

		[Space(10)]

		// === STRETCH ===
		// Axis for stretching
		[Header(Stretch)]
		// _StretchAmount: How much to stretch along main axis
		// _StretchStrength: How much to exagerate stretch
		_StretchAmountX("Stretch Amount X", Range(-2,2)) = 0
		_StretchStrengthX("Stretch Strength X", Range(0,3)) = 1
		[Space(5)]
		_StretchAmountY("Stretch Amount Y", Range(-2,2)) = 0
		_StretchStrengthY("Stretch Strength Y", Range(0,3)) = 1
		[Space(5)]
		_StretchAmountZ("Stretch Amount Z", Range(-2,2)) = 0
		_StretchStrengthZ("Stretch Strength Z", Range(0,3)) = 1

		// TODO: anche gli altri assi!!
		[Space(10)]
		// === BEND ===
		// Axis for bending
		[Enum(X,0,Y,1,Z,2)]_BendAxis("Bend around", Int) = 2

		// Params for bending region
		_YMin("Min value", Range(0,1)) = 0
		_YMax("Max value", Range(0,1)) = 0
		_Y0("Starting value", Range(0,1)) = 0

		// ...
		_BendRate("Bend Rate (k)", Float) = 0
    }

    SubShader
    {
        Pass
        {
			// Main HLSL code
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			// Utility functions
            #include "UnityCG.cginc"
			#include "TransformationShader.cginc"

			// === UNIFORM VARIABLES ===
			// Just copied from Shaderlab wrapper!

			// COMMON
			float4 _MaxExtents;

			// TWIST
			float _TwistAngleX;
			float _TwistAngleY;
			float _TwistAngleZ;

			// STRETCH
			float _StretchStrengthX;
			float _StretchStrengthY;
			float _StretchStrengthZ;
			float _StretchAmountX;
			float _StretchAmountY;
			float _StretchAmountZ;

			// BEND
			int _BendAxis;
			float _YMin;
			float _YMax;
			float _Y0;
			float _BendRate;

            // Vertex Shader
            v2f vert (appdata_base v)
            {
                v2f o;

				// First of all copy vertices to output
				o.vertex = v.vertex;
				o.normal = v.normal;

				// The order is as follows
				// TWIST - STRETCH - BEND, for each along X,Y,Z respectively
				o = DoTwist(o, X_AXIS, _TwistAngleX, _MaxExtents);
				o = DoTwist(o, Y_AXIS, _TwistAngleY, _MaxExtents);
				o = DoTwist(o, Z_AXIS, _TwistAngleZ, _MaxExtents);

				o = DoStretch(o, X_AXIS, _StretchAmountX, _StretchStrengthX, _MaxExtents );
				o = DoStretch(o, Y_AXIS, _StretchAmountY, _StretchStrengthY, _MaxExtents );
				o = DoStretch(o, Z_AXIS, _StretchAmountZ, _StretchStrengthZ, _MaxExtents );

				o = DoBend(o, _BendAxis, _YMin, _YMax, _Y0, _BendRate, _MaxExtents );

				// Finally, do MVP transformation as usual and return
				o.vertex = UnityObjectToClipPos(o.vertex);
				o.normal = UnityObjectToWorldNormal(o.normal);
                return o;
            }

			// Fragment Shader
            fixed4 frag (v2f i) : SV_Target
            {
				// TODO check what to do here according to client's need
                fixed4 c;
				i.normal = normalize(i.normal);
                c.xyz = i.normal * 0.5 + 0.5;
                c.w = 1;

                return c;
            }
            ENDCG
        }
    }
}
