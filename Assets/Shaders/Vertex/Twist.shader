// Implement z-twist
Shader "Custom/TwistShader"
{
	Properties
    {
    	_Alpha("Alpha", Float/*Range(-0.05, 0.05)*/) = 0	
		[Enum(X,0,Y,1,Z,2)]_Axis("Twist around", Int) = 2
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

			float _Alpha;
			int _Axis;

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

				if( _Axis == 0 )
					v.vertex = mul(ZtoXAxis, v.vertex);
				else if( _Axis == 1 )
					v.vertex = mul(ZtoYAxis, v.vertex);
				
				// Setup
				float x = v.vertex.x;
				float y = v.vertex.y;
				float z = v.vertex.z;
				float w = v.vertex.w;

				// TWIST around z axis
				float theta = _Alpha * v.vertex.z;
				float dtheta = _Alpha;

				float c = cos(theta);
				float s = sin(theta);

				o.vertex.x = x*c - y*s;
				o.vertex.y = x*s + y*c;
				o.vertex.z = z;
				o.vertex.w = w;
				
				if( _Axis == 0 )
					o.vertex = mul(XtoZAxis, o.vertex);
				else if( _Axis == 1 )
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
