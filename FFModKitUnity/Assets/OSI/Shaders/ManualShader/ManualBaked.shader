// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Manual/Baked" {
	Properties {
		_MainTex ("Albedo", 2D) = "white" {}
		_Metallic ("Metallic", 2D) = "black" {}
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
			float2 uv_MainTex;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
		}

		sampler2D _MainTex;
		sampler2D _Metallic;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = IN.vertexColor * c.rgb;
			o.Alpha = c.a;
			fixed4 mc = tex2D(_Metallic, IN.uv_MainTex);
			o.Metallic = mc.r;
			o.Smoothness = mc.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
