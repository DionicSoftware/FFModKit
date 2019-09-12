Shader "Unlit/MyGridShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
      	_Alpha ("Alpha", Range(0,1)) = 0.1
      	_Emissive ("Emissive", Range(0,8)) = 1
		_Offset ("Offset", Range(0,0.5)) = 0
		_Thickness ("Thickness", Range(0,0.3)) = 0.1
		_Breaks("Breaks", Int) = 0

    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
		
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 world : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Color;
			float _Alpha;
			float _Emissive;
			float _Offset;
			float _Thickness;
			int _Breaks;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
				o.world = mul(unity_ObjectToWorld, v.vertex).xz + _Offset;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

				float2 worldMod = i.world % 1;
				float minDistFromLine = min(min(abs(worldMod.x-0), abs(worldMod.x-1)), min(abs(worldMod.y-0), abs(worldMod.y-1)));
				//if (i.world.x%1 < 0.04 || i.world.x%1 > 0.96 || i.world.y%1 < 0.04 || i.world.y%1 > 0.96) {

				col.rgb = _Color.rgb * _Emissive;
				
				float alphaThreshold = _Thickness;
				float solidThreshold = alphaThreshold*0.1;

				float alpha = clamp(1-(minDistFromLine-solidThreshold) * 1/(alphaThreshold-solidThreshold),0,1);
				alpha = alpha*alpha*alpha*alpha;
				col.a = alpha*_Alpha;

				if (_Breaks > 0) {
					float breakFactor = (_Breaks+1.0);
					float2 worldMod2 = ((i.world*breakFactor+breakFactor/2) % 1);
					float minDistFromBreak = min(min(abs(worldMod2.x-0), abs(worldMod2.x-1)), min(abs(worldMod2.y-0), abs(worldMod2.y-1)));
					if (minDistFromBreak < 0.25) {
						col.a = 0;
					}
				}


                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
