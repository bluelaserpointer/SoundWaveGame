#ifndef SOUND_WAVE_COMMON_INCLUDED
#define SOUND_WAVE_COMMON_INCLUDED

#include "UnityCG.cginc"

// —— 通用：TMP 的 SDF alpha ——
// 把纹理和顶点色当作参数传入，避免强依赖全局名
inline float GetTMPAlpha(sampler2D sdfTex, float2 uv, float vtxAlpha)
{
    float sdf   = tex2D(sdfTex, uv).a;
    float w     = fwidth(sdf);
    float alpha = smoothstep(0.5 - w, 0.5 + w, sdf);
    return alpha * vtxAlpha;
}

// —— 通用：粒子用贴图 A（真的有 Alpha 时）——
inline float GetParticleAlpha(sampler2D tex, float2 uv, float vtxAlpha)
{
    return tex2D(tex, uv).a * vtxAlpha;
}

// —— 通用：亮度转 Alpha（针对加法黑底贴图）——
inline float GetParticleAlphaBlackKey(sampler2D tex, float2 uv, float vtxAlpha,
                                         float blackClip, float feather)
{
    float3 c  = tex2D(tex, uv).rgb;
    float lum = dot(c, float3(0.2126, 0.7152, 0.0722));
    float a   = smoothstep(blackClip, blackClip + feather, lum);
    return a * vtxAlpha;
}

inline void ClipCombinedAlpha(
    sampler2D tex, float2 uv, float vtxAlpha,
    int useParticleAlphaClip,
    int useAdditiveBlackKey)
{
    // TMP SDF alpha
    float alphaTMP = GetTMPAlpha(tex, uv, vtxAlpha);

    // 粒子 alpha：根据是否为加法黑底或带真 alpha
    float alphaPS = (useAdditiveBlackKey == 1)
        ? GetParticleAlphaBlackKey(tex, uv, vtxAlpha, 0.02, 0.03)
        : GetParticleAlpha(tex, uv, vtxAlpha);

    // 使用哪个 alpha 来源
    float alpha = (useParticleAlphaClip == 1) ? alphaPS : alphaTMP;

    // 直接在函数内部执行 clip，剔除透明像素
    clip(alpha - 1e-4);
}

#endif // SOUND_WAVE_COMMON_INCLUDED
