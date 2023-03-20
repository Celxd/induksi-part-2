// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef UNITY_STANDARD_INPUT_INCLUDED
#define UNITY_STANDARD_INPUT_INCLUDED

#include "UnityCG.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityPBSLighting.cginc" // TBD: remove
#include "UnityStandardUtils.cginc"

//---------------------------------------
// Directional lightmaps & Parallax require tangent space too
#if (_NORMALMAP || DIRLIGHTMAP_COMBINED || _PARALLAXMAP)
    #define _TANGENT_TO_WORLD 1
#endif

//---------------------------------------
sampler2D   _MainTex;
float4      _MainTex_ST;

sampler2D   _BumpMap;
sampler2D   _SpecGlossMap;
sampler2D   _MetallicGlossMap;
sampler2D   _OcclusionMap;
sampler2D   _ParallaxMap;

sampler2D   _GlsSmcParBus;
sampler2D   _CutOccGlo;

sampler2D   _EmissionColorMap;
sampler2D   _EmissionMap;
float       _EmissionScale;

float       _BumpMapMax;

float       _AtlasWidth;
float       _AtlasHeight;
float       _MaxMipLevel;

//-------------------------------------------------------------------------------------
// Input functions

struct VertexInput
{
    fixed4 color    : COLOR0;
    float4 vertex   : POSITION;
    half3 normal    : NORMAL;
    float2 uv0      : TEXCOORD0;
    float2 uv1      : TEXCOORD1;
    float4 atlas    : TEXCOORD2;
#if defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META)
    float2 uv3      : TEXCOORD3;
#endif
#ifdef _TANGENT_TO_WORLD
    half4 tangent   : TANGENT;
#endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

float4 TexCoords(VertexInput v)
{
    float4 texcoord;
    texcoord.xy = TRANSFORM_TEX(v.uv0, _MainTex); // Always source from uv0
    texcoord.zw = float2(0,0); //TRANSFORM_TEX(v.uv0, _DetailAlbedoMap);
    return texcoord;
}

float4 pickUVlod(float2 uv, float4 bounds)
{
    float2 relativeSize = frac(bounds.zw);
    float2 scale = floor(bounds.zw) / 1000;
	float2 fracuv = frac(uv.xy * scale) * relativeSize;
	float2 uv_atlas = fracuv+bounds.xy;
    float2 subTextureSize = relativeSize*scale * float2(_AtlasWidth, _AtlasHeight);
	float2 dx = ddx(uv * subTextureSize.x);
	float2 dy = ddy(uv * subTextureSize.y);
	int d = max(dot(dx, dx), dot(dy, dy));

    const float rangeClamp = pow(2, _MaxMipLevel * 2);
    d = clamp(d, 1.0, rangeClamp);

    float mipLevel = 0.5 * log2(d);
    mipLevel = floor(mipLevel);
	
	return float4(uv_atlas, 0, mipLevel);
}

float4 pickUV(float2 uv, float4 bounds)
{
	float2 fracuv = frac(uv.xy * floor(bounds.zw) / 1000) * frac(bounds.zw);
	float2 uv_atlas = fracuv + bounds.xy;
        
	return float4(uv_atlas, 0, 0);
}


float4 tex2Datlas(sampler2D tex, float2 uv, float4 atlas)
{
    //return float4(1-pickUVlod(uv, atlas).a/_MaxMipLevel,0,0,1);
	return tex2Dlod(tex, pickUVlod(uv, atlas));
}

float4 tex2Dnomip(sampler2D tex, float2 uv, float4 atlas)
{
    //return tex2Dlod(tex, pickUVlod(uv, atlas));
    return tex2Dlod(tex, pickUV(uv, atlas));
}

half3 Albedo(float4 uv, float4 atlas, fixed4 color)
{
    half3 texColor = tex2Datlas (_MainTex, uv.xy, atlas).rgb;
    half3 albedo = color.rgb * texColor.rgb;

    return color * texColor;
}

#if defined(_ALPHATEST_ON)
half Cutoff(float2 uv, float4 atlas)
{
    return tex2Dnomip(_CutOccGlo, uv, atlas).x;
}
#endif

half Alpha(float2 uv, float4 atlas, fixed4 color)
{
    float channel = 1-tex2Dnomip(_GlsSmcParBus, uv, atlas).g;
    half texAlpha = tex2Datlas(_MainTex, uv, atlas).a;
    return lerp(color.a, texAlpha * color.a, channel);
}

half Occlusion(float2 uv, float4 atlas)
{
#if (SHADER_TARGET < 30)
    // SM20: instruction count limitation
    // SM20: simpler occlusion
    return tex2Datlas(_OcclusionMap, uv, atlas).g;
#else
    half occ = tex2Datlas(_OcclusionMap, uv, atlas).g;
    return LerpOneTo (occ, tex2Dnomip(_CutOccGlo, uv, atlas).y);
#endif
}

half4 SpecularGloss(float2 uv, float4 atlas)
{
    float channel = 1 - tex2Dnomip(_GlsSmcParBus, uv, atlas).g;
    half4 sg = tex2Datlas(_SpecGlossMap, uv, atlas);

    float glosinessStrength = tex2Dnomip(_GlsSmcParBus, uv, atlas).x;
    float glosiness = tex2Dnomip(_CutOccGlo, uv, atlas).z;

    sg.a = lerp(tex2Datlas(_MainTex, uv, atlas).a, glosiness, channel) * glosinessStrength;

    return sg;
}

half2 MetallicGloss(float2 uv, float4 atlas)
{
    half2 mg;
    half2 glossMapScaleAndChannel = tex2Dnomip(_GlsSmcParBus, uv, atlas).xy;
    half glossMapScale = glossMapScaleAndChannel.x;
    half channel = 1-glossMapScaleAndChannel.y;

    half glossiness = tex2Dnomip(_CutOccGlo, uv, atlas).z;
    
    half diffuseAlpha = tex2Datlas(_MainTex, uv, atlas).a;
    half scaledDiffuseAlpha = diffuseAlpha * glossMapScale;
    half2 metallicGloss = tex2Datlas(_MetallicGlossMap, uv, atlas).ra;
    
    mg.r = metallicGloss.r;
    
#ifdef _METALLICGLOSSMAP
    mg.g = lerp(scaledDiffuseAlpha, metallicGloss.g * glossMapScale, channel) ;
#else
    mg.g = lerp(scaledDiffuseAlpha, glossiness, channel);    
#endif

    //return half2(0,0);

    return mg;
}

half MetallicSimple(float2 uv, float4 atlas)
{
    return tex2Datlas(_MetallicGlossMap, uv, atlas).x;
}

half GlossinessSimple(float2 uv, float4 atlas)
{
    return tex2Dnomip(_CutOccGlo, uv, atlas).z;
}

half3 SpecularColor(float2 uv, float4 atlas)
{
    return tex2Datlas(_SpecGlossMap, uv, atlas).rgb;
}

half2 MetallicRough(float2 uv, float4 atlas)
{
    half2 mg;
    mg.r = tex2Datlas(_MetallicGlossMap, uv, atlas).r;

#ifdef _SPECGLOSSMAP
    mg.g = 1.0f - tex2Datlas(_SpecGlossMap, uv, atlas).r;
#else
    mg.g = 1.0f - tex2Dnomip(_CutOccGlo, uv, atlas).z;
#endif
    
    //return half2(0,0);

    return mg;
}

half3 Emission(float2 uv, float4 atlas)
{
    half3 emissionColor = tex2Dnomip(_EmissionColorMap, uv, atlas).rgb;

#ifndef UNITY_COLORSPACE_GAMMA
    emissionColor = LinearToGammaSpace(emissionColor) * _EmissionScale;
    emissionColor = GammaToLinearSpace(emissionColor);
#else
    emissionColor = emissionColor * _EmissionScale;    
#endif

    return tex2Datlas(_EmissionMap, uv, atlas).rgb * emissionColor * _EmissionScale;
}

#ifdef _NORMALMAP
half3 NormalInTangentSpace(float4 texcoords, float4 atlas)
{
    half3 normalTangent = UnpackScaleNormal(tex2Datlas (_BumpMap, texcoords.xy, atlas), tex2Dnomip (_GlsSmcParBus, texcoords.xy, atlas).w * _BumpMapMax);

    return normalTangent;
}
#endif

float4 Parallax (float4 texcoords, half3 viewDir, float4 atlas)
{
#if !defined(_PARALLAXMAP) || (SHADER_TARGET < 30)
    // Disable parallax on pre-SM3.0 shader target models
    return texcoords;
#else
    half h = tex2Datlas (_ParallaxMap, texcoords.xy, atlas).g;
    float2 offset = ParallaxOffset1Step (h, tex2Dnomip (_GlsSmcParBus, texcoords.xy, atlas).z, viewDir);
    return float4(texcoords.xy + offset, texcoords.zw + offset);
#endif

}

#endif // UNITY_STANDARD_INPUT_INCLUDED
