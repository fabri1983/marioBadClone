Shader "Custom/Unlit Background Mirror CG" {

Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
	
SubShader {
    Tags { "Queue"="Background" "IgnoreProjector"="True" "RenderType"="Opaque" 
        	"PreviewType"="Plane" "CanUseSpriteAtlas"="False" }
    ZWrite Off
	Lighting Off
	Fog { Mode Off }
	Blend Off
	
	Pass {
		Cull Off // here it solves an issue (donno which issue)
		
		CGPROGRAM
		#pragma target 2.0
	    #pragma exclude_renderers ps3 xbox360 flash glesdesktop d3d11
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
			// mirroring: makes the UV repeat between its tilling values 0.0 and 2.0
    		fixed2 t = frac(i.uv * 0.5) * 2.0; // frac(x): returns x - floor(x)
    		fixed2 length = {1.0, 1.0};
    		fixed2 mirrorTexCoords = length - abs(t - length);

			half4 c = tex2D(_MainTex, mirrorTexCoords);
			return c;
		}
	    ENDCG
    }
}
}