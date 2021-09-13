/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	{
            "NAME": "SUNX",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
          },
          {
            "NAME": "SUNY",
            "TYPE": "float",
            "DEFAULT": 0.3,
            "MIN": 0.0,
            "MAX": 1.0
          },
          {
            "NAME": "SCATTERING",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 1.0
          },
          {
            "NAME": "UNDERSCATTERING",
            "TYPE": "float",
            "DEFAULT": 0.2,
            "MIN": 0.0,
            "MAX": 1.0
          },
          {
            "NAME": "SUNSIZE",
            "TYPE": "float",
            "DEFAULT": 0.01,
            "MIN": 0.0,
            "MAX": 1.0
          },
          {
            "NAME": "HALOSIZE",
            "TYPE": "float",
            "DEFAULT": 0.9,
            "MIN": 0.0,
            "MAX": 1.0
          },
           {
			"NAME": "SKYCOLOUR",
			"TYPE": "color",
			"DEFAULT": [
				0.3,
				0.5,
				1.0,
				1.0
			]
			}
	]
}*/

//Ported/Adapted from "Realy Simple Atmospheric Scattering" by robobo1221: https://www.shadertoy.com/view/4tVSRt

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

float coeiff = SCATTERING;
vec3 totalSkyLight = vec3 (SKYCOLOUR.r, SKYCOLOUR.g, SKYCOLOUR.b);

vec3 mie(float dist, vec3 sunL){
    return exp(-pow(dist, 0.25)) * sunL - 0.4;
}

vec3 getSky(vec2 uv, out vec3 mieScatter){
	
	vec2 sunPos = vec2(SUNX,SUNY*2.-.5);
    
    float sunDistance = distance(uv, clamp(sunPos, -1., 1.0));
	
	float scatterMult = clamp(sunDistance, .003, 1.0);
	float sun = clamp(1.0 - smoothstep(0.001, SUNSIZE+.003, scatterMult), 0.0, 1.0);
	
	float dist = uv.y;
	
	dist = (coeiff * mix(scatterMult, 1.0, dist/(1.-HALOSIZE+.003))) / dist;
    
    mieScatter = mie(sunDistance, vec3(1.0));
	
	vec3 color = dist * totalSkyLight;
    
	color = mix(pow(color, 1.0 - color),
	color / (2.0 * color + 0.5 - color),
	clamp(sunPos.y * 2.0, 0.0, 1.0))
	+ sun + mieScatter;
	
	color *=  1.0 + pow(1.0 - scatterMult, 10.0) * 10.0;
	
	float underscatter = distance(sunPos.y * 0.5 + UNDERSCATTERING, 1.0);
	
	color = mix(color, vec3(0.0), clamp(underscatter, 0.0, 1.0));
	
	return color;	
}

void mainImage( out vec4 fragColor, in vec2 fragCoord ){
    
    vec3 mieScatter = vec3(0.0);
    
	vec3 color = getSky(fragCoord.xy / iResolution.xy, mieScatter);
	
	color = color / (2.0 * color + 0.5 - color);
	
	fragColor = vec4(color, 1.0);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}