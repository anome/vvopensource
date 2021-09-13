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
			"LABEL": "ATTENUATION",
			"NAME": "ATTENUATION",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "RADIUS",
			"NAME": "RADIUS",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		}
		]
}*/

//Ported from "Spiral Kernel Basic SSAO" by zachernuk: https://www.shadertoy.com/view/MddGWB

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

#define SAMPLES 16
float dGamma = 100.;//1:10:2
float oscSpeed = 3.;//0:10:5

float radAttenuation = ATTENUATION * 10. ;//0:2:1
float radius = RADIUS; //0:0.6:0.024
float spiral = 16524.56; //1:100:50
float spinSpeed = .10;


void mainImage( out vec4 fragColor, in vec2 fragCoord )
{

    vec2 uv = fragCoord.xy/iResolution.xy;
    float dp = IMG_NORM_PIXEL(iChannel0, uv).r;

  float f;
  vec2 offset;
    float dTotal;

    for(int i = 0;i<SAMPLES;i++) {
      f = float(i)/float(SAMPLES);
     offset = vec2(radius*pow(f, radAttenuation)*sin(f*spiral+spinSpeed), 
                  radius*pow(f, radAttenuation)*cos(f*spiral+spinSpeed));
      float dd = IMG_NORM_PIXEL(iChannel0,uv+offset).r-dp;
      dTotal+=max(dd,0.);
    }
    dTotal/=float(SAMPLES);
    dTotal = (1.-dTotal);
    dTotal = pow(dTotal,dGamma);
    fragColor.rgb = vec3(dTotal*(0.35*dp+0.65));
    fragColor.rgb = mix(fragColor.rgb, vec3(dp), 0.);
    fragColor.a = 1.;

}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
    
}