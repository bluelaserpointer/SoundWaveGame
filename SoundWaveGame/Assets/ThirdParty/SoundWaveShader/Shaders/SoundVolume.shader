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

            // --- 玩家基色控制 ---
            float _IsPlayer;                // 每个Renderer通过 MPB 设置: 1=玩家, 0=非玩家
            float _PlayerBaseRipple;        // 玩家轮廓无声音时描边颜色

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
                float3 receivedVolumeColor = (_IsPlayer > 0.5) ? float3(_PlayerBaseRipple, _PlayerBaseRipple, _PlayerBaseRipple) : 0;
                float2 screenUV = i.vertex.xy / _ScreenParams.xy;
                float depth = tex2D(_CameraDepthTexture, screenUV);
                for (int id = 0; id < _SoundSourceCount; ++id) {
                    float soundDistance = length(_SoundSourcePositions[id] - i.worldPosition);
                    float soundVolume = _SoundSourceVolumes[id];
                    float soundLifeTime = _SoundSourceLifeTimes[id];
                    float borderDistance = min(soundLifeTime * SOUND_SPEED_FACTOR, soundVolume) - soundDistance;
                    if (borderDistance <= 0)
                        continue;
                    float alpha = soundVolume * borderDistance * (1 - soundLifeTime / MAX_SOUND_LIFE_TIME) * 0.05;
                    receivedVolumeColor = _SoundColors[id] * alpha + receivedVolumeColor * (1 - alpha);
                }
                return float4(receivedVolumeColor, 1);
            }
            ENDCG
        }
    }
}
