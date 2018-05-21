// Implement y-bend
Shader "Custom/BendShader"
{
	Properties
    {
		// min/max y to use for transformation
    	_YMin("Min Value", Float) = 0
		_YMax("Max Value", Float) = 0
		_Y0("Bend Center", Float) = 0

		// velocity of bending
		_BendRate("Bend Rate", Float) = 1

		// Which axis?
		[Enum(X,0,Y,1,Z,2)]_BendAxis("Bend around", Int) = 2

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

			float _YMin, _YMax;
			int _BendAxis;
			float _Y0;
			float _BendRate;

            // vertex shader
            v2f vert (appdata_base v)
            {
                v2f o;

				// Setup
				float x = v.vertex.x;
				float y = v.vertex.y;
				float z = v.vertex.z;
				float w = v.vertex.w;

				// first of all, get angle theta
				float yHat = clamp(y, _YMin, _YMax);
				float theta = _BendRate * (yHat - _Y0);
				float c = cos(theta), s = sin(theta);

				// apply transformation
				// let's start with the simplest ones
				o.vertex.x = x;
				o.vertex.w = w;

				// y and z have formula that change according to region
				// Y
				o.vertex.y = -s * (z-1.0f/_BendRate) + _Y0; // common part

				if( y < _YMin ) {
					o.vertex.y += c * (y - _YMin);
				} else if( y > _YMax ) {
					o.vertex.y += c * (y - _YMax);
				}

				// Z
				o.vertex.z = c * (z-1.0f/_BendRate) + 1.0f / _BendRate;

				if( y < _YMin ) {
					o.vertex.z += s * (y - _YMin);
				} else if( y > _YMax ) {
					o.vertex.z += s * (y - _YMax);
				}


				/*
				if( _BendAxis == 0 )
					o.vertex = mul(XtoZAxis, o.vertex);
				else if( _BendAxis == 1 )
					o.vertex = mul(YtoZAxis, o.vertex);
					*/
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
