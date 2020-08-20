Shader "Custom RP/Lit" {
	
	Properties {
		_BaseMap("Texture", 2D) = "white" {}
		_BaseColor("Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		[Toggle(_CLIPPING)] _Clipping ("Alpha Clipping", Float) = 0

		//[Enum(UnityEngine.Rendering.BlendMode)]可以让属性有一个下拉框，内容是各种混合模式的枚举
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
		[Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
	}
	
	SubShader {
		Pass {
			Tags{
				"LightMode" = "CustomLit" //自定义的照明模式
			}
			
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

			HLSLPROGRAM
			#pragma shader_feature _CLIPPING
			//启用GPU实例化，本质是将具有相同网格物体的各自材质放到一个数组里送到gpu，然后遍历它，按顺序渲染
			#pragma multi_compile_instancing 
			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment
			#include "LitPass.hlsl"
			ENDHLSL
		}
	}
}