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

float3 LightPosition;
float LightRadius = 100;

float3 Camera;
static const float PI = 3.14159265f;

float4 AmbientColor;
float AmbientIntensity;
float4 DiffuseColor;
float DiffuseIntensity = 0.7;
float Shininess = 10;
float4 SpecularColor;
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
	float3 lightDirection = LightPosition - (float3)input.WorldPosition;
	float3 normal = normalize(input.Normal);
	float intensity = pow(1-saturate(length(lightDirection) / LightRadius), 2);
	lightDirection = normalize(lightDirection);
	float3 view = normalize(Camera-(float3)input.WorldPosition);
	float diffuseColor = dot(normal,lightDirection) * intensity;
	float3 reflect = normalize(2 * diffuseColor * normal - lightDirection);
	float dotProduct = dot(reflect, view);
	float4 specular = (8 + Shininess) / (8 * PI) * SpecularIntensity * SpecularColor * pow(saturate(dotProduct), Shininess) * intensity;
	
	return saturate(diffuseColor + AmbientColor * AmbientIntensity + specular);
}

technique PointLight
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
};