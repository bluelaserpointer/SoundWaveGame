Shader "Hidden/Roystan/Outline Post Process"
{
	//从_SoundBordersTexture、_SoundVolumesTexture裁剪所需要的区域。
	//1._SoundBordersTexture的所有颜色都会被保留。
	//2._SoundVolumesTexture的颜色只有在轮廓处才会被保留。
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
			// Custom post processing effects are written in HLSL blocks,
			// with lots of macros to aid with platform differences.
			// https://github.com/Unity-Technologies/PostProcessing/wiki/Writing-Custom-Effects#shader
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			// _CameraNormalsTexture contains the view space normals transformed
			// to be in the 0...1 range.
			sampler2D _CameraNormalsTexture;
			sampler2D _CameraDepthTexture;
			sampler2D _SoundBordersTexture;
			sampler2D _SoundVolumesTexture;

			int _Debug_ShowTextureID = 0; //0: default, 1:normal, 2:depth, 3:soundWave
        
			// Data pertaining to _MainTex's dimensions.
			// https://docs.unity3d.com/Manual/SL-PropertiesInPrograms.html
			float4 _MainTex_TexelSize;

			float _Scale;
			float4 _BackgroundColor;

			float _DepthThreshold;
			float _DepthNormalThreshold;
			float _DepthNormalThresholdScale;

			float _NormalThreshold;

			// This matrix is populated in PostProcessOutline.cs.
			float4x4 _ClipToView;

			// Combines the top and bottom colors using normal blending.
			// https://en.wikipedia.org/wiki/Blend_modes#Normal_blend_mode
			// This performs the same operation as Blend SrcAlpha OneMinusSrcAlpha.
			float4 alphaBlend(float4 top, float4 bottom)
			{
				float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
				float alpha = top.a + bottom.a * (1 - top.a);

				return float4(color, alpha);
			}

			float LinEye(float d) { return LinearEyeDepth(d); }              // 或 LinearEyeDepth(d, _ZBufferParams)

			// Both the Varyings struct and the Vert shader are copied
			// from StdLib.hlsl included above, with some modifications.
			struct Varyings
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float3 viewSpaceDir : TEXCOORD1;
			};

			Varyings Vert(appdata_img v)
			{
				Varyings o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				// Transform our point first from clip to view space,
				// taking the xyz to interpret it as a direction.
				o.viewSpaceDir = mul(_ClipToView, o.vertex).xyz;
				return o;
			}

			float4 Frag(Varyings i) : SV_Target
			{
				//debug mode
				switch (_Debug_ShowTextureID) {
				case 1:
					return tex2D(_CameraNormalsTexture, i.texcoord);
				case 2:
					return tex2D(_CameraDepthTexture, i.texcoord);
				case 3:
					return tex2D(_SoundBordersTexture, i.texcoord);
				case 4:
					return tex2D(_SoundVolumesTexture, i.texcoord);
				}
				float lin01 = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.texcoord));
				// 根据深度调节采样半径：远处放大
				float adaptiveScale = _Scale * lerp(1.0, 25.0, lin01);
				//test if this fragment is on an outline
				float halfScaleFloor = floor(adaptiveScale * 0.5);
				float halfScaleCeil = ceil(adaptiveScale * 0.5);

				// Sample the pixels in an X shape, roughly centered around i.texcoord.
				// As the _CameraDepthTexture and _CameraNormalsTexture default samplers
				// use point filtering, we use the above variables to ensure we offset
				// exactly one pixel at a time.
				float2 bottomLeftUV = i.texcoord - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
				float2 topRightUV = i.texcoord + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;  
				float2 bottomRightUV = i.texcoord + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
				float2 topLeftUV = i.texcoord + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);

				float3 normal0 = tex2D(_CameraNormalsTexture, bottomLeftUV).rgb;
				float3 normal1 = tex2D(_CameraNormalsTexture, topRightUV).rgb;
				float3 normal2 = tex2D(_CameraNormalsTexture, bottomRightUV).rgb;
				float3 normal3 = tex2D(_CameraNormalsTexture, topLeftUV).rgb;

				float depth0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, bottomLeftUV);
				float depth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, topRightUV);
				float depth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, bottomRightUV);
				float depth3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, topLeftUV);

				// Transform the view normal from the 0...1 range to the -1...1 range.
				float3 viewNormal = normal0 * 2 - 1;
				float NdotV = 1 - dot(viewNormal, -i.viewSpaceDir);

				// Return a value in the 0...1 range depending on where NdotV lies 
				// between _DepthNormalThreshold and 1.
				float normalThreshold01 = saturate((NdotV - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
				// Scale the threshold, and add 1 so that it is in the range of 1..._NormalThresholdScale + 1.
				float normalThreshold = normalThreshold01 * _DepthNormalThresholdScale + 1;

				// Modulate the threshold by the existing depth value;
				// pixels further from the screen will require smaller differences
				// to draw an edge.

				float depthThreshold = _DepthThreshold * depth0 * normalThreshold;

				float depthFiniteDifference0 = depth1 - depth0;
				float depthFiniteDifference1 = depth3 - depth2;
				// edgeDepth is calculated using the Roberts cross operator.
				// The same operation is applied to the normal below.
				// https://en.wikipedia.org/wiki/Roberts_cross
				float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 1;

				float3 normalFiniteDifference0 = normal1 - normal0;
				float3 normalFiniteDifference1 = normal3 - normal2;
				// Dot the finite differences with themselves to transform the 
				// three-dimensional values to scalars.
				float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
				float3 outlineClipColor;
				//not outline returns backgroud color
				if (edgeDepth <= depthThreshold && edgeNormal <= _NormalThreshold)
					outlineClipColor = _BackgroundColor;
				else
					outlineClipColor = tex2D(_SoundVolumesTexture, i.texcoord);
				//draw border line
				float3 borderColor = tex2D(_SoundBordersTexture, i.texcoord);
				//outline returns sound volume color
				return float4(max(borderColor, outlineClipColor), 1);
			}
            ENDCG
        }
    }
}