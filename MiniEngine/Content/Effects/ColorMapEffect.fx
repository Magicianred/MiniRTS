#include "Includes/Defines.hlsl"
#include "Includes/Matrices.hlsl"
#include "Includes/Pack.hlsl"

texture Texture;
sampler diffuseSampler = sampler_state
{
    Texture = (Texture);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;    
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;    
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz,1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);      
    output.TexCoord = input.TexCoord;    

    return output;
}

float4  MainPS(VertexShaderOutput input) : COLOR0
{    
    float2 texCoord = input.TexCoord;    

    float4 color = tex2D(diffuseSampler, texCoord);
    color.rgb *= (1.0f - color.a);   
    return color;
}

technique RenderTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}