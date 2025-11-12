Shader "Hidden/TransparentDepthOnly_TMP"
{
    Properties
    {
        _MainTex   ("Base", 2D) = "white" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "./SoundWaveCommon.cginc"
    #pragma multi_compile __ FRONT_MOST

    sampler2D _MainTex;
    
    int   _UseParticleAlphaClip;
    int   _UseAdditiveBlackKey;

    struct appdata
    {
        float4 vertex  : POSITION;
        float2 uv      : TEXCOORD0;
        float4 color   : COLOR;     // for TMP vertex color (alpha & tint)
    };

    struct v2f
    {
        float4 pos : SV_POSITION;
        float2 uv  : TEXCOORD3;
        float4 col : COLOR;
    };

    v2f vert(appdata v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv  = v.uv;
        o.col = v.color;
        return o;
    }
    // 把 clip 空间的 z/w 还原到“原始深度 0..1”，再统一用 Linear01Depth 线性化
    inline float Linear01FromSVPos(float4 svPos)
    {
        float z_ndc = svPos.z / svPos.w;      // 反Z: [1..0]，正Z: [-1..1]
#if defined(UNITY_REVERSED_Z)
        float raw01 = z_ndc;                   // 反Z时原始深度直接是 [1..0]
#else
        float raw01 = 0.5f * (z_ndc + 1.0f);   // 正Z时把 [-1..1] 映射到 [0..1]
#endif
        return Linear01Depth(raw01);           // 统一得到 线性0..1（近=0，远=1）
    }
    inline float4 frag_common(v2f i) : SV_Target
    {
        // TMP(SDF) / 粒子透明裁剪
        ClipCombinedAlpha(_MainTex, i.uv, i.col.a, _UseParticleAlphaClip, _UseAdditiveBlackKey);
        float d01 = Linear01FromSVPos(i.pos);
        return float4(d01, d01, d01, 1);
    }
    float4 frag_opaque(v2f i) : SV_Target
    {
        return frag_common(i);
    }
    float4 frag_transparent(v2f i) : SV_Target
    {
        return frag_common(i);
    }
    float4 frag_cutout(v2f i) : SV_Target
    {
        return frag_common(i);
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
                return frag_clip(i);
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
                return frag_clip(i);
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
