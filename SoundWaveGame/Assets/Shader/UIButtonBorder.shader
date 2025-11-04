Shader "UI/ButtonBorder"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (1,1,1,1)
        _BorderWidth ("Border Width", Range(0, 0.2)) = 0.05
        _Progress ("Progress", Range(0, 1)) = 0
        _WaveSpeed ("Wave Speed", Float) = 2.0
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord  : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                float4 color    : COLOR;
                float2 localPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _BorderColor;
            float _BorderWidth;
            float _Progress;
            float _WaveSpeed;
            float4 _ClipRect;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                OUT.localPos = IN.texcoord - float2(0.5, 0.5); // 中心点为(0,0)
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 计算到四个边的距离
                float distToLeft = IN.texcoord.x;
                float distToRight = 1.0 - IN.texcoord.x;
                float distToBottom = IN.texcoord.y;
                float distToTop = 1.0 - IN.texcoord.y;
                
                // 找到到最近边的距离
                float minDist = min(min(distToLeft, distToRight), min(distToBottom, distToTop));
                
                // 判断是否在边框范围内
                float isBorder = step(minDist, _BorderWidth);
                
                // 计算从左下角到右上角的对角线进度
                // 左下角(0,0)值为0，右上角(1,1)值为1
                float diagonalValue = (IN.texcoord.x + IN.texcoord.y) * 0.5;
                
                // 关键修改：使用Progress来控制波浪的传播
                // 当Progress=0时，wave在大部分区域为0（透明）
                // 当Progress增加时，wave从左下角开始逐渐变为1（白色）
                float waveThreshold = _Progress * 1.5; // 调整系数来控制传播速度
                float wave = smoothstep(waveThreshold - 0.4, waveThreshold, diagonalValue);
                
                // 主纹理颜色
                fixed4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // 边框颜色（带透明度动画）
                fixed4 borderColor = _BorderColor;
                borderColor.a *= wave * isBorder;
                
                // 混合边框和主纹理
                color.rgb = lerp(color.rgb, borderColor.rgb, borderColor.a);
                color.a = max(color.a, borderColor.a);
                
                color.a *= UnityGet2DClipping(IN.texcoord, _ClipRect);
                
                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}