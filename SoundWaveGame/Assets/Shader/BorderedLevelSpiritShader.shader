Shader "Unlit/BorderedLevelSpiritShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (1,0,0,1)
        _BorderWidth ("Border Width", Range(0, 0.3)) = 0.05
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
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _BorderColor;
            float _BorderWidth;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 获取原始纹理颜色
                fixed4 originalColor = tex2D(_MainTex, i.texcoord) * i.color;
                
                // 检查是否在边框区域
                float2 uv = i.texcoord;
                float border = 0.0;
                
                if(uv.x < _BorderWidth || uv.x > 1.0 - _BorderWidth ||
                   uv.y < _BorderWidth || uv.y > 1.0 - _BorderWidth)
                {
                    border = 1.0;
                }
                
                // 混合原始颜色和边框颜色
                fixed4 finalColor;
                if(border > 0.5)
                {
                    // 在边框区域使用边框颜色，但保留原始Alpha
                    finalColor = _BorderColor;
                    finalColor.a *= originalColor.a; // 保持原始透明度
                }
                else
                {
                    // 在内部区域使用原始颜色
                    finalColor = originalColor;
                }
                
                clip(finalColor.a - 0.001);
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}