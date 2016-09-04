// Particle texture and sampler.
texture Texture;

sampler Sampler = sampler_state
{
    Texture = (Texture);
};

// Pixel shader for drawing particles.
float4 ParticlePixelShader(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
    return tex2D(Sampler, input.Position);
}


// Effect technique for drawing particles.
technique Particles
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_1 ParticlePixelShader();
    }
}