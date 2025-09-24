Shader "Unlit/Test1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _OutColor ("OutColor", Color) = (1,1,1,1)
        _InColor ("InColor", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _Border ("Border", Range(0,0.5)) = 0.5
        _OutLimit ("OutLimit", Range(0,0.5)) = 0.5
        _InLimit ("InLimit", Range(0,0.5)) = 0.5
        _Speed ("Speed", Range(0, 10)) = 1
        _NoiseScale ("NoiseScale", Range(0, 10)) = 1
        _InAlpha ("InAlpha", Range(0,1)) = 1
        _Alpha ("Alpha", Range(0,1)) = 1
        _OutAlpha ("OutAlpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
        }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NoiseTex;
            float4 _InColor;
            float4 _OutColor;
            float4 _Color;

            float _Cutoff;
            float _Border;
            float _OutLimit;
            float _InLimit;
            float _Speed;
            float _NoiseScale;
            float _OutAlpha;
            float _Alpha;
            float _InAlpha;


            fixed get_circle_dist(float2 pos)
            {
                return length(pos - float2(0.5, 0.5)) * 0.91;
            }

            fixed get_squre_dist(float2 pos)
            {
                float2 p = abs(pos - float2(0.5, 0.5));
                float P = 8.0; // 4~16 사이에서 취향대로
                return pow(pow(p.x, P) + pow(p.y, P), 1.0 / P);
            }
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 noise = tex2D(_NoiseTex, i.uv + _Time * _Speed);
                float2 dir = normalize(i.uv - float2(0.5, 0.5)) * _NoiseScale * 0.1;
                i.uv = i.uv + dir * (noise.r - 0.5) * 0.5;
                
                fixed2 dist;
                if (i.uv.y > 0.7)
                {
                    dist = get_circle_dist(i.uv);
                }
                else if (i.uv.y < 0.6)
                {
                    dist = get_squre_dist(i.uv);
                }
                else
                {
                    dist = lerp(get_squre_dist(i.uv),  get_circle_dist(i.uv), (i.uv.y - 0.6) / 0.1);
                }

                fixed m_dist = clamp(dist, _InLimit, _OutLimit);

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                if (m_dist > _Border)
                {
                    fixed t = (m_dist - _Border) / (_OutLimit - _Border);
                    col.rgb = lerp(_Color, _OutColor, smoothstep(0, 1, t));
                    col.a *= lerp(_Alpha, _OutAlpha, smoothstep(0, 1, t));
                }
                else
                {
                    fixed t = (m_dist - _InLimit) / (_Border - _InLimit);
                    col.rgb = lerp(_InColor, _Color, smoothstep(0, 1, t));
                    col.a *= lerp(_InAlpha, _Alpha, smoothstep(0, 1, t));
                }
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                clip(col.a - _Cutoff);

                return col;
            }
            ENDCG
        }
    }
}