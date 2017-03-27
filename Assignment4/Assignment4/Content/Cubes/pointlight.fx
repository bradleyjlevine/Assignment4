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
float4x4 WorldInverseTranspose;

float3 LightPosition[3];
float LightRadius = 100;

float3 Camera;
static const float PI = 3.14159265f;

float4 DiffuseColor;
float4 DiffuseIntensity;
float4 AmbientColor;
float AmbientIntensity;
float Shininess = 10;
float4 SpecularColor[3];
float SpecularIntensity = 0.5;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
};
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 WorldPosition : POSITIONT;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	output.WorldPosition = worldPosition;
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	float3 normal = normalize(mul(input.Normal, (float3x3)WorldInverseTranspose));
	output.Normal = normal;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{	
    float diffuseColor = 0;
    float4 specular = float4(0, 0, 0, 0);
    float4 ambient = float4(0, 0, 0, 0);

	for(int i = 0; i < 2; i++)
	{
		float3 lightDirection = LightPosition[i] - (float3)input.WorldPosition;
		float intensity = pow(1-saturate(length(lightDirection) / LightRadius), 2);
		lightDirection = normalize(lightDirection);
        float3 normal = normalize(input.Normal);
        diffuseColor += dot(normal, lightDirection) * intensity;
		float3 reflect = normalize(2 * diffuseColor * normal - lightDirection);
        float3 view = normalize(Camera - (float3) input.WorldPosition);
		float dotProduct = dot(reflect, view);
        specular += (8 + Shininess) / (8 * PI) * SpecularIntensity * SpecularColor[i] * pow(saturate(dotProduct), Shininess) * intensity;
    }
	
    ambient = AmbientColor * AmbientIntensity;
    ambient[3] = 1;
    specular[3] = 1;

	return saturate(diffuseColor + ambient + specular);
}

technique PointLight
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};
