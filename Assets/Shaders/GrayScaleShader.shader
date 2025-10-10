Shader "Unlit/GrayScaleShader"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _Lerp ("Grayscale Lerp", Range(0,1)) = 0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _BaseColor ("Color", Color) = (0,0,0,0)
        
        _NoiseTex ("Noise", 2D) = "white" {}
        _RampTex ("Ramp", 2D) = "white" {}
        _BurnAmount ("Burn", Range(0,1)) = 0
        _NoiseScale ("NoiseScale", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="Geometry" }
        LOD 100

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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _BaseMap;
            sampler2D _NoiseTex;
            sampler2D _RampTex;
            float4 _BaseMap_ST;
            float _Lerp;
            float _Cutoff;
            float4 _BaseColor;
            float _BurnAmount;
            float _NoiseScale;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_BaseMap, i.uv) * _BaseColor;

                fixed noise = max(tex2D(_NoiseTex, i.uv * _NoiseScale).r, 0.1);

                clip(min(col.a - _Cutoff, noise - _BurnAmount));

                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float3 grayRGB = float3(gray, gray, gray);
                col.rgb = lerp(col.rgb, grayRGB, _Lerp);

                float edge = smoothstep(_BurnAmount-0.05, _BurnAmount, noise) - smoothstep(_BurnAmount, _BurnAmount+0.05, noise);

                fixed3 fireColor = tex2D(_RampTex, float2(edge,0)).rgb;
                col.rgb = lerp(col.rgb, fireColor, edge);
                
                return col;
            }
            ENDCG
        }
    }
}
