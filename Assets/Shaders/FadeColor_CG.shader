Shader "Custom/Fade Color CG" {

Properties {
	_Color ("Color", Color) = (0,0,0,0)
}

SubShader {
	Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" 
			"PreviewType"="Plane" "CanUseSpriteAtlas"="False" }
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha // The generated color is multiplied by the SrcFactor. The color already on screen is multiplied by DstFactor and the two are added together.
	
	Pass {
		Cull Off // here it solves an issue (donno which issue)
		
		CGPROGRAM
		#pragma target 2.0
	    #pragma exclude_renderers ps3 xbox360 flash glesdesktop
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment frag
	    #pragma glsl_no_auto_normalization
	    
	    uniform fixed4 _Color; // if using fixed4 as type then be sure frag() has fixed4 in its signature
	    
	    struct vertexInput
		{
			half4 vertex: POSITION;
		};

		struct fragmentInput
		{
			half4 pos: SV_POSITION;
		};
	    
	    fragmentInput vert(vertexInput i)
		{
			fragmentInput o;
			o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
			return o;
		}
		
		fixed4 frag(fragmentInput i) : COLOR
		{
			fixed4 c = _Color;
			return c;
		}
	    ENDCG
    }
}
}
