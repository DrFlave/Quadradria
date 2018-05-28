sampler s0;

int Size;
float2 Direction;

//const float gauss[] = { 1.0, 0.1945945946, 0.1216216216, 0.0540540541, 0.0162162162 };//{ 0.2270270270, 0.1945945946, 0.1216216216, 0.0540540541, 0.0162162162 };
//const float gauss[] = { 1.0, 0.35121951219, 0.08780487804, 0.03902439024, 0.02195121951 };
const float gauss[] = { 1.0, 1.0, 1.0, 1.0, 1.0 };

float4 FragmentShader(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = float4(0, 0, 0, 1);
    float pixel = 1.0/Size;

	[loop]
    for (float i = -5; i <= 5; i++) {
		color += tex2D(s0, coords + Direction * i * pixel) * (1.0 / max(i*i, 0.1));
    }

	/*
	color += tex2D(s0, coords + Direction * -4 * pixel) * 0.02195121951;
	color += tex2D(s0, coords + Direction * -3 * pixel) * 0.03902439024;
	color += tex2D(s0, coords + Direction * -2 * pixel) * 0.08780487804;
	color += tex2D(s0, coords + Direction * -1 * pixel) * 0.35121951219;
	color += tex2D(s0, coords + Direction *  0 * pixel) * 1.0;
	color += tex2D(s0, coords + Direction *  1 * pixel) * 0.35121951219;
	color += tex2D(s0, coords + Direction *  2 * pixel) * 0.08780487804;
	color += tex2D(s0, coords + Direction *  3 * pixel) * 0.03902439024;
	color += tex2D(s0, coords + Direction *  4 * pixel) * 0.02195121951;*/

    return color;
}

technique GaussianBlur
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 FragmentShader();
    }
}