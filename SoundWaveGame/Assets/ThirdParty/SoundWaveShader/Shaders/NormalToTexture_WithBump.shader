Shader "Hidden/NormalToTexture_WithBump"
{
    Properties
    {
        _MainTex   ("Base", 2D) = "white" {}
        _BumpMap   ("Normal Map", 2D) = "bump" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "./SoundWaveCommon.cginc"
    #include "./SeaDeform.cginc"
    #pragma multi_compile __ FRONT_MOST
    #pragma multi_compile __ IS_SEA

    sampler2D _MainTex;
    sampler2D _BumpMap;
    
    int   _UseParticleAlphaClip;
    int   _UseAdditiveBlackKey;

    struct appdata
    {
        float4 vertex  : POSITION;
        float3 normal  : NORMAL;
        float4 tangent : TANGENT;
        float2 uv      : TEXCOORD0;
        float4 color   : COLOR;     // for TMP vertex color (alpha & tint)
    };

    struct v2f
    {
        float4 pos : SV_POSITION;
        float3 tWS : TEXCOORD0;
        float3 bWS : TEXCOORD1;
        float3 nWS : TEXCOORD2;
        float2 uv  : TEXCOORD3;
        float3 wpos : TEXCOORD4;
        float4 col : COLOR;
    };

    v2f vert(appdata v)
    {
        v2f o;

        float3 nWS = UnityObjectToWorldNormal(v.normal);
        float3 tWS = UnityObjectToWorldDir(v.tangent.xyz);
        tWS = normalize(tWS);
        nWS = normalize(nWS);
        float3 bWS = normalize(cross(nWS, tWS) * v.tangent.w);

        o.tWS = tWS;
        o.bWS = bWS;
        o.nWS = nWS;
        o.uv  = v.uv;

        float3 worldPos;
        #ifdef IS_SEA
            // 对海面：走公用的 Gerstner 逻辑
            worldPos = Sea_LocalToWorld(v.vertex.xyz, v.uv);
            o.pos = UnityWorldToClipPos(worldPos);
        #else
            // 其他物体：保持原逻辑
            worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.pos = UnityObjectToClipPos(v.vertex);
        #endif
        o.wpos = float4(worldPos, 1.0);
        o.col = v.color;
        return o;
    }

    // ——— 输出视空间法线到 RT（0..1）——— //
    float4 OutputVSNormal(float3 nWS)
    {
        float3 nVS = mul((float3x3)UNITY_MATRIX_V, nWS);
        nVS = normalize(nVS);
        return float4(nVS * 0.5 + 0.5, 0);  // 和你原逻辑一致：RGB=法线映射，A=0
    }

    // ——— 主片元：法线贴图 → WS → VS —— //
    float4 frag_opaque(v2f i) : SV_Target
    {
        // Tangent-space normal
        float3 nTS = UnpackNormal(tex2D(_BumpMap, i.uv)); // default "bump" 给 neutral 正常
        float3x3 TBN = float3x3(i.tWS, i.bWS, i.nWS);
        float3 nWS = normalize(mul(TBN, nTS));
        return OutputVSNormal(nWS);
    }
    // 片元参数里加：UNITY_VFACE_TYPE faceSign : SV_IsFrontFace
    // Unity 提供 UNITY_VFACE 宏来兼容性声明，下方写法在 HLSL 下可用：
    float4 frag_transparent(v2f i) : SV_Target
    {
        // —— 1) SDF/Alpha 裁剪——
        ClipCombinedAlpha(_MainTex, i.uv, i.col.a, _UseParticleAlphaClip, _UseAdditiveBlackKey);

        // —— 2) TBN 法线——
        float3 nTS = UnpackNormal(tex2D(_BumpMap, i.uv));
        float3x3 TBN = float3x3(i.tWS, i.bWS, i.nWS);
        float3 nWS = normalize(mul(TBN, nTS));

        float3 viewDirWS = normalize(_WorldSpaceCameraPos - i.wpos);
        if (dot(nWS, viewDirWS) < 0) nWS = -nWS;   // 朝向摄像机

        return OutputVSNormal(nWS);
    }
    float4 frag_cutout(v2f i) : SV_Target
    {
        ClipCombinedAlpha(_MainTex, i.uv, 1, _UseParticleAlphaClip, _UseAdditiveBlackKey);
        return frag_opaque(i);
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
