Shader "Hidden/SoundVolume"
{
    //用途：生成“声波体积图”——在屏幕空间内为每个像素计算当前所有声源的波动强度与颜色叠加。
    // 输出结果存入 _SoundVolumesTexture，用于后处理着色（如音波区域的描边颜色）。
    //改变_ConstantColor以没有声波时显示常驻色
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "./SoundWaveCommon.cginc"

    sampler2D _CameraDepthTexture;

    int _SoundSourceCount;
    #define MAX_SOUND_SOURCE_COUNT 64
    float3 _SoundSourcePositions[MAX_SOUND_SOURCE_COUNT];
    float _SoundSourceVolumes[MAX_SOUND_SOURCE_COUNT];
    float _SoundSourceLifeTimes[MAX_SOUND_SOURCE_COUNT];
    float3 _SoundColors[MAX_SOUND_SOURCE_COUNT];

    float3 _ConstantColor;
    int _SampleTextureColorAsConstantColor;
	int _IgnoreOutlineClip;
    
    int   _UseParticleAlphaClip;
    int   _UseAdditiveBlackKey;

    sampler2D _MainTex;
    float4 _MainTex_ST;

    struct appdata {
        float4 vertex : POSITION;
        float2 uv     : TEXCOORD0;
        float4 color  : COLOR;
    };

    struct v2f {
        float4 vertex        : SV_POSITION;
        float4 color         : COLOR;
        float4 worldPosition : TEXCOORD0;
        float2 uv            : TEXCOORD1;
        float4 screenPos     : TEXCOORD2;
    };

    v2f vert (appdata v){
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.color = v.color;
        o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
        o.uv  = v.uv;
        o.screenPos = ComputeScreenPos(o.vertex);
        return o;
    }

    // —— 声波叠加主逻辑（保持你的代码） —— //
    float4 AccumulateSound(float3 baseCol, float4 worldPos, float2 svPos)
    {
        float3 receivedVolumeColor = baseCol;

        float2 screenUV = svPos / _ScreenParams.xy;
        float depth = tex2D(_CameraDepthTexture, screenUV).r;

        [loop]
        for (int id = 0; id < _SoundSourceCount; ++id) {
            float soundDistance = length(_SoundSourcePositions[id] - worldPos.xyz);
            float soundVolume   = _SoundSourceVolumes[id];
            float soundLifeTime = _SoundSourceLifeTimes[id];
            float borderDistance = min(soundVolume, soundLifeTime * 4) - soundDistance;
            if (borderDistance <= 0) continue;
            float prob = (1 - pow(saturate(soundDistance / soundVolume), 6.0)) * 0.25;
            float rand = frac(sin(dot(worldPos.xyz + id * 12.345, float2(12.9898,78.233))) * 43758.5453);
            if (rand > prob) continue;

            float falloff = 1 - pow(1 - saturate(borderDistance / soundVolume), 2.0);
            float t = clamp(soundLifeTime / 4.0, 0.0, 1.0);
            float alpha = 5 * falloff * (1.0 - t * t);
            receivedVolumeColor = max(receivedVolumeColor, _SoundColors[id] * alpha);
        }
        return float4(receivedVolumeColor, _IgnoreOutlineClip == 1 ? 1 : 0);
    }

    // —— 基础色，供两个通道复用 —— //
    float3 GetBaseColor(float2 uv, float4 vtxColor)
    {
        if (_SampleTextureColorAsConstantColor == 1)
            return tex2D(_MainTex, uv).rgb * vtxColor.rgb; // 贴图 * 顶点色（LineRenderer/TMP）
        else
            return _ConstantColor;
    }

    ENDCG

    // -------- Opaque --------
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
            // 按需：ZWrite On / Cull Back
            Cull Back
            ZWrite On
            ZTest LEqual
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_opaque

            float4 frag_opaque(v2f i) : SV_Target
            {
                float3 baseCol = GetBaseColor(i.uv, i.color);
                float4 outCol  = AccumulateSound(baseCol, i.worldPosition, i.vertex.xy);
                return outCol;
            }
            ENDCG
        }
    }

    // -------- Transparent（TMP 常用） --------
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Pass
        {
            // 关键设置：
            Cull Off          // 双面渲染，解决“只有一面可见”
            ZWrite On        // 透明物体通常不写深度，但是本游戏的渲染不存在"半透明"，所以还是写深度
            ZTest LEqual
            // 不需要混合（我们写到独立的 RT）；若你想叠加，可自行设 Blend

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_transparent

            float4 frag_transparent(v2f i) : SV_Target
            {
                // 1) 选择 alpha 来源：TMP(SDF) 或 粒子(贴图A)， 丢弃透明区（把 PNG 的透明边缘/空白剔除）
                 ClipCombinedAlpha(_MainTex, i.uv, i.color.a, _UseParticleAlphaClip, _UseAdditiveBlackKey);

                // 2) 计算颜色并叠加声波
                float3 baseCol = GetBaseColor(i.uv, i.color);
                float4 outCol  = AccumulateSound(baseCol, i.worldPosition, i.vertex.xy);
                return outCol;
            }
            ENDCG
        }
    }

    // -------- TransparentCutout（可选） --------
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
                // 简化：硬阈值（如需软边，直接调用 GetTMPAlpha 并 smoothstep）
                float a = tex2D(_MainTex, i.uv).a * i.color.a;
                clip(a - 0.5);

                float3 baseCol = GetBaseColor(i.uv, i.color);
                float4 outCol  = AccumulateSound(baseCol, i.worldPosition, i.vertex.xy);
                return outCol;
            }
            ENDCG
        }
    }
}
