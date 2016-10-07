#if OPENGL
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


float2 Offset;
float4x4 VirtualViewport;
float CurrentTime;

float4 ColorInitial;
float4 ColorFinal;

texture Texture;
float3x3 TextureProjection;

sampler Sampler = sampler_state
{
	Texture = (Texture);

	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Point;

	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	float2 Corner : POSITION0;

	float2 StartPosition : POSITION1;
	float2 Velocity : NORMAL0;
	
	float StartTime : TEXCOORD0;
	float Lifetime : TEXCOORD1;
	
	float StartSize : TEXCOORD2;
	float FinalSize : TEXCOORD3;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 TextureCoordinate : COLOR1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float age = CurrentTime - input.StartTime;
	float progress = age / input.Lifetime;
	float size = lerp(input.StartSize, input.FinalSize, progress) / 2;

	float colorR = lerp(ColorInitial.r, ColorFinal.r, progress);
	float colorG = lerp(ColorInitial.g, ColorFinal.g, progress);
	float colorB = lerp(ColorInitial.b, ColorFinal.b, progress);
	float colorA = lerp(ColorInitial.a, ColorFinal.a, progress);

	float4 worldPosition = float4(input.StartPosition.x + input.Corner.x * size + input.Velocity.x * age, input.StartPosition.y + input.Corner.y * size + input.Velocity.y * age, 0, 1);
	worldPosition.x += Offset.x;
	worldPosition.y += Offset.y;
	output.Position = mul(worldPosition, VirtualViewport);

	output.Color = float4(colorR, colorG, colorB, 1) * colorA;

	float3 texcoords = float3((input.Corner.x + 1) / 2, (input.Corner.y + 1) / 2, 1);
	output.TextureCoordinate = mul(texcoords, TextureProjection);

	if (progress > 1) output.Color = float4(1,1,1,1);

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return tex2D(Sampler, input.TextureCoordinate) * input.Color;
}

technique Ambient
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}