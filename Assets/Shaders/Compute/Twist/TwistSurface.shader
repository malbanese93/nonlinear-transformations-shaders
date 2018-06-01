// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Example/TwistSurface" {
    Properties {
		[HideInInspector]_VertexCount("Vertex Count", Int) = 0
    }
    SubShader {
        Pass {
          Tags { "RenderType" = "Opaque" }

          CGPROGRAM

    	  // Use standard Unity surface Shader to write as less code as possible
    	  // The only thing we need is to change vertex data with the ones given
    	  // by compute shader
    	  #pragma vertex vert
    	  #pragma fragment frag
    	  #pragma target 5.0

    	  #include "UnityCG.cginc"

    		// Same as in CPU
    		struct VertexData {
    			float3 pos;
    			float3 normal;
    		};

    		// data structure from vertex to fragment shader
			struct v2f {
        		float4 vertex : SV_POSITION;
        		float3 normal : TEXCOORD0;
			};

    	  // NB: compute shaders use DX11 syntax! No support for previous architectures (use vshader instead)
    	  #ifdef SHADER_API_D3D11
    		uniform StructuredBuffer<VertexData> buffer;
    	  #endif

    	  // Just provide vertex ID and basic data info
          v2f vert (uint id : SV_VertexID) {
    		  // Every time you access the buffer you need to add this preprocessing directive
              v2f o;

			  #ifdef SHADER_API_D3D11
    		  o.vertex = UnityObjectToClipPos(float4(buffer[id].pos,1));
    		  o.normal = buffer[id].normal;
    		  #endif

    		  return o;
    	  }

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
