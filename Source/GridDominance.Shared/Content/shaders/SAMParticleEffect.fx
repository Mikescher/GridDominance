#if OPENGL
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//---------------------------------------------------------------------------------------------------------
//  ============================================== PARAMETER ==============================================
//---------------------------------------------------------------------------------------------------------

//----------------- WORLD CONFIG -----------------

float2 Offset;
float4x4 VirtualViewport;
float CurrentTime;

//------------ PARTICLE SYSTEM CONFIG ------------

texture Texture;
float3x3 TextureProjection;

float ParticleLifetimeMin;
float ParticleLifetimeMax;
float ParticleRespawnTime;

float ParticleSpawnAngleMin;
float ParticleSpawnAngleMax;
bool ParticleSpawnAngleIsRandom;
float2 FixedParticleSpawnAngle;

float ParticleVelocityMin;
float ParticleVelocityMax;

float ParticleAlphaInitial;
float ParticleAlphaFinal;

float ParticleSizeInitialMin;
float ParticleSizeInitialMax;

float ParticleSizeFinalMin;
float ParticleSizeFinalMax;

float4 ColorInitial;
float4 ColorFinal;

//------------------------------------------------

sampler Sampler = sampler_state
{
	Texture = (Texture);

	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Point;

	AddressU = Clamp;
	AddressV = Clamp;
};

//---------------------------------------------------------------------------------------------------------
//  ================================================ TYPES ================================================
//---------------------------------------------------------------------------------------------------------

struct VertexShaderInput
{
	float2 Corner          : POSITION0;
	float2 StartPosition   : POSITION1;
	float  StartTimeOffset : TEXCOORD0;
	float4 Random          : POSITION2;
};

struct VertexShaderOutput
{
	float4 Position     : POSITION0;
	float4 Color        : COLOR0;
	float2 TextureCoord : COLOR1;
};

//---------------------------------------------------------------------------------------------------------
//  ============================================== FUNCTIONS ==============================================
//---------------------------------------------------------------------------------------------------------

float rand(float2 co) 
{
	return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
}

float2 rot2(float2 vec, float rad)
{
	return float2(vec.x * cos(rad) - vec.y * sin(rad), vec.x * sin(rad) + vec.y * cos(rad));
}

float randLerp(float min, float max, float4 seed, int iteration, int modifier)
{
	return lerp(min, max, rand(float2(iteration + seed[modifier % 4], iteration + seed[int(modifier / 4) % 4])));
}

//---------------------------------------------------------------------------------------------------------
//  ================================================ SHADER ===============================================
//---------------------------------------------------------------------------------------------------------

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	int iteration = int((CurrentTime + input.Random.x * ParticleRespawnTime) / ParticleRespawnTime);
	float age = (CurrentTime + input.Random.x * ParticleRespawnTime) % ParticleRespawnTime;

	float MaxLifetime = randLerp(ParticleLifetimeMin, ParticleLifetimeMax, input.Random, iteration, 1);
	float StartLifetime = randLerp(0, ParticleRespawnTime - MaxLifetime, input.Random, iteration, 2);
	float EndLifetime = (StartLifetime + MaxLifetime);

	float progress = (age - StartLifetime) / MaxLifetime;

	float StartSize = randLerp(ParticleSizeInitialMin, ParticleSizeInitialMax, input.Random, iteration, 3);
	float FinalSize = randLerp(ParticleSizeFinalMin, ParticleSizeFinalMax, input.Random, iteration, 4);

	float2 Velocity = FixedParticleSpawnAngle;
	if (ParticleSpawnAngleIsRandom == true)
	{
		float angle = randLerp(ParticleSpawnAngleMin, ParticleSpawnAngleMax, input.Random, iteration, 6);
		float absVel = randLerp(ParticleVelocityMin, ParticleVelocityMax, input.Random, iteration, 7);
		Velocity = rot2(float2(absVel, 0), angle);
	}

	float size = lerp(StartSize, FinalSize, progress) / 2;

	float colorR = lerp(ColorInitial.r, ColorFinal.r, progress);
	float colorG = lerp(ColorInitial.g, ColorFinal.g, progress);
	float colorB = lerp(ColorInitial.b, ColorFinal.b, progress);
	float colorA = lerp(ParticleAlphaInitial, ParticleAlphaFinal, progress);

	float4 worldPosition = float4(input.StartPosition.x + input.Corner.x * size + Velocity.x * age, input.StartPosition.y + input.Corner.y * size + Velocity.y * age, 0, 1);
	worldPosition.x += Offset.x;
	worldPosition.y += Offset.y;
	output.Position = mul(worldPosition, VirtualViewport);

	output.Color = float4(colorR, colorG, colorB, 1) * colorA;

	float3 texcoords = float3((input.Corner.x + 1) / 2, (input.Corner.y + 1) / 2, 1);
	output.TextureCoord = mul(texcoords, TextureProjection);


	if (age<StartLifetime || age>EndLifetime || CurrentTime < input.StartTimeOffset)
	{
		// in theory this could be done a lot earlier
		// and the function could be early terminated
		// but then android does strange things (ignores the return ??)
		// so ... wtf
		output.Position = float4(0, 0, 0, 0);
		output.Color = float4(0, 0, 0, 0);
		output.TextureCoord = float2(0, 0);
		return output;
	}


	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	if (input.Color.a == 0.0) discard;

	return tex2D(Sampler, input.TextureCoord) * input.Color;
}

//---------------------------------------------------------------------------------------------------------
//  ============================================== TECHNIQUE ==============================================
//---------------------------------------------------------------------------------------------------------

technique ParticleTech
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}