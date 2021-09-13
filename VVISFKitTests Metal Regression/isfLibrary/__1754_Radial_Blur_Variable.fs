/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
		{
			"NAME": "iChannel0",
			"TYPE": "image"
		},
		{
			"NAME": "Blur",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "Layers",
			"TYPE": "float",
			"MIN": 1.0,
			"MAX": 100.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "ShiftX",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "ShiftY",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},{
			"NAME": "ShiftFactor",
			"TYPE": "float",
			"MIN": 1.0,
			"MAX": 5.0,
			"DEFAULT": 1.0
		}
      	]
}*/

// Based on "You are drunk man" by Danil: https://www.shadertoy.com/view/ls3SDX

vec3 iResolution = vec3(RENDERSIZE, 1.);

const int samples = 101;

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 shift= vec2 (ShiftX*ShiftFactor,ShiftY*ShiftFactor);
	vec2 p = fragCoord.xy / iResolution.xy;
	
    vec4 result = vec4(0);
    
  for (int i=0; i<=samples; i++)
  {
  
		if (float(i) >= Layers) {
        break;
        
     }
        float q = float(i)/float(Layers);
        result += texture2D(iChannel0, p + (vec2(0.5+shift)-p)*q*Blur)/float(Layers);
    }
    
	
	fragColor = result;
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}