Shader "Custom/RadialAdditiveUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Range(0,360)) = 1
        _FillOrigin ("Fill Origin", Range(0,360)) = 90
        _FillClockwise ("Clockwise", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha One
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _FillAmount;
            float _FillOrigin;
            float _FillClockwise;
            float _CutoffSmooth;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float radialMask(float2 uv, float fillAmount, float originDeg, float clockwise)
            {
                // Map uv to -0.5..0.5 center
                float2 p = uv - 0.5;
                float angle = atan2(p.y, p.x) * 57.29577951308232;
                if (angle < 0) angle += 360;
                float origin = fmod(originDeg + 180 + 360, 360);
                float sweep = fillAmount;
                float rel;
                if (clockwise >= 0.5)
                {
                    // clockwise: angle decreases from origin
                    rel = origin - angle;
                }
                else
                {
                    // counter-clockwise: angle increases from origin
                    rel = angle - origin;
                }
                rel = fmod(rel + 720.0, 360.0);
                // if rel <= sweep -> inside
                float inside = step(rel, sweep);
                return inside;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
                if (tex.a <= 0.001) discard;

                float mask = radialMask(i.uv, _FillAmount, _FillOrigin, _FillClockwise);

                float2 p = i.uv - 0.5;
                float dist = length(p);
                float hole = smoothstep(0.2, 0.2 + 0.1, dist);
                
                tex.a *= mask * hole;

                return tex;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}