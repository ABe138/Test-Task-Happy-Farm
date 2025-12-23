Shader "Custom/RadialAdditiveUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Range(0,360)) = 1
        _FillOrigin ("Fill Origin", Range(0,360)) = 90
        _FillClockwise ("Clockwise", Float) = 1
        _CutoffSmooth ("Edge Smooth", Range(0,0.1)) = 0.01
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

            // returns radial mask value in [0,1]
            float radialMask(float2 uv, float fillAmount, float originDeg, float clockwise)
            {
                // Map uv to -0.5..0.5 center
                float2 p = uv - 0.5;
                // compute angle in degrees 0..360 where 0 is +X (right) and increases CCW
                float angle = atan2(p.y, p.x) * 57.29577951308232;
                if (angle < 0) angle += 360;
                // convert origin (deg) to angle where fill sweeps from origin clockwise/ccw
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
                // normalize to 0..360
                rel = fmod(rel + 720.0, 360.0);
                // if rel <= sweep -> inside
                float inside = step(rel, sweep);
                // smooth edge by distance from radial boundary (optional)
                // Compute signed difference to boundary in degrees and smooth slightly
                float distDeg = rel - sweep;
                float soft = smoothstep(0.0, _CutoffSmooth * 360.0, -distDeg);
                //return lerp(inside, soft, 1.0); // inside or softened
                return inside; // inside or softened
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
                // If texture is fully transparent, save cost
                if (tex.a <= 0.001) discard;

                float mask = radialMask(i.uv, _FillAmount, _FillOrigin, _FillClockwise);

                float2 p = i.uv - 0.5;
                float dist = length(p);
                // smooth factor: 0 -> inside hole, 1 -> outside
                float hole = smoothstep(0.2, 0.2 + 0.1, dist);
                

                // Multiply alpha by mask to preserve original alpha shape
                tex.a *= mask * hole;
                // Premultiply color by alpha? Not necessary for additive blend, but keep color multiplied by alpha
                //tex.rgb *= tex.a;

                return tex;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}