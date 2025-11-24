

Shader "Hidden/AddShader"
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
                float3 newSample = tex2D(_MainTex, i.uv).rgb;
                float3 oldAccum = tex2D(_History, i.uv).rgb;

                // Compute weight
                float weightNew = 1.0 / (_Sample + 1.0);
                float weightOld = _Sample / (_Sample + 1.0);

                // Explicit averaging.
                float3 result = oldAccum * weightOld + newSample * weightNew;

                return float4(result, 1);
            }
            ENDCG
        }
    }
}