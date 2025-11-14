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
    #include "./SeaDeform.cginc"
    #pragma multi_compile __ FRONT_MOST
    #pragma multi_compile __ IS_SEA

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

    int _UseParticleAlphaClip;
    int _UseAdditiveBlackKey;

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
        o.color = v.color;
        o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
        o.screenPos = ComputeScreenPos(o.vertex);
        return o;
    }

    // —— 声波叠加主逻辑（保持你的算法） —— //
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

        // 不再依赖 _ZWrite/_ZTest 的调试分支
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

    // 片元实现

    float4 frag_transparent(v2f i) : SV_Target
    {
        // TMP(SDF) / 粒子透明裁剪
        ClipCombinedAlpha(_MainTex, i.uv, i.color.a, _UseParticleAlphaClip, _UseAdditiveBlackKey);

        float3 baseCol = GetBaseColor(i.uv, i.color);
        return AccumulateSound(baseCol, i.worldPosition, i.vertex.xy);
    }

    float4 frag_cutout(v2f i) : SV_Target
    {
        // 硬阈值裁剪（如需软边，可改 smoothstep）
        float a = tex2D(_MainTex, i.uv).a * i.color.a;
        clip(a - 0.5);

        float3 baseCol = GetBaseColor(i.uv, i.color);
        return AccumulateSound(baseCol, i.worldPosition, i.vertex.xy);
    }
    
    float4 frag_opaque(v2f i) : SV_Target
    {
        float3 baseCol = GetBaseColor(i.uv, i.color);
        return AccumulateSound(baseCol, i.worldPosition, i.vertex.xy);
    }

    float4 frag_clip(v2f i) : SV_Target
    {
        clip(-1);
        return float4(0, 0, 0, 0);
    }
    ENDCG
    
    // -------- Opaque --------
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
            Name "Normal"
            Cull Back
            ZWrite On
            ZTest LEqual
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment Frag_Normal
            float4 Frag_Normal(v2f i) : SV_Target
            {
            #ifdef FRONT_MOST
                return frag_clip(i);
            #else
                return frag_opaque(i);
            #endif
            }
            ENDCG
        }
        Pass
        {
            Name "FrontMost"
            Cull Back
            ZWrite Off
            ZTest Always
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment Frag_Front
            float4 Frag_Front(v2f i) : SV_Target
            {
            #ifdef FRONT_MOST
                return frag_opaque(i);
            #else
                return frag_clip(i);
            #endif
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
            Name "Normal"
            Cull Off
            ZWrite On
            ZTest LEqual
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment Frag_Front
            float4 Frag_Front(v2f i) : SV_Target
            {
            #ifdef FRONT_MOST
                return frag_clip(i);
            #else
                return frag_transparent(i);
            #endif
            }
            ENDCG
        }
        Pass
        {
            Name "FrontMost"
            Cull Off
            ZWrite Off
            ZTest Always
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment Frag_Front
            float4 Frag_Front(v2f i) : SV_Target
            {
            #ifdef FRONT_MOST
                return frag_transparent(i);
            #else
                return frag_clip(i);
            #endif
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
            Name "Normal"
            Cull Off
            ZWrite On
            ZTest LEqual
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment Frag_Front
            float4 Frag_Front(v2f i) : SV_Target
            {
            #ifdef FRONT_MOST
                return frag_clip(i);
            #else
                return frag_cutout(i);
            #endif
            }
            ENDCG
        }
        Pass
        {
            Name "FrontMost"
            Cull Off
            ZWrite Off
            ZTest Always
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment Frag_Front
            float4 Frag_Front(v2f i) : SV_Target
            {
            #ifdef FRONT_MOST
                return frag_cutout(i);
            #else
                return frag_clip(i);
            #endif
            }
            ENDCG
        }
    }
}
