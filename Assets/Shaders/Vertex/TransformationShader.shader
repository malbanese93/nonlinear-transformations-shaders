// Applies all transformations to loaded mesh
// THE ORDER IS THE FOLLOWING:
// 1) TWIST 	(default axis: z)
// 2) STRETCH 	(default axis: z)
// 3) BEND    	(default axis: y)
// Mainly based on "Global and local deformations of solid primitives" paper by Alan H. Barr

// By applying all transformations in a single shader file we can check all vertices once per pass
Shader "Custom/TransformationShader"
{
	// Here all properties are set from scripts outside
	// Inspector values are used for debugging purposes
	Properties
    {
		// Mesh max values for bounding box. Equal to two times max extents.
		[HideInInspector]_MaxExtents("Max extents", Vector) = (0,0,0,0)

		// === TWIST ===
		// Axis for twisting
		[Enum(X,X_AXIS, Y,Y_AXIS, Z,Z_AXIS)]_TwistAxis("Twist around", Int) = 2
		// Angle of rotation at the extremes (in degrees)
		_TwistAngle("Twist Angle", Range(-360,360)) = 0
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

            // Vertex Shader
            v2f vert (appdata_base v)
            {
                v2f o;

				// First of all copy vertices to output
				o.vertex = v.vertex;
				o.normal = v.normal;

				// Apply all transformations in sequence
				o = DoTwist(o);
				o = DoStretch(o);
				o = DoBend(o);
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
