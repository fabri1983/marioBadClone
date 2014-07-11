Shader "Custom/Unlit Background" {

Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
	
SubShader {
    Tags { "Queue"="Background" }
    ZWrite Off
	Lighting Off
	
    Pass {
        SetTexture [_MainTex] { combine texture }
    }
}
}
