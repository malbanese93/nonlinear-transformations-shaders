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
		// === TWIST ===
		[Header(Twist)]
		// _TwistAngle: Angle of rotation at the extremes (in degrees)
		_TwistAngleX("Twist Angle X", Range(-360,360)) = 0
		_TwistAngleY("Twist Angle Y", Range(-360,360)) = 0
		_TwistAngleZ("Twist Angle Z", Range(-360,360)) = 0

		[Space(10)]

		// === STRETCH ===
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

		[Space(10)]
		// === BEND ===
		[Header(Bend X)]
		// Params for bending region, expressed in percentage
		_XMin("Min value X", Range(0,1)) = 0
		_XMax("Max value X", Range(0,1)) = 1
		_X0("Starting value X", Range(0,1)) = 0
		_BendRateX("Bend Rate X", Float) = 0
		[Space(5)]
		[Header(Bend Y)]
		_YMin("Min value Y", Range(0,1)) = 0
		_YMax("Max value Y", Range(0,1)) = 1
		_Y0("Starting value Y", Range(0,1)) = 0
		_BendRateY("Bend Rate Y", Float) = 0
		[Space(5)]
		[Header(Bend Z)]
		_ZMin("Min value Z", Range(0,1)) = 0
		_ZMax("Max value Z", Range(0,1)) = 1
		_Z0("Starting value Z", Range(0,1)) = 0
		_BendRateZ("Bend Rate Z", Float) = 0
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
			float _XMin;
			float _XMax;
			float _X0;
			float _BendRateX;

			float _YMin;
			float _YMax;
			float _Y0;
			float _BendRateY;

			float _ZMin;
			float _ZMax;
			float _Z0;
			float _BendRateZ;

			// FFD
			bool _IsOriginDown;

			// bezier curves degrees
			int _L;
			int _M;
			int _N;

			// Control points in FFD grid
			// NB: At most FFD_MAX_PTS points can be used!
			// This is done since dynamically sized arrays are not supported in hlsl
			float4 _ControlPoints[FFD_MAX_PTS];

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

				o = DoBend(o, X_AXIS, _XMin, _XMax, _X0, _BendRateX, _MaxExtents );
				o = DoBend(o, Y_AXIS, _YMin, _YMax, _Y0, _BendRateY, _MaxExtents );
				o = DoBend(o, Z_AXIS, _ZMin, _ZMax, _Z0, _BendRateZ, _MaxExtents );

				// Apply FREE-FORM DEFORMATION (lattice)
				o = DoFFD(o, _IsOriginDown, _L, _M, _N, _ControlPoints, _MaxExtents);

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
