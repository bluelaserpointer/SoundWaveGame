// SeaDeform.cginc
// 公用的海面几何形变（Gerstner Wave）实现
// 需要在使用它的 Shader 中保证有对应的 Properties 和 uniform 声明。

#ifndef SEA_DEFORM_INCLUDED
#define SEA_DEFORM_INCLUDED

#include "UnityCG.cginc"

//======================
// 公共参数（由材质提供）
//======================

static const float  SEA_HeightOffset  = 0.0;

static const float  SEA_WaveAmp1      = 0.15;
static const float  SEA_WaveAmp2      = 0.075;

static const float  SEA_WaveLength1   = 8.0;
static const float  SEA_WaveLength2   = 3.5;

static const float  SEA_WaveSpeed1    = 0.85;
static const float  SEA_WaveSpeed2    = 1.5;

static const float2 SEA_WaveDir1      = float2(1.0, 0.2);
static const float2 SEA_WaveDir2      = float2(-0.4, 1.0);

static const float  SEA_Choppy        = 0.1;
static const float  SEA_UVScale       = 40.0;

//======================
// 内部帮助函数
//======================
inline float2 Sea_SafeNormalize(float2 dir)
{
    float len = length(dir);
    if (len < 1e-5)
    {
        // 给一个默认方向，避免 0 向量导致 NaN
        return float2(1.0, 0.0);
    }
    return dir / len;
}
// 计算一组 Gerstner 波的位移
inline float3 Sea_GerstnerDisplacement(
    float2 posXZ,   // 使用的平面坐标（可以是 local xz 或 UV 放大后的值）
    float2 dir,     // 波方向（会归一化）
    float amplitude,
    float wavelength,
    float speed,
    float choppy,
    float time)
{
    // 防止除 0
    wavelength = max(wavelength, 0.0001);

    float2 D = Sea_SafeNormalize(dir);
    float k  = 2.0 * UNITY_PI / wavelength;   // 波数
    float w  = sqrt(9.8 * k);                 // 简单重力波 dispersion，可换成你自己的
    float phase = dot(D, posXZ) * k + (w * speed) * time;

    float sinP = sin(phase);
    float cosP = cos(phase);

    // Gerstner 波：上下位移 + 水平偏移
    float3 disp;
    // 水平（x,z）偏移：产生“尖浪”效果
    disp.xz = choppy * amplitude * cosP * D;
    // 竖直位移
    disp.y  = amplitude * sinP;

    return disp;
}

//======================
// 对外接口函数（你在 vert 里用的）
//======================

// 在物体坐标系下对顶点做形变
inline float3 Sea_ApplyDeformLocal(float3 localPos, float2 uv)
{
    // 先算世界坐标
    float3 worldPos = mul(unity_ObjectToWorld, float4(localPos, 1.0)).xyz;

    // 用世界坐标的 xz 作为波的输入，乘一个缩放控制频率
    float2 seaPos = worldPos.xz;

    float t = _Time.y;

    float3 disp = 0;

    disp += Sea_GerstnerDisplacement(
        seaPos,
        SEA_WaveDir1,
        SEA_WaveAmp1,
        SEA_WaveLength1,
        SEA_WaveSpeed1,
        SEA_Choppy,
        t
    );

    // 如需第二组波再加一组
    disp += Sea_GerstnerDisplacement(
        seaPos,
        SEA_WaveDir2,
        SEA_WaveAmp2,
        SEA_WaveLength2,
        SEA_WaveSpeed2,
        SEA_Choppy,
        t
    );

    float3 result = localPos;
    result += disp;
    result.y += SEA_HeightOffset;

    return result;
}

// 直接从 local 坐标 + uv 得到世界坐标（大多数情况你只需要这个）
inline float3 Sea_LocalToWorld(float3 localPos, float2 uv)
{
    float3 deformedLocal = Sea_ApplyDeformLocal(localPos, uv);
    float4 w = mul(unity_ObjectToWorld, float4(deformedLocal, 1.0));
    return w.xyz;
}

#endif // SEA_DEFORM_INCLUDED
