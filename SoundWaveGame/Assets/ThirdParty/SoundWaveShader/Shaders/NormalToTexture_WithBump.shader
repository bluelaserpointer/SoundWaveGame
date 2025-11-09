Shader "Hidden/NormalToTexture_WithBump"
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _BumpMap;

            struct appdata
            {
                float4 vertex  : POSITION;
                float3 normal  : NORMAL;
                float4 tangent : TANGENT;   // 需要切线
                float2 uv      : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 tWS : TEXCOORD0;
                float3 bWS : TEXCOORD1;
                float3 nWS : TEXCOORD2;
                float2 uv  : TEXCOORD3;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // 世界空间 TBN
                float3 nWS = UnityObjectToWorldNormal(v.normal);
                float3 tWS = UnityObjectToWorldDir(v.tangent.xyz);
                tWS = normalize(tWS);
                nWS = normalize(nWS);
                float3 bWS = normalize(cross(nWS, tWS) * v.tangent.w);

                o.tWS = tWS;
                o.bWS = bWS;
                o.nWS = nWS;
                o.uv  = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // 切线空间法线（Unity 内置解包）
                float3 nTS = UnpackNormal(tex2D(_BumpMap, i.uv)); // [-1,1]

                // TBN: TS -> WS
                float3x3 TBN = float3x3(i.tWS, i.bWS, i.nWS);
                float3 nWS = normalize(mul(TBN, nTS));

                // WS -> VS（方向向量 w=0）
                float3 nVS = mul((float3x3)UNITY_MATRIX_V, nWS);

                // 映射到 0..1 并输出
                return float4(normalize(nVS) * 0.5 + 0.5, 0);
            }
            ENDCG
        }
    }
}
