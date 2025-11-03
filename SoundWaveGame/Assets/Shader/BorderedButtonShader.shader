Shader "Unlit/BorderedButtonShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (1,0,0,1) // 默认红色，确保可见
        _BorderWidth ("Border Width", Range(0, 0.5)) = 0.1 // 增加范围
        [MaterialToggle] _UseAlphaClip ("Use Alpha Clip", Float) = 0
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

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
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
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _BorderColor;
            float _BorderWidth;
            fixed _UseAlphaClip;

            v2f vert(appdata v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                
                // 改进的边框检测
                float2 uv = i.texcoord;
                float border = 0.0;
                
                // 检测四个边缘
                if(uv.x < _BorderWidth) border = 1.0;
                if(uv.x > 1.0 - _BorderWidth) border = 1.0;
                if(uv.y < _BorderWidth) border = 1.0;
                if(uv.y > 1.0 - _BorderWidth) border = 1.0;
                
                // 混合边框颜色和原色
                if(border > 0.5)
                {
                    col.rgb = _BorderColor.rgb;
                    col.a = _BorderColor.a;
                }
                
                // Alpha裁剪（可选）
                if(_UseAlphaClip > 0.5)
                    clip(col.a - 0.001);
                    
                return col;
            }
            ENDCG
        }
    }
}