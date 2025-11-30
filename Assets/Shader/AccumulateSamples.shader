Shader "Hidden/AccumulateSamples"
{
    Properties
    {
        _MainTex ("New Sample", 2D) = "white" {} // from targetRT
        _History ("Previous Accumulation", 2D) = "white" {} // from convergedRT
        _Sample ("Sample Count", Float) = 0 // sampleCount
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _History;
            float _Sample; // current sample index (0, 1, 2, ...)


            float4 frag(v2f i) : SV_Target
            {
                float3 currentSample = tex2D(_MainTex, i.uv).rgb; // new sample from current frame
                float3 accumulatedColor = tex2D(_History, i.uv).rgb; // accumulated color from previous frames

                // Compute blending weights
                float weightCurrent = 1.0 / (_Sample + 1.0); // smaller weight for new sample
                float weightAccum = _Sample / (_Sample + 1.0); // larger weight for accumulated history

                // Progressive running average
                float3 result = (accumulatedColor * weightAccum) + (currentSample * weightCurrent);

                return float4(result, 1);
            }

            ENDCG
        }
    }
}