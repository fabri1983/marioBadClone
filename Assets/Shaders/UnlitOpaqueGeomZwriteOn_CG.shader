// NOTE: this shader intended for 3D geometry. For 2D geometry use another with zwrite off and ull off
Shader "Custom/Unlit Opaque Geom Z Write On CG" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" }
	ZWrite On // is On because some scene game objects are in 3D, so avoiding overlapping with other materials which depends on RenderQueue
	Lighting Off
	Blend Off // The generated color is multiplied by the SrcFactor. The color already on screen is multiplied by DstFactor and the two are added together.
	
	Pass {
		Cull Back
		
		CGPROGRAM
	    #pragma exclude_renderers ps3 xbox360 flash glesdesktop opengl
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment frag
	    #pragma glsl_no_auto_normalization
	    
	    uniform sampler2D _MainTex;
	    uniform fixed4 _MainTex_ST; // The texture name + _ST is needed to get the tiling & offset. ST: Scale+Transform
	    
	    struct vertexInput
		{
			half4 vertex: POSITION;
			fixed2 texcoord: TEXCOORD0;
		};

		struct fragmentInput
		{
			half4 pos: SV_POSITION;
			fixed2 uv: TEXCOORD0;
		};
	    
	    fragmentInput vert(vertexInput i)
		{
			fragmentInput o;
			o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
			// Does exactly the same as the next line of code: o.uv = TRANSFORM_TEX(i.texcoord, _MainTex) (need to include UnityCG.cginc)
			// _MainTex_ST.xy is tiling and _MainTex_ST.zw is offset
			o.uv = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			return o;
		}
		
		half4 frag(fragmentInput i) : COLOR
		{
			half4 c = tex2D(_MainTex, i.uv);
			return c;
		}
	    ENDCG
    }
}
}
