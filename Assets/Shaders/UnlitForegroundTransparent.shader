Shader "Custom/Unlit Foreground Transparent" {

Properties {
	_MainTex ("Base (RGB) A(alpha)", 2D) = "white" {}
}
	
SubShader {
    Tags { "IgnoreProjector"="True" "Queue"="Overlay" "RenderType"="Transparent" }
    Cull Off
    ZWrite Off
	Lighting Off
	Blend SrcAlpha OneMinusSrcAlpha // The generated color is multiplied by the SrcFactor. The color already on screen is multiplied by DstFactor and the two are added together.
	
    Pass {
        SetTexture [_MainTex] { combine texture }
    }
}
}
