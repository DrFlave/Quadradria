sampler s0;

int Size;
float2 Direction;
const float gauss[] = { 1.0, 1.0, 1.0, 1.0, 1.0 };

float4 FragmentShader(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = float4(0, 0, 0, 1);
    float pixel = 1.0/Size;

	[loop]
    for (float i = -5; i <= 5; i++) {
		color += tex2D(s0, coords + Direction * i * pixel) * (1.0 / max(i*i, 0.1));
    }

    return color;
}

technique GaussianBlur
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 FragmentShader();
    }
}