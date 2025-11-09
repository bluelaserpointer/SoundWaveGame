Shader "Hidden/SoundBorder"
{
    //用途：生成“声波边界图”——检测像素是否处于声波传播边缘，输出对应声源的颜色。
    // 输出结果存入 _SoundBordersTexture，用于后处理中优先显示声波轮廓（高亮边缘）。
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _CameraDepthTexture;

            int _SoundSourceCount;
            #define MAX_SOUND_SOURCE_COUNT 64
            float3 _SoundSourcePositions[MAX_SOUND_SOURCE_COUNT];
            float _SoundSourceVolumes[MAX_SOUND_SOURCE_COUNT];
            float _SoundSourceLifeTimes[MAX_SOUND_SOURCE_COUNT];
            float3 _SoundColors[MAX_SOUND_SOURCE_COUNT];

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 worldPosition : TEXCOORD0;
            };
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            #define MAX_SOUND_LIFE_TIME 4 //warning: need to be same value with "SoundSouce.cs" script
            #define SOUND_SPEED_FACTOR 4
            #define SOUND_BORDER_SCALE 0.1

            fixed4 frag(v2f i) : SV_Target
            {
                float2 screenUV = i.vertex.xy / _ScreenParams.xy;
                float depth = tex2D(_CameraDepthTexture, screenUV);
                
                float3 receivedBorderColor = float3(0, 0, 0);
                for (int id = 0; id < _SoundSourceCount; ++id) {
                    float soundDistance = length(_SoundSourcePositions[id] - i.worldPosition);
                    float soundVolume = _SoundSourceVolumes[id];
                    float soundLifeTime = _SoundSourceLifeTimes[id];
                    float borderDistance = min(soundLifeTime * SOUND_SPEED_FACTOR, soundVolume) - soundDistance;
                    if (borderDistance <= 0)
                        continue;
                    float lineWidth = SOUND_BORDER_SCALE * (1 - soundLifeTime * SOUND_SPEED_FACTOR / soundVolume);
                    if (lineWidth > 0 && borderDistance < lineWidth) //edge of sound wave
                    {
                        float alpha = pow(1.0 - borderDistance / lineWidth, 3.0);
                        receivedBorderColor = max(receivedBorderColor, _SoundColors[id] * alpha);
                    }
                }
                return float4(receivedBorderColor, 1);
            }
            ENDCG
        }
    }
}
