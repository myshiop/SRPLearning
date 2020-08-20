#ifndef CUSTOM_SURFACE_INCLUDED
#define CUSTOM_SURFACE_INCLUDED

struct Surface{
	float3 normal;
	float3 color;
	float3 alpha;
};

float3 GetLighting (Surface surface){
	return surface.normal.y * surface.color;	
}


#endif