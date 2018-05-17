Shader "Custom/TwistShader"
{
	Properties
    {
    	_Alpha("Alpha", Range(-0.05, 0.05)) = 0	
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

            // vertex shader
            v2f vert (appdata_base v)
            {
                v2f o;

				// TWIST around z axis
				float theta = _Alpha * v.vertex.z;
				float dtheta = _Alpha;

				float c = cos(theta);
				float s = sin(theta);

				float x = v.vertex.x;
				float y = v.vertex.y;
				float z = v.vertex.z;
				float w = v.vertex.w;

				o.vertex.x = x*c - y*s;
				o.vertex.y = x*s + y*c;
				o.vertex.z = z;
				o.vertex.w = w;
				              
                // MVP projection
                o.vertex = UnityObjectToClipPos(o.vertex);

                // Normals
				float nx = v.normal.x;
				float ny = v.normal.y;
				float nz = v.normal.z;

                o.normal.x = c*nx - s*ny;
				o.normal.y = s*nx + c*ny;
				o.normal.z = y*dtheta*nx - x*dtheta*ny + nz;
				
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
