Shader "KyndeBlade/Gold Leaf"
{
    // Flat, metallic shine like real gold leaf on a manuscript page (not glossy 3D).
    // Use for UI borders, boss name plates, marginalia.
    Properties
    {
        _Color ("Base Color", Color) = (0.75, 0.6, 0.2, 1)
        _Specular ("Specular (flat shine)", Color) = (0.95, 0.9, 0.6, 1)
        _Flatness ("Flatness (0=normal 1=always bright)", Range(0, 1)) = 0.7
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
            #include "Lighting.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

            float4 _Color;
            float4 _Specular;
            float _Flatness;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 N = normalize(i.worldNormal);
                float3 L = _WorldSpaceLightPos0.xyz;
                float NdotL = saturate(dot(N, L));
                // Flat gold: reduce variation so it looks like leaf, not 3D metal
                float flat = lerp(NdotL, 1, _Flatness);
                fixed3 diff = _Color.rgb * (0.4 + 0.6 * flat);
                fixed3 spec = _Specular.rgb * pow(flat, 4);
                fixed3 col = diff + spec * 0.5;
                return fixed4(col, 1);
            }
            ENDCG
        }
    }
    Fallback "Unlit/Color"
}
