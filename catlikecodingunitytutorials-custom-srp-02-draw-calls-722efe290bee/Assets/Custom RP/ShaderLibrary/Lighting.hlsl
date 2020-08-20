#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

float3 GetIncomingLight(Surface surface, Light light){
	return saturate(dot(surface.normal, light.dircetion)) * light.color;
}

float3 GetLighting(Surface surface){
	return GetIncomingLight(surface, GetDirectionalLight());
}

#endif