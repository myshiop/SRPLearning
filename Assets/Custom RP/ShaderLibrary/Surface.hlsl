#ifndef CUSTOM_SURFACE_INCLUDED
#define CUSTOM_SURFACE_INCLUDED
//计算着色结果

struct Surface{
	float3 normal;
	float3 viewDirection; 
	float3 color;
	float alpha;
	float metallic;
	float smoothness;
};

#endif