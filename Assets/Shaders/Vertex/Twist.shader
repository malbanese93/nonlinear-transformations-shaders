// Implement z-twist
Shader "Custom/TwistShader"
{
	Properties
    {
		// TWIST
    	//_TwistCoeff("Twist Coefficient", Range(-0.5, 0.5)) = 0
		[HideInInspector]_MaxExtent("Max value", Float) = 0
		// Which axis?
		[Enum(X,0,Y,1,Z,2)]_TwistAxis("Twist around", Int) = 2

		_Angle("Twist Angle", Range(-360,360)) = 0
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			// data structure from vertex to fragment shader
            struct v2f {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

			//float _TwistCoeff;
			float _MaxExtent;
			int _TwistAxis;
			float _Angle;

            // vertex shader
            v2f vert (appdata_base v)
            {
                v2f o;

				// ====== ROTATE TO GET AROUND Y AXIS ======
				float4x4 ZtoYAxis = {
				   1,0,0,0,
				   0,0,-1,0,
				   0,1,0,0,
				   0,0,0,1
				};
				float4x4 YtoZAxis = transpose(ZtoYAxis);

				// ===== ROTATE TO GET AROUND X AXIS ======
				float4x4 ZtoXAxis = {
					0,0,1,0,
					0,1,0,0,
					-1,0,0,0,
					0,0,0,1
				};
				float4x4 XtoZAxis = transpose(ZtoXAxis);

				if( _TwistAxis == 0 )
					v.vertex = mul(ZtoXAxis, v.vertex);
				else if( _TwistAxis == 1 )
					v.vertex = mul(ZtoYAxis, v.vertex);

				// Setup
				float x = v.vertex.x;
				float y = v.vertex.y;
				float z = v.vertex.z;
				float w = v.vertex.w;

				// TWIST around z axis
				// Rotate by angle theta = f(z) = a*z + b
				/*float theta = _TwistCoeff * v.vertex.z;
				float dtheta = _TwistCoeff;*/

				// Rotate by angle theta = f(z) = lerp(0,z,z*,0,theta*)
				// where z* is max z (_MaxExtent) and theta* is angle set by user
				float theta = (z / _MaxExtent) * radians(_Angle);
				float c = cos(theta);
				float s = sin(theta);

				o.vertex.x = x*c - y*s;
				o.vertex.y = x*s + y*c;
				o.vertex.z = z;
				o.vertex.w = w;

				if( _TwistAxis == 0 )
					o.vertex = mul(XtoZAxis, o.vertex);
				else if( _TwistAxis == 1 )
					o.vertex = mul(YtoZAxis, o.vertex);

                // MVP projection
                o.vertex = UnityObjectToClipPos(o.vertex);

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

            fixed4 frag (v2f i) : SV_Target
            {
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
