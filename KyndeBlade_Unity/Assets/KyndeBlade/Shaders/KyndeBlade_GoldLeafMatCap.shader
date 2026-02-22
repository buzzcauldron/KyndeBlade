Shader "KyndeBlade/Gold Leaf (MatCap)"
{
    // Manuscript-style gold: flat metallic sticker look. Uses a MatCap (Material Capture) texture
    // so the gold reflects a static "golden cathedral" environment regardless of scene lights.
    // Use on Green Knight trim, Pride's armor, marginalia.
    Properties
    {
        _MatCapTex ("MatCap (Golden Cathedral)", 2D) = "white" {}
        _Tint ("Gold Tint", Color) = (0.92, 0.78, 0.35, 1)
        _Darken ("Shadow / Recess", Range(0, 1)) = 0.25
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MatCapTex;
            float4 _MatCapTex_ST;
            float4 _Tint;
            float _Darken;

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 viewNormal : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                // Transform normal to view space for MatCap lookup (flat reflection, no world lights)
                float3 n = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                o.viewNormal = normalize(n);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // MatCap UV: view-space normal xy mapped to 0..1 (standard hemisphere lookup)
                float2 matcapUV = i.viewNormal.xy * 0.5 + 0.5;
                fixed4 matcap = tex2D(_MatCapTex, matcapUV);
                fixed3 col = matcap.rgb * _Tint.rgb;
                // Slight darkening for depth (recessed areas)
                float fade = 1.0 - _Darken * (1.0 - matcap.a);
                col *= fade;
                return fixed4(col, 1);
            }
            ENDCG
        }
    }
    Fallback "Unlit/Texture"
}
