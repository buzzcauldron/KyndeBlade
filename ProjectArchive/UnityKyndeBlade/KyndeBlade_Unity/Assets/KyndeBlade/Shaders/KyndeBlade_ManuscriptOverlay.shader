Shader "KyndeBlade/Manuscript Overlay (Full Screen)"
{
    // 14th-century page: multiply parchment texture, grain, and sepia tint over final render.
    // Use with ManuscriptOverlayEffect (built-in) or ManuscriptOverlayPass (URP).
    Properties
    {
        _MainTex ("Scene", 2D) = "white" {}
        _ParchmentTex ("Parchment / Paper", 2D) = "gray" {}
        _Strength ("Parchment Blend", Range(0, 1)) = 0.45
        _SepiaTint ("Sepia Tint (aged vellum)", Color) = (0.76, 0.68, 0.55, 1)
        _SepiaStrength ("Sepia Strength", Range(0, 1)) = 0.35
        _Grain ("Grain / Noise", Range(0, 0.15)) = 0.06
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
        ZTest Always
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _ParchmentTex;
            float4 _ParchmentTex_ST;
            float _Strength;
            float4 _SepiaTint;
            float _SepiaStrength;
            float _Grain;

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 scene = tex2D(_MainTex, i.uv);
                fixed4 parchment = tex2D(_ParchmentTex, i.uv * _ParchmentTex_ST.xy + _ParchmentTex_ST.zw);
                // Multiply with parchment for paper surface
                fixed3 col = scene.rgb * lerp(fixed3(1,1,1), parchment.rgb, _Strength);
                // Sepia (aged vellum)
                float lum = dot(col, float3(0.299, 0.587, 0.114));
                col = lerp(col, lerp(col, _SepiaTint.rgb, lum), _SepiaStrength);
                // Grain
                float n = hash(i.uv * _ScreenParams.xy + _Time.y);
                col += (n - 0.5) * _Grain;
                return fixed4(col, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}
