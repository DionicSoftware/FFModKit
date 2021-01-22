// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

	Shader "Custom/GradientColorChange" {
	Properties {
		_Color1 ("Color1", Color) = (1,1,1,1)
		_Color2 ("Color2", Color) = (0,0,0,1)
		_Progress ("Progress", float) = 0.5
		_GradientTex("Gradient Texture", 2D) = "white"
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha noshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _GradientTex;
		float _Progress;

		struct Input {
			float2 uv_GradientTex;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			//o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
		}

		fixed4 _Color1;
		fixed4 _Color2;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float4 tex = tex2D(_GradientTex, IN.uv_GradientTex);
			if (tex.a < 0.99)
				discard;
			// Albedo comes from a texture tinted by color
			if (tex.r > 1 - _Progress) {
				o.Albedo = _Color1.rgb;
				o.Alpha = _Color1.a;
			}
			else {
				o.Albedo = _Color2.rgb;
				o.Alpha = _Color2.a;
			}
			// Metallic and smoothness come from slider variables
			o.Metallic = 0;
			o.Smoothness = 0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
