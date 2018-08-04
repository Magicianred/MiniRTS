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

texture SpecularMap;
sampler specularSampler = sampler_state
{
    Texture = (SpecularMap);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture NormalMap;
sampler normalSampler = sampler_state
{
    Texture = (NormalMap);
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    MipFilter = LINEAR;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Mask;
sampler maskSampler = sampler_state
{
    Texture = (Mask);
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
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float3x3 tangentToWorld : TEXCOORD2;    
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz,1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);      
    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;
    
    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(float4(input.Tangent, 0), World).xyz;
    output.tangentToWorld[1] = mul(float4(input.Binormal, 0), World).xyz;
    output.tangentToWorld[2] = mul(float4(input.Normal, 0), World).xyz;

    return output;
}

struct PixelShaderOutput
{
    float4 Color : COLOR0;
    float4 Normal : COLOR1;
    float4 Depth : COLOR2;
};

PixelShaderOutput MainPS(VertexShaderOutput input)
{
    PixelShaderOutput output = (PixelShaderOutput)0;
    
    float mask = tex2D(maskSampler, input.TexCoord).r;
    if(mask < 0.5f)
    {   
        clip(-1);       
        return output;
    }

    output.Color = tex2D(diffuseSampler, input.TexCoord);
    
    // read the normal from the normal map
    float3 normalFromMap = tex2D(normalSampler, input.TexCoord).rgb;
    //tranform to [-1,1]
    normalFromMap = (2.0f * normalFromMap) - 1.0f;
    //transform into world space
    normalFromMap = mul(normalFromMap, input.tangentToWorld);
    //normalize the result
    normalFromMap = normalize(normalFromMap);
    //output the normal, in [0,1] space
    output.Normal.rgb = 0.5f * (normalFromMap + 1.0f);    

    //specular Power
    float specularPower = tex2D(specularSampler, input.TexCoord).r;           
    
    // Shininess is stored in textures with black is most shiney, and white is non-shiney
    // make 1.0f most shiney here
    output.Normal.a = 1.0f - specularPower;    
    
    output.Depth = input.Depth.x / input.Depth.y;

    return output;
}

technique RenderTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
