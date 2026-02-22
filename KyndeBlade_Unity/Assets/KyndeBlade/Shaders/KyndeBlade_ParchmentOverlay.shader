Shader "KyndeBlade/Parchment Overlay"
{
    // Multiplies a grain/paper texture over the camera view so the game looks drawn on parchment.
    // Use with a fullscreen quad or post-process: render with this after the scene.
    Properties
    {
        _ParchmentTex ("Parchment / Grain Texture", 2D) = "gray" {}
        _Strength ("Blend Strength", Range(0, 1)) = 0.4
        _Tint ("Parchment Tint", Color) = (0.92, 0.88, 0.78, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
        Blend DstColor Zero
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
            float4 _Tint;

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 scene = tex2D(_MainTex, i.uv);
                fixed4 parchment = tex2D(_ParchmentTex, i.uv * _ParchmentTex_ST.xy + _ParchmentTex_ST.zw);
                parchment.rgb *= _Tint.rgb;
                // Multiply blend: result = scene * parchment (darkens and adds paper)
                fixed4 outCol = scene * lerp(fixed4(1,1,1,1), parchment, _Strength);
                outCol.a = 1;
                return outCol;
            }
            ENDCG
        }
    }
    Fallback Off
}
