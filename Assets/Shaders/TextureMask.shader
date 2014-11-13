// This simple shader will mask off areas of a texture. 
// This is useful for, say, when you really want to make your minimap round. 
// I've designed it with GUI elements in mind. To make a round object you would:
// 
//  - Put a black border around a white circle and import that into unity.
//  - Change the texture format to Alpha 8 bit and check "Build Alpha from Grayscale"
//  - Create a shader with the code below.
//  - Create a new plane and import your texture on to it.
//  - Select the shader you created.
//  - Drop your culling mask in to it. 
//
// Optimization for Mobile: avoid the use of alpha cut off (Alpha Test)

Shader "Custom/TextureMask" {

Properties
{
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_MaskTex ("Culling Mask", 2D) = "white" {}
	//_Cutoff ("Alpha cutoff", Range (0,1)) = 0.1
}

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
	Lighting Off
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	//AlphaTest GEqual [_Cutoff]

	Pass {
		Cull Off // here it solves an issue (dunno which issue)
		
		SetTexture [_MaskTex] {combine texture}
		SetTexture [_MainTex] {combine texture, previous}
	}
}
}
