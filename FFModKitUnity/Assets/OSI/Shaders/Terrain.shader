// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Terrain" {
	Properties {
		_NoiseTex ("NoiseTexture", 2D) = "white" {}
		_SplatTex ("SplatTexture", 2D) = "white" {}
		_GrassColor ("GrassColor", Color) = (1,1,1,1)
		_SplatColor ("SplatColor", Color) = (1,1,1,1)
		_GrassGlossiness ("GrassSmoothness", Range(0,1)) = 0.5
		_GrassMetallic ("GrassMetallic", Range(0,1)) = 0.0
		_SplatSmoothness ("SplatSmoothness", Range(0,1)) = 0.5
		_SplatMetallic ("SplatMetallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float3 vertexColor;
			float2 uv_NoiseTex;
			float2 uv_SplatTex;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
		}

		half _GrassSmoothness;
		half _GrassMetallic;
		half _SplatSmoothness;
		half _SplatMetallic;
		fixed4 _GrassColor;
		fixed4 _SplatColor;
		sampler2D _NoiseTex;
		sampler2D _SplatTex;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = _GrassColor;

			// distortion
			float2 offset = float2(tex2D(_NoiseTex, IN.uv_NoiseTex+fixed2(0.1,0.7)).r, tex2D(_NoiseTex, IN.uv_NoiseTex+fixed2(0.4,0.3)).g) + fixed2(0,0);
			//float2 offset = float2(0,0);
			offset = offset * 0.005*0.4;
			float dist= 0.003;

			// blurring
			float forestValue0 = tex2D(_SplatTex, IN.uv_SplatTex + offset).r;
			float forestValue1 = tex2D(_SplatTex, IN.uv_SplatTex + offset + fixed2(dist,0)).r;
			float forestValue2 = tex2D(_SplatTex, IN.uv_SplatTex + offset + fixed2(-dist,0)).r;
			float forestValue3 = tex2D(_SplatTex, IN.uv_SplatTex + offset + fixed2(0,dist)).r;
			float forestValue4 = tex2D(_SplatTex, IN.uv_SplatTex + offset + fixed2(0,-dist)).r;
			float forestValue = forestValue0/2.0 + forestValue1/8.0 + forestValue2/8.0 + forestValue3/8.0 + forestValue4/8.0;

			// unused: test mixing with noise
			//float mixedForestValue = forestValue + (tex2D(_NoiseTex, IN.uv_NoiseTex + offset).r-0.5);
			o.Albedo = forestValue < 0.5 ? _GrassColor.rgb : _SplatColor.rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = forestValue < 0.5 ? _GrassMetallic : _SplatMetallic;
			o.Smoothness = forestValue < 0.5 ? _GrassSmoothness : _SplatSmoothness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
