#ifndef CUSTOM_BRDF_INCLUDED
#define CUSTOM_BRDF_INCLUDED
//BRDF

//有一个最小的反射率
#define MIN_REFLECTIVITY 0.04 

struct BRDF{
	float3 diffuse;
	float3 specular;
	float roughness;
};

float OneMinusReflectivity(float metallic){
	float range = 1.0 - MIN_REFLECTIVITY;
	return range - metallic * range; //将范围从[0,1]*[1-金属度]调整为[0,0.96] * [1-金属度]
}

BRDF GetBRDF(Surface surface){
	BRDF brdf;
	float oneMinusReflectivity = OneMinusReflectivity(surface.metallic); //物体的漫反射率，金属度越低漫反射就越多
	brdf.diffuse = surface.color * oneMinusReflectivity;
	brdf.specular = lerp(MIN_REFLECTIVITY, surface.color, surface.metallic);
	float perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(surface.smoothness); //内置的从光滑度转到粗糙度的函数
	brdf.roughness = PerceptualRoughnessToRoughness(perceptualRoughness); //转换成实际的粗糙度值
	return brdf;
}

//计算镜面反射的强度，用brdf的公式
float SpecularStrength(Surface surface, BRDF brdf, Light light){
	float3 h = SafeNormalize(light.direction + surface.viewDirection);//半角向量
	float nh2 = Square(saturate(dot(surface.normal, h))); //n·h的平方
	float lh2 = Square(saturate(dot(light.direction, h)));
	float r2 = Square(brdf.roughness);
	float d2 = Square(nh2 * (r2 - 1) + 1.0001);
	float normalization = brdf.roughness * 4.0 + 2.0;
	
	return r2 / (d2 * max(0.1, lh2) * normalization);
}

float3 DirectBRDF(Surface surface, BRDF brdf, Light light){
	return SpecularStrength(surface, brdf, light) * brdf.specular + brdf.diffuse;
}

#endif