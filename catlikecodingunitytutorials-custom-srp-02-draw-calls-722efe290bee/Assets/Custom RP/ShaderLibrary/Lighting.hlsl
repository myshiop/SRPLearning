#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED
//着色过程

float3 GetIncomingLight(Surface surface, Light light){
	return saturate(dot(surface.normal, light.direction)) * light.color; //返回到达光源的亮度*颜色
}

float3 GetLighting(Surface surface,BRDF brdf, Light light){
	return GetIncomingLight(surface, light) * DirectBRDF(surface, brdf, light);
}

float3 GetLighting (Surface surface, BRDF brdf){
	float3 color = 0.0;
	for (int i = 0; i < GetDirectionalLightCount(); i++) 
	{
		color += GetLighting(surface, brdf, GetDirectionalLight(i)); //计算出光源的着色颜色
	}
	return color;
}

#endif