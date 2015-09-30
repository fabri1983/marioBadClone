// Same as Custom/UnlitTransparentGeomAnimCG however this shader runs all the offset operations in the vertex shader
Shader "Custom/Unlit Transparent Geom Anim Full CG" {

Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_SetupVec1 ("SetupVec1", Vector) = (0,0,0,0)
	_SetupVec2 ("SetupVec2", Vector) = (0,0,0,0)
}

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" 
	    	"PreviewType"="Plane" "CanUseSpriteAtlas"="False" }
	Lighting Off
	ZWrite Off
	Fog { Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha // The generated color is multiplied by the SrcFactor. The color already on screen is multiplied by DstFactor and the two are added together.

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
		uniform fixed4 _SetupVec1, _SetupVec2;
		
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
			
			// positioning of variables:
			// _SetupVec1: _index, _maxColsInRows, _rowsTotalInSprite, offsetYStart
			// _SetupVec2: _textureTiling.x, _textureTiling.y, _offset.x, _offset.y
			
			fixed2 _offset;
			fixed xTemp = _SetupVec1.x / _SetupVec1.y;
			fixed xTempFloor = floor(_SetupVec1.x / _SetupVec1.y);
			_offset.x = xTemp - xTempFloor;
			_offset.y = 1.0 - (xTempFloor / _SetupVec1.z) - _SetupVec1.w;
	 
			// Reset the y offset, if needed
			//if (_offset.y == 1.0) _offset.y = 0.0;
			// Consider a replacement using: 
			//   saturate(x): clamps x to [0,1]
			//   step(a,x): if x < a then 0, if x >= a 1
			//   min/max
	 		_offset.y = (step(1.0, _offset.y) - 1.0) * -1.0 * _offset.y;
	 		
	        // If we have scaled the texture, we need to reposition the texture to the center of the object
	        _offset.x += ((1.0 / _SetupVec1.y) - _SetupVec2.x) * 0.5;
	        _offset.y += ((1.0 / _SetupVec1.z) - _SetupVec2.y) * 0.5;
	 		// try this: _offset.xy = ((fixed2(1.0, 1.0) / _SetupVec1.xy) - _SetupVec2.xy) * 0.5;
	 		
	        // Add an additional offset if the user does not want the texture centered
	        _offset.x += _SetupVec2.z;
	        _offset.y += _SetupVec2.w;
	        // try this: _offset.xy += _SetupVec2.zw;
	        
			o.uv = i.texcoord.xy * _SetupVec2.xy + _offset;
			 
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
