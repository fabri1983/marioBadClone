Shader "Custom/Unlit Background" {

Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
	
SubShader {
    Tags { "IgnoreProjector"="True" "Queue"="Background" }
    ZWrite Off
	Lighting Off
	
    Pass {
    	Cull Off // here it solves an issue (dunno which issue)
    	
        SetTexture [_MainTex] { combine texture }
    }
}
}
