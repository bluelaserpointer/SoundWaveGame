Shader "Hidden/TransparentDepthOnly_TMP"
{
    Properties
    {
        _MainTex   ("Base", 2D) = "white" {}
        _BumpMap   ("Normal Map", 2D) = "bump" {}

        // —— 透明裁剪控制 —— //
        _UseSDF    ("Use SDF Alpha (TMP)", Float) = 1   // 1=按TMP SDF软边；0=普通alpha裁剪
        _AlphaClip ("Alpha Clip (non-SDF)", Range(0,1)) = 0.5
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    sampler2D _BumpMap;

    float _UseSDF;
    float _AlphaClip;

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
    ENDCG

    // -------- Transparent（覆盖 TMP 常用路径） --------
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            // 关键设置：双面 & 不写深度
            Cull Off
            ZWrite Off
            ZTest LEqual
            // 如需把输出叠加而不是覆盖，自己加 Blend 规则（多数替换渲染输出到专用RT可不设）

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_t

            // Transparent Pass 的片元：对 TMP 做 SDF 裁剪，再输出法线
            float4 frag_t(v2f i) : SV_Target
            {
                // —— 1) SDF/Alpha 裁剪：把字外的像素直接丢弃（不写 RT）——
                // 如果是 TMP SDF 字体：
                float a = tex2D(_MainTex, i.uv).a;   // SDF 存在 alpha
                float w = fwidth(a);                 // 屏幕空间导数，实现软边
                float alpha = smoothstep(0.5 - w, 0.5 + w, a) * i.col.a; // 乘上顶点色的透明度
                clip(alpha - 1e-4);                  // 字外像素不写入 RT

                // —— 2) 计算并写入depth（你的原逻辑）——
                float d01 = Linear01FromSVPos(i.pos);
                return float4(d01, d01, d01, 1);  // A=1 标记“已写”
            }
            ENDCG
        }
    }
}
