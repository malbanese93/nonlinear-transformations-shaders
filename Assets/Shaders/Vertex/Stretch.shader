// Implement z-stretch
Shader "Custom/StretchShader"
{
	Properties
    {
		// STRETCH
		// How much to stretch along z axis
    	_StretchCoeff("Stretch Coefficient", Range(-2,2)) = 0

		// Which Axis to choose?
		[Enum(X,0,Y,1,Z,2)]_StretchAxis("Stretch around", Int) = 2
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

			float _StrechStrength;
			float _StretchCoeff;
			int _StretchAxis;
			

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

				if( _StretchAxis == 0 )
					v.vertex = mul(ZtoXAxis, v.vertex);
				else if( _StretchAxis == 1 )
					v.vertex = mul(ZtoYAxis, v.vertex);

				// Setup
				float x = v.vertex.x;
				float y = v.vertex.y;
				float z = v.vertex.z;
				float w = v.vertex.w;

				o.vertex.x = x * (z * z * _StretchCoeff - _StretchCoeff + 1.0);
				o.vertex.y = y * (z * z * _StretchCoeff - _StretchCoeff + 1.0);
				o.vertex.z = z * ( 1.0 + _StretchCoeff );
				o.vertex.w = w;

				if( _StretchAxis == 0 )
					o.vertex = mul(XtoZAxis, o.vertex);
				else if( _StretchAxis == 1 )
					o.vertex = mul(YtoZAxis, o.vertex);

                // MVP projection
                o.vertex = UnityObjectToClipPos(o.vertex);

                // Normals
				// IGNORED FOR NOW
				o.normal = v.normal;

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
