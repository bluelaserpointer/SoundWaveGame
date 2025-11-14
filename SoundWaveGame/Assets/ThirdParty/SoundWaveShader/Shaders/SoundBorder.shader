Shader "Hidden/SoundBorder"
{
    // 用途：生成“声波边界图”——检测像素是否处于声波传播边缘，输出对应声源的颜色。
    // 输出结果存入 _SoundBordersTexture，用于后处理中优先显示声波轮廓（高亮边缘）。

    Properties
    {
        _MainTex ("Base (for TMP SDF/Cutout)", 2D) = "white" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "./SoundWaveCommon.cginc"
    #include "./SeaDeform.cginc"
    #pragma multi_compile __ IS_SEA

    sampler2D _CameraDepthTexture;

    int _SoundSourceCount;
    #define MAX_SOUND_SOURCE_COUNT 64
    float3 _SoundSourcePositions[MAX_SOUND_SOURCE_COUNT];
    float  _SoundSourceVolumes[MAX_SOUND_SOURCE_COUNT];
    float  _SoundSourceLifeTimes[MAX_SOUND_SOURCE_COUNT];
    float3 _SoundColors[MAX_SOUND_SOURCE_COUNT];
    
    int   _UseParticleAlphaClip;
    int   _UseAdditiveBlackKey;

    sampler2D _MainTex;
    float4 _MainTex_ST;

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv     : TEXCOORD0;
        float4 color  : COLOR;
    };

    struct v2f
    {
        float4 vertex        : SV_POSITION;
        float4 worldPosition : TEXCOORD0;
        float2 uv            : TEXCOORD1;
        float4 color         : COLOR;
    };

    v2f vert (appdata v)
    {
        v2f o;
        o.vertex        = UnityObjectToClipPos(v.vertex);
        float3 worldPos;
        #ifdef IS_SEA
            // 对海面：走公用的 Gerstner 逻辑
            worldPos = Sea_LocalToWorld(v.vertex.xyz, v.uv);
            o.vertex = UnityWorldToClipPos(worldPos);
        #else
            // 其他物体：保持原逻辑
            worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.vertex = UnityObjectToClipPos(v.vertex);
        #endif
        o.worldPosition = float4(worldPos, 1.0);
        o.uv            = TRANSFORM_TEX(v.uv, _MainTex);
        o.color         = v.color;
        return o;
    }

    // —— 声波“边界色”叠加主逻辑，三个通道复用 —— //
    #define MAX_SOUND_LIFE_TIME 4   // 与 SoundSource.cs 保持一致
    #define SOUND_SPEED_FACTOR 4
    #define SOUND_BORDER_SCALE 0.1

    float3 AccumulateBorder(float4 worldPos, float2 svPos)
    {
        float3 receivedBorderColor = 0;

        float2 screenUV = svPos / _ScreenParams.xy;

        [loop]
        for (int id = 0; id < _SoundSourceCount; ++id)
        {
            float soundDistance = length(_SoundSourcePositions[id] - worldPos.xyz);
            float soundVolume   = _SoundSourceVolumes[id];
            float soundLifeTime = _SoundSourceLifeTimes[id];

            float borderDistance = min(soundLifeTime * SOUND_SPEED_FACTOR, soundVolume) - soundDistance;
            if (borderDistance <= 0) continue;

            // 概率抖动，稀疏化
            float prob = (1 - pow(saturate(soundLifeTime / MAX_SOUND_LIFE_TIME), 6.0)) * 0.75;
            float rand = frac(sin(dot(worldPos.xyz + id * 12.345, float3(12.9898,78.233,37.719))) * 43758.5453);
            if (rand > prob) continue;

            float lineWidth = SOUND_BORDER_SCALE * (1 - (soundLifeTime * SOUND_SPEED_FACTOR) / max(soundVolume, 1e-4));
            if (lineWidth > 0 && borderDistance < lineWidth)
            {
                float alpha = 1 - pow(abs(0.5 - borderDistance / lineWidth) * 2, 2.0);
                receivedBorderColor = max(receivedBorderColor, _SoundColors[id] * alpha);
            }
        }
        return receivedBorderColor;
    }
    ENDCG

    // -------- Opaque --------
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Back
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_opaque

            float4 frag_opaque(v2f i) : SV_Target
            {
                float3 col = AccumulateBorder(i.worldPosition, i.vertex.xy);
                return float4(col, 1);
            }
            ENDCG
        }
    }

    // -------- Transparent（TMP 常用，SDF 裁剪字外区域） --------
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite On
            ZTest LEqual
            // 如需叠加到已有颜色，可自行加：Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_transparent

            float4 frag_transparent(v2f i) : SV_Target
            {
                ClipCombinedAlpha(_MainTex, i.uv, i.color.a, _UseParticleAlphaClip, _UseAdditiveBlackKey);

                float3 col = AccumulateBorder(i.worldPosition, i.vertex.xy);
                return float4(col, 1);
            }
            ENDCG
        }
    }

    // -------- TransparentCutout（硬阈值裁剪） --------
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_cutout

            float4 frag_cutout(v2f i) : SV_Target
            {
                float a = tex2D(_MainTex, i.uv).a * i.color.a;
                clip(a - 0.5);                               // 简单硬阈值；如需软边请改用 GetTMPAlpha

                float3 col = AccumulateBorder(i.worldPosition, i.vertex.xy);
                return float4(col, 1);
            }
            ENDCG
        }
    }
}
