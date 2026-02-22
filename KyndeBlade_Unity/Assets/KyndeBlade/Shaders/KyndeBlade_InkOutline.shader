Shader "KyndeBlade/Ink Outline"
{
    // Post-process: Sobel-style edge detection for a drawn ink outline (manuscript look).
    // Apply to a fullscreen pass; reads scene and draws dark outline.
    Properties
    {
        _OutlineColor ("Ink Color", Color) = (0.05, 0.02, 0.02, 1)
        _Thickness ("Outline Thickness", Range(0.5, 3)) = 1.2
        _Threshold ("Edge Threshold", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
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
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _Thickness;
            float _Threshold;

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float luminance(fixed3 c) { return dot(c, float3(0.299, 0.587, 0.114)); }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 ts = _MainTex_TexelSize.xy * _Thickness;
                float L = luminance(tex2D(_MainTex, uv).rgb);
                float Lx = luminance(tex2D(_MainTex, uv + float2(ts.x, 0)).rgb) - luminance(tex2D(_MainTex, uv - float2(ts.x, 0)).rgb);
                float Ly = luminance(tex2D(_MainTex, uv + float2(0, ts.y)).rgb) - luminance(tex2D(_MainTex, uv - float2(0, ts.y)).rgb);
                float edge = sqrt(Lx*Lx + Ly*Ly);
                edge = saturate((edge - _Threshold) / max(0.01, 1 - _Threshold));
                fixed4 scene = tex2D(_MainTex, uv);
                fixed4 ink = _OutlineColor;
                ink.a = edge;
                return lerp(scene, ink, ink.a);
            }
            ENDCG
        }
    }
    Fallback Off
}
