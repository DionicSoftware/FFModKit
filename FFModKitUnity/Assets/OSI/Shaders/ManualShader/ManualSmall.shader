// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Manual/ManualSmall" {
	Properties {
		_Color1 ("Color 1", Color) = (1,1,1,1)
		_Glossiness1 ("Smoothness 1", Range(0,1)) = 0.3
		_Metallic1 ("Metallic 1", Range(0,1)) = 0.0
		_Color2 ("Color 2", Color) = (1,1,1,1)
		_Glossiness2 ("Smoothness 2", Range(0,1)) = 0.3
		_Metallic2 ("Metallic 2", Range(0,1)) = 0.0
		_Color3 ("Color 3", Color) = (1,1,1,1)
		_Glossiness3 ("Smoothness 3", Range(0,1)) = 0.3
		_Metallic3 ("Metallic 3", Range(0,1)) = 0.0
		_Color4 ("Color 4", Color) = (1,1,1,1)
		_Glossiness4 ("Smoothness 4", Range(0,1)) = 0.3
		_Metallic4 ("Metallic 4", Range(0,1)) = 0.0
		_Color5 ("Color 5", Color) = (1,1,1,1)
		_Glossiness5 ("Smoothness 5", Range(0,1)) = 0.3
		_Metallic5 ("Metallic 5", Range(0,1)) = 0.0
		_Color6 ("Color 6", Color) = (1,1,1,1)
		_Glossiness6 ("Smoothness 6", Range(0,1)) = 0.3
		_Metallic6 ("Metallic 6", Range(0,1)) = 0.0
		_Color7 ("Color 7", Color) = (1,1,1,1)
		_Glossiness7 ("Smoothness 7", Range(0,1)) = 0.3
		_Metallic7 ("Metallic 7", Range(0,1)) = 0.0
		_Color8 ("Color 8", Color) = (1,1,1,1)
		_Glossiness8 ("Smoothness 8", Range(0,1)) = 0.3
		_Metallic8 ("Metallic 8", Range(0,1)) = 0.0
		_Color9 ("Color 9", Color) = (1,1,1,1)
		_Glossiness9 ("Smoothness 9", Range(0,1)) = 0.3
		_Metallic9 ("Metallic 9", Range(0,1)) = 0.0
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

		half _Glossiness1;
		half _Metallic1;
		half _Glossiness2;
		half _Metallic2;
		half _Glossiness3;
		half _Metallic4;
		half _Glossiness4;
		half _Metallic5;
		half _Glossiness5;
		half _Metallic6;
		half _Glossiness6;
		half _Metallic7;
		half _Glossiness7;
		half _Metallic8;
		half _Glossiness8;
		half _Metallic9;
		half _Glossiness9;
		half _Metallic3;
		fixed4 _Color1;
		fixed4 _Color2;
		fixed4 _Color3;
		fixed4 _Color4;
		fixed4 _Color5;
		fixed4 _Color6;
		fixed4 _Color7;
		fixed4 _Color8;
		fixed4 _Color9;
		sampler2D _MainTex;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			
			float4 colors[9] = {_Color1, _Color2, _Color3, _Color4, _Color5, _Color6, _Color7, _Color8, _Color9};
			int index = ((int)(IN.uv_MainTex.x*4)*4 + (int)(IN.uv_MainTex.y*4))%16;
			index = clamp(index, 0, 8);
			o.Albedo = IN.vertexColor * colors[index];
			float smoothness[9] = {_Glossiness1, _Glossiness2, _Glossiness3, _Glossiness4, _Glossiness5, _Glossiness6, _Glossiness7, _Glossiness8, _Glossiness9};
			float metallic[9] = {_Metallic1, _Metallic2, _Metallic3, _Metallic4, _Metallic5, _Metallic6, _Metallic7, _Metallic8, _Metallic9};
			o.Metallic = metallic[index];
			o.Smoothness = smoothness[index];
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
