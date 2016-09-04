// Particle texture and sampler.
texture Texture;

sampler Sampler = sampler_state
{
	Texture = (Texture);
};

// Pixel shader for drawing particles.
float4 ParticlePixelShader(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
	float4 color = 1;
	return color;
}


// Effect technique for drawing particles.
technique Particles
{
	pass P0
	{
		PixelShader = compile ps_2_0 ParticlePixelShader();
	}
}