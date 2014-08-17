Shader "Custom/Unlit Background" {

Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
	
SubShader {
    Tags { "IgnoreProjector"="True" "Queue"="Background" }
    Cull Off
    ZWrite Off
	Lighting Off
	
    Pass {
        SetTexture [_MainTex] { combine texture }
    }
}
}
