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
		// Axis for twisting
		[Enum(X,0,Y,1,Z,2)]_TwistAxis("Twist around", Int) = 2
		// Angle of rotation at the extremes (in degrees)
		_TwistAngle("Twist Angle", Range(-360,360)) = 0

		[Space(10)]
		[Header(A group of things)]
		// === STRETCH ===
		// Axis for stretching
		[Enum(X,0,Y,1,Z,2)]_StretchAxis("Stretch around", Int) = 2
		// How much to stretch along main axis
		_StretchAmount("Stretch Amount", Range(-2,2)) = 0
		// How much to exagerate stretch
		_StretchStrength("Stretch Strength", Range(0,3)) = 1

		[Space(10)]
		// === BEND ===
		// Axis for bending
		[Enum(X,0,Y,1,Z,2)]_BendAxis("Bend around", Int) = 2
		// Min and max y for transform (TODO: in percentage!)
		_YMin("Min value", Float) = 0
		_YMax("Max value", Float) = 0

		// Starting y0
		_Y0("Starting value", Float) = 0
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
			int _TwistAxis;
			float _TwistAngle;

			// STRETCH
			int _StretchAxis;
			float _StretchStrength;
			float _StretchAmount;

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

				// Apply all transformations in sequence
				o = DoTwist(o, _TwistAxis, _TwistAngle, _MaxExtents);
				o = DoStretch(o, _StretchAxis, _StretchAmount, _StretchStrength, _MaxExtents );
				o = DoBend(o, _BendAxis, _YMin, _YMax, _Y0, _BendRate, _MaxExtents );
				// TODO: lattice transformation

				// Finally, do MVP transformation as usual and return
				o.vertex = UnityObjectToClipPos(o.vertex);
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
