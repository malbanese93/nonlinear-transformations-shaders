Shader "Example/TwistSurface" {
    Properties {

    }
    SubShader {
      Tags { "RenderType" = "Opaque" }

      CGPROGRAM

	  // Use standard Unity surface Shader to write as less code as possible
	  // The only thing we need is to change vertex data with the ones given
	  // by compute shader
	  #pragma surface surf Lambert
	  #pragma target 5.0

	  // Same as in CPU
	  struct VertexData {
	  	float3 pos;
	  	float3 normal;
	  };

	  struct Input {
          float4 color : COLOR;
      };

	  // NB: compute shaders use DX11 syntax! No support for previous architectures (use vshader instead)
	  #ifdef SHADER_API_D3D11
		uniform StructuredBuffer<VertexData> buffer;
	  #endif
	  
	  // Just provide vertex ID and basic data info
      void vert (inout appdata_base v, uint id : SV_VertexID) {
		  // Every time you access the buffer you need to add this preprocessing directive
          #ifdef SHADER_API_D3D11
		  v.vertex.xyz = buffer[id].pos;
		  v.normal     = buffer[id].normal;
		  #endif
	  }

	  // Fragment Shader
		void surf (Input IN, inout SurfaceOutput o)
		{
			o.Albedo = 1;
		}

      ENDCG
    }
    Fallback "Diffuse"
  }
