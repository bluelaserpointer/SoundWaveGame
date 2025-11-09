Shader "Hidden/SoundVolume"
{
    //用途：生成“声波体积图”——在屏幕空间内为每个像素计算当前所有声源的波动强度与颜色叠加。
    // 输出结果存入 _SoundVolumesTexture，用于后处理着色（如音波区域的描边颜色）。
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

            float3 _ConstantColor; // base color displayed when no sound waves are lighting it 

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
                float3 receivedVolumeColor = _ConstantColor;
                float2 screenUV = i.vertex.xy / _ScreenParams.xy;
                float depth = tex2D(_CameraDepthTexture, screenUV);
                for (int id = 0; id < _SoundSourceCount; ++id) {
                    float soundDistance = length(_SoundSourcePositions[id] - i.worldPosition);
                    float soundVolume = _SoundSourceVolumes[id];
                    float soundLifeTime = _SoundSourceLifeTimes[id];
                    float borderDistance = min(soundVolume, soundLifeTime * SOUND_SPEED_FACTOR) - soundDistance;
                    if (borderDistance <= 0)
                        continue;
                    float falloff = 1 - pow(1 - saturate(borderDistance / soundVolume), 2.0);
                    float t = clamp(soundLifeTime / MAX_SOUND_LIFE_TIME, 0.0, 1.0);
                    float alpha = 5 * falloff * (1.0 - pow(t, 2.0));
                    receivedVolumeColor = max(receivedVolumeColor, _SoundColors[id] * alpha);
                }
                return float4(receivedVolumeColor, 1);
            }
            ENDCG
        }
    }
}
