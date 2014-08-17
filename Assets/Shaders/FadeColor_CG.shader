Shader "Custom/Fade Color CG" {

Properties {
	_Color ("Color", Color) = (0,0,0,0)
}

SubShader {
	Tags { "Queue"="Overlay+10" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Cull Off
	ZWrite Off
	Lighting Off
	Blend SrcAlpha OneMinusSrcAlpha // The generated color is multiplied by the SrcFactor. The color already on screen is multiplied by DstFactor and the two are added together.
	
	Pass {
		CGPROGRAM
	    #pragma exclude_renderers ps3 xbox360 flash
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment frag
	    
	    uniform fixed4 _Color;
	    
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
		
		half4 frag(fragmentInput i) : COLOR
		{
			half4 c = _Color;
			return c;
		}
	    ENDCG
    }
}
}
