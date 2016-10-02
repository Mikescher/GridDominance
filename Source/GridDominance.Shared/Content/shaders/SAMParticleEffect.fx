struct VertexShaderInput
{
	float4 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	return input;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color;
	color.r = 1;
	color.g = 0;
	color.b = 0;
	color.a = 1;
	return color;
}

technique Ambient
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}