// Difference with Unlit/Transparent is that this is a GLSL implementation hence is faster for mobiles
Shader "Custom/Unlit Transparent GLSL" {

Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

Category {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True"}
	LOD 100
	
	Lighting Off
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 

	SubShader {
		Pass {
        GLSLPROGRAM
        
        varying mediump vec2 texUV;
        
        #ifdef VERTEX
        uniform mediump vec4 _MainTex_ST;
        void main()
        {
			gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			// _MainTex_ST.xy and _MainTex_ST.zw seems to be offset and tiling properties
			texUV = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
        }
        #endif
 
        #ifdef FRAGMENT
        uniform lowp sampler2D _MainTex;
        void main()
        {
			gl_FragColor = texture2D(_MainTex, texUV);
        }
        #endif
       
        ENDGLSL
		}
	}
	
	// Fallback for Editor
	SubShader {
		Pass {
		SetTexture [_MainTex] {} 
		}
	}
}

}
