#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position : POSITION0;   
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Position2D : TEXCOORD0;
}; 

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    
    float4 worldPosition = mul(float4(input.Position.xyz,1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);      
    output.Position2D = output.Position;

    return output;
}

struct PixelShaderOutput
{
    float4 Color : COLOR0;   
};

PixelShaderOutput MainPS(VertexShaderOutput input)
{
     PixelShaderOutput output = (PixelShaderOutput)0;

     output.Color = input.Position2D.z / input.Position2D.w;

     return output;
}

technique Technique1
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}