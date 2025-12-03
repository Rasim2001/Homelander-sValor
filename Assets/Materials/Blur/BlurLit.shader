Shader "Unlit/GaussianBlurY"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 10)) = 2
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _BlurSize;

            static const float weights[5] = {0.06, 0.098, 0.16, 0.098, 0.06};

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                float2 blurStep = float2(0, _BlurSize / _ScreenParams.y);

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * weights[2];

                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + blurStep) * weights[3];
                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - blurStep) * weights[3];

                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + 2 * blurStep) * weights[4];
                col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - 2 * blurStep) * weights[4];

                col.a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
                return col;
            }
            ENDHLSL
        }
    }
}