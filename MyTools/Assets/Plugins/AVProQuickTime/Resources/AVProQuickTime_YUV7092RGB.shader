// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/AVProQuickTime_CompositeYUV2709RGB" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TextureWidth ("Texure Width", Float) = 256.0
	}
	SubShader 
	{
		Pass
		{ 
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
		
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
float _TextureWidth;
float4 _MainTex_ST;
float4 _MainTex_TexelSize;

struct v2f {
	float4 pos : POSITION;
	float4 uv : TEXCOORD0;
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
	
	// On D3D when AA is used, the main texture & scene depth texture
	// will come out in different vertical orientations.
	// So flip sampling of the texture when that is the case (main texture
	// texel size will have negative Y).
	#if SHADER_API_D3D9
	if (_MainTex_TexelSize.y < 0)
	{
		o.uv.y = 1-o.uv.y;
	}
	#endif
	

	return o;
}

// BT709
float4
convertYUV(float y, float v, float u)
{
	float rr = saturate( 1.164 * (y - (16.0 / 255.0)) + 1.793 * (u - 0.5) );
	float gg = saturate( 1.164 * (y - (16.0 / 255.0)) - 0.534 * (u - 0.5) - 0.213 * (v - 0.5) );
	float bb = saturate( 1.164 * (y - (16.0 / 255.0)) + 2.115 * (v - 0.5) );
	return float4(rr, gg, bb, 1);
}

float4 frag (v2f i) : COLOR
{
	float4 uv = i.uv;
	
	uv.x /= 2;
	float4 col = tex2D(_MainTex, uv.xy);

	float realTexCoordX = uv.x * _TextureWidth;

	float4 oCol = 0;
	if (frac(realTexCoordX) > 0.5 )
	{
		oCol = convertYUV(col.z, col.y, col.w);
		//oCol = col.z;
	}
	else
	{
		float xoffset = _MainTex_TexelSize.x;
		float4 col3 = tex2D(_MainTex, uv.xy - float2(xoffset, 0.0));
	
		float u = (col.y + col3.y) * 0.5;
		float v = (col.w + col3.w) * 0.5;
		oCol = convertYUV(col.x, u, v);
		//oCol = 1;
		//oCol = col.x;
	}
	
	//oCol = col;
	
	return oCol;
} 
ENDCG
		}
	}
	
	FallBack Off
}