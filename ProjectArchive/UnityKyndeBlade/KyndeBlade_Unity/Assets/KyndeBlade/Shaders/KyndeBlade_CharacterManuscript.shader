Shader "KyndeBlade/Character Manuscript (Toon + Satin + Ink)"
{
    // Pre-Raphaelite / illuminated-manuscript character look:
    // Step toon shading, satin specular (velvet/porcelain), ink outlines via inverted hull.
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        [Header(Toon)]
        _ToonSteps ("Toon Ramp Steps", Range(2, 8)) = 4
        _ShadowColor ("Shadow Tint (jewel shadow)", Color) = (0.25, 0.2, 0.35, 1)
        [Header(Satin Specular)]
        _SatinColor ("Satin Highlight Color", Color) = (0.95, 0.92, 0.88, 1)
        _SatinPower ("Satin Softness (higher = tighter)", Range(8, 128)) = 32
        _SatinStrength ("Satin Strength", Range(0, 1)) = 0.5
        [Header(Ink Outline)]
        _OutlineColor ("Ink Color", Color) = (0.04, 0.03, 0.06, 1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.05)) = 0.015
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
        LOD 200

        // Pass 1: Inverted hull ink outline (back faces, extruded along normals)
        Pass
        {
            Name "InkOutline"
            Cull Front
            ZWrite On

            CGPROGRAM
            #pragma vertex vert_outline
            #pragma fragment frag_outline
            #include "UnityCG.cginc"

            float _OutlineWidth;
            float4 _OutlineColor;

            struct v2f_outline
            {
                float4 pos : SV_POSITION;
            };

            v2f_outline vert_outline(appdata_base v)
            {
                v2f_outline o;
                float3 norm = normalize(v.normal);
                float3 pos = v.vertex.xyz + norm * _OutlineWidth;
                o.pos = UnityObjectToClipPos(float4(pos, 1));
                return o;
            }

            fixed4 frag_outline(v2f_outline i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        // Pass 2: Step-toon + satin lit
        Pass
        {
            Name "ForwardBase"
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _ToonSteps;
            float4 _ShadowColor;
            float4 _SatinColor;
            float _SatinPower;
            float _SatinStrength;

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3)
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 N = normalize(i.worldNormal);
                float3 L = _WorldSpaceLightPos0.xyz;
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 H = normalize(L + V);

                float NdotL = dot(N, L);
                float NdotH = saturate(dot(N, H));

                // Step toon: discrete bands (jewel-tone shadows)
                float toon = floor(saturate(NdotL) * _ToonSteps) / _ToonSteps;
                toon = max(toon, 0.15); // avoid pure black
                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
                fixed3 diff = lerp(_ShadowColor.rgb, albedo, toon);

                // Satin specular: soft, broad highlight (velvet/porcelain)
                float satin = pow(NdotH, _SatinPower);
                satin *= _SatinStrength;
                fixed3 spec = _SatinColor.rgb * satin;

                fixed shadow = SHADOW_ATTENUATION(i);
                fixed3 col = (diff + spec) * _LightColor0.rgb * max(0.4, shadow);
                col += albedo * unity_AmbientSky.rgb * 0.3;

                return fixed4(col, 1);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
