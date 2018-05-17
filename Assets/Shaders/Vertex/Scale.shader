Shader "Custom/ScaleShader"
{
	Properties
    {
    	_Sx("Scale x", Float) = 1
		_Sy("Scale y", Float) = 1
		_Sz("Scale z", Float) = 1

		[Toggle] _Normalize("Normalize normal?", Int) = 0
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
                float3 normal : NORMAL;
            };

			float _Sx, _Sy, _Sz;
			int  _Normalize;

            // vertex shader
            v2f vert (appdata_base v)
            {
                v2f o;

				// TWIST around z axis
				float x = v.vertex.x;
				float y = v.vertex.y;
				float z = v.vertex.z;
				float w = v.vertex.w;

				o.vertex.x = _Sx * x;
				o.vertex.y = _Sy * y;
				o.vertex.z = _Sz * z;
				o.vertex.w = w;
				              
                //o.vertex = UnityObjectToClipPos(o.vertex);

                // Normals
				float nx = v.normal.x;
				float ny = v.normal.y;
				float nz = v.normal.z;

                o.normal.x = nx / _Sx;
				o.normal.y = ny / _Sy;
				o.normal.z = nz / _Sz;

				if( _Normalize != 0 )
					o.normal = normalize(o.normal);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c;
                c.xyz = i.normal * 0.5 + 0.5;
                c.w = 1;

                return c;
            }
            ENDCG
        }
    }
}
