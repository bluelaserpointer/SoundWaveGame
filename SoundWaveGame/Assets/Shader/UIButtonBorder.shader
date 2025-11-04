Shader "UI/ButtonBorder"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (1,1,1,1)
        _BorderWidth ("Border Width", Range(0, 0.2)) = 0.05
        _Progress ("Progress", Range(0, 1)) = 0
        
        // 圆环效果属性
        _RingColor ("Ring Color", Color) = (1,1,1,1)
        _RingWidth ("Ring Width", Range(0, 0.1)) = 0.02
        _RingSpeed ("Ring Speed", Float) = 1.0
        
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
                float2 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _BorderColor;
            float _BorderWidth;
            float _Progress;
            
            // 圆环效果变量
            fixed4 _RingColor;
            float _RingWidth;
            float _RingSpeed;
            
            float4 _ClipRect;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                OUT.worldPos = IN.texcoord;
                return OUT;
            }

            // 修正后的圆环效果函数 - 圆环从右上角开始逐渐放大
            float CalculateRingEffect(float2 uv, float progress)
            {
                // 圆心固定在右上角(1,1)
                float2 center = float2(1, 1);
                
                // 计算当前点到圆心的距离
                float dist = length(uv - center);
                
                // 计算按钮对角线长度（从(0,0)到(1,1)）
                float maxRadius = length(float2(1, 1));
                
                // 修正：圆环半径从0逐渐增加到最大半径
                float currentRadius = (1.0 -progress) * _RingSpeed * maxRadius;
                
                // 计算圆环效果 - 确保圆环随着半径增加而显示
                // 当dist接近currentRadius时显示圆环
                float ringInner = smoothstep(currentRadius - _RingWidth, currentRadius, dist);
                float ringOuter = smoothstep(currentRadius, currentRadius + _RingWidth, dist);
                float ringEffect = ringInner * (1.0 - ringOuter);
                
                return ringEffect;
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
                
                // 计算从左下角到当前点的进度
                float2 fromBottomLeft = IN.texcoord;
                float diagonalProgress = (fromBottomLeft.x + fromBottomLeft.y) * 0.5;
                
                // 边框波浪效果
                float waveThreshold = _Progress * 1.5;
                float wave = smoothstep(waveThreshold - 0.4, waveThreshold, diagonalProgress);
                
                // 计算圆环效果
                float ringEffect = CalculateRingEffect(IN.texcoord, _Progress);
                
                // 主纹理颜色
                fixed4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // 边框颜色
                fixed4 borderColor = _BorderColor;
                borderColor.a *= wave * isBorder;
                
                // 圆环颜色
                fixed4 ringColor = _RingColor;
                ringColor.a = ringEffect;
                
                // 混合效果
                color.rgb = lerp(color.rgb, ringColor.rgb, ringColor.a);
                color.rgb = lerp(color.rgb, borderColor.rgb, borderColor.a);
                color.a = max(color.a, max(ringColor.a, borderColor.a));
                
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