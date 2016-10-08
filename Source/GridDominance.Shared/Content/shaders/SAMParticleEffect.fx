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
	float2 Random          : POSITION2;
};

struct VertexShaderOutput
{
	float4 Position     : POSITION0;
	float4 Color        : COLOR0;
	float2 TextureCoord : COLOR1;
};

//---------------------------------------------------------------------------------------------------------
//  ================================================ NOISE ================================================
//  https://github.com/ashima/webgl-noise/blob/bfd18615a9662aa329c80998d9fdcbc7d605dd5f/src/noise2D.glsl
//---------------------------------------------------------------------------------------------------------

float3 mod289(float3 x) {
	return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float2 mod289(float2 x) {
	return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float3 permute(float3 x) {
	return mod289(((x*34.0)+1.0)*x);
}

float snoise(float2 v)
{
	const float4 C = float4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
	                        0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
	                       -0.577350269189626,  // -1.0 + 2.0 * C.x
	                        0.024390243902439); // 1.0 / 41.0
	// First corner
	float2 i	= floor(v + dot(v, C.yy) );
	float2 x0 = v -	 i + dot(i, C.xx);

	// Other corners
	float2 i1;
	//i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
	//i1.y = 1.0 - i1.x;
	i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
	// x0 = x0 - 0.0 + 0.0 * C.xx ;
	// x1 = x0 - i1 + 1.0 * C.xx ;
	// x2 = x0 - 1.0 + 2.0 * C.xx ;
	float4 x12 = x0.xyxy + C.xxzz;
	x12.xy -= i1;

	// Permutations
	i = mod289(i); // Avoid truncation effects in permutation
	float3 p = permute( permute( i.y + float3(0.0, i1.y, 1.0 )) + i.x + float3(0.0, i1.x, 1.0 ));

	float3 m = max(0.5 - float3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
	m = m*m;
	m = m*m;

	// Gradients: 41 points uniformly over a line, mapped onto a diamond.
	// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

	float3 x = 2.0 * frac(p * C.www) - 1.0;
	float3 h = abs(x) - 0.5;
	float3 ox = floor(x + 0.5);
	float3 a0 = x - ox;

	// Normalise gradients implicitly by scaling m
	// Approximation of: m *= inversesqrt( a0*a0 + h*h );
	m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );

// Compute final noise value at P
	float3 g;
	g.x  = a0.x  * x0.x   + h.x  * x0.y;
	g.yz = a0.yz * x12.xz + h.yz * x12.yw;
	return 130.0 * dot(m, g);
}

//---------------------------------------------------------------------------------------------------------
//  ============================================== FUNCTIONS ==============================================
//---------------------------------------------------------------------------------------------------------

float2 rot2(float2 vec, float rad)
{
	return float2(vec.x * cos(rad) - vec.y * sin(rad), vec.x * sin(rad) + vec.y * cos(rad));
}

float randLerp(float min, float max, float2 seed, int seedModifier1, int seedModifier2)
{
	float rng = snoise(float2(seed.x * (seedModifier1 + 1), seed.y * (seedModifier2 + 1)));

	return lerp(min, max, rng);
}

float randNoise(float2 seed, int seedModifier1, int seedModifier2)
{
	return snoise(float2(seed.x * (seedModifier1 + 1), seed.y * (seedModifier2 + 1)));
}

//---------------------------------------------------------------------------------------------------------
//  ================================================ SHADER ===============================================
//---------------------------------------------------------------------------------------------------------

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	int iteration = int((CurrentTime + input.StartTimeOffset) / ParticleRespawnTime);
	float age = (CurrentTime + input.StartTimeOffset) % ParticleRespawnTime;

	float r1 = randNoise(input.Random, iteration, 1);

	float MaxLifetime = lerp(ParticleLifetimeMin, ParticleLifetimeMax, r1);

	if (age > MaxLifetime)
	{
		output.Position = float4(0, 0, 0, 0);
		output.Color = float4(0, 0, 0, 0);
		output.TextureCoord = float2(0, 0);
		return output;
	}


	float r2 = randNoise(input.Random, iteration, 1);

	float StartSize = lerp(ParticleSizeInitialMin, ParticleSizeInitialMax, r1);
	float FinalSize = lerp(ParticleSizeFinalMin, ParticleSizeFinalMax, r2);

	//float2 Velocity = FixedParticleSpawnAngle;
	//if (ParticleSpawnAngleIsRandom)
	//{
		float angle = lerp(ParticleSpawnAngleMin, ParticleSpawnAngleMax, r2);
		float absVel = lerp(ParticleVelocityMin, ParticleVelocityMax, r1);
		float2 Velocity = rot2(float2(absVel, 0), angle);
	//}

	float progress = age / MaxLifetime;
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

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
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