sampler s0;

int Size;
float2 Direction;

float samples[5] = {0.1, 0.2, 0.4, 0.2, 0.1};

float4 FragmentShader(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = float4(0, 0, 0, 1);
    float pixel = 1.0/Size;

    for (int i = -4; i < 4; i++) {
        color += tex2D(s0, coords + Direction * i * pixel) * (1.0/8);
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