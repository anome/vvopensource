/*{
	"CREDIT": "by joshpbatty",
	"DESCRIPTION": "tripped",
	"CATEGORIES": [
		"Joshua Batty"
	],
	  "INPUTS": [
   	 {
		"NAME": "scale",
		"TYPE": "float",
		"DEFAULT": 0.83,
		"MIN": 0.0,
		"MAX": 1.0
	},
	{
		"NAME": "color_iter",
		"TYPE": "float",
		"DEFAULT": 0.05,
		"MIN": 0.0,
		"MAX": 1.0
	},
	{
		"NAME": "color_speed",
		"TYPE": "float",
		"DEFAULT": 0.15,
		"MIN": 0.0,
		"MAX": 1.0
	},
	{
		"NAME": "line_amp",
		"TYPE": "float",
		"DEFAULT": 1.0,
		"MIN": 0.0,
		"MAX": 1.0
	},
	{
		"NAME": "diag_amp",
		"TYPE": "float",
		"DEFAULT": 0.0,
		"MIN": 0.0,
		"MAX": 1.0
	},
	{
		"NAME": "boarder_amp",
		"TYPE": "float",
		"DEFAULT": 4.0,
		"MIN": 0.0,
		"MAX": 20.0
	}
  ]
}*/


//uncomment the line below to kill the color
//#define bw

#define PI 3.1415

vec4 hue(vec4 color, float shift) {

    const vec4  kRGBToYPrime = vec4 (0.299, 0.587, 0.114, 0.0);
    const vec4  kRGBToI     = vec4 (0.596, -0.275, -0.321, 0.0);
    const vec4  kRGBToQ     = vec4 (0.212, -0.523, 0.311, 0.0);

    const vec4  kYIQToR   = vec4 (1.0, 0.956, 0.621, 0.0);
    const vec4  kYIQToG   = vec4 (1.0, -0.272, -0.647, 0.0);
    const vec4  kYIQToB   = vec4 (1.0, -1.107, 1.704, 0.0);

    // Convert to YIQ
    float   YPrime  = dot (color, kRGBToYPrime);
    float   I      = dot (color, kRGBToI);
    float   Q      = dot (color, kRGBToQ);

    // Calculate the hue and chroma
    float   hue     = atan (Q, I);
    float   chroma  = sqrt (I * I + Q * Q);

    // Make the user's adjustments
    hue += shift;

    // Convert back to YIQ
    Q = chroma * sin (hue);
    I = chroma * cos (hue);

    // Convert back to RGB
    vec4    yIQ   = vec4 (YPrime, I, Q, 0.0);
    color.r = dot (yIQ, kYIQToR);
    color.g = dot (yIQ, kYIQToG);
    color.b = dot (yIQ, kYIQToB);

    return color;
}

float tri(float x) {
    return asin(sin(x))/(PI/2.);
}

float saw(float x) {
    return (fract((x/2.)/PI)-0.5)*2.;
}

void main()
{
    float i = TIME*(color_speed*3.0);
    //vec2 uv = fragCoord.xy-iResolution.xy*.5;
    vec2 uv = isf_FragNormCoord.xy * RENDERSIZE.xy;
	uv = uv / RENDERSIZE.xx*(4.+scale*35.);
    float d = uv.y;
    float a = atan(uv.y,uv.x)+(sin(d*.3+i*.3)*(1.0/RENDERSIZE.y*2.)+i*.2);
    d = pow(d,1.5);    

    float j = mod(i*0.4,3.14);

    float f;
    f = ((abs(mod(uv.x,1.0)-.5)-.45)*boarder_amp) * line_amp;
    f = max(f, (abs(mod(uv.y,0.5)-.25)-.2)*boarder_amp) * line_amp;
    
    f = mix(f,max(f, (abs(mod(uv.y+uv.x*1.5,1.0)-.5)-.4)*boarder_amp),diag_amp);
    f = mix(f,max(f, (abs(mod(uv.y+uv.x*-1.5,1.0)-.5)-.4)*boarder_amp),diag_amp);
    
    vec4 c = vec4(0.0,0.0,0.0,1.0);
    c.r = f;
    c.b = cos(f+sin(i))*.5+.5;
    c.g = abs(f);
    #ifdef bw
    	c.rgb = vec3(f);
    #endif
    
    float s = i+d*(color_iter*.50);
    c = hue(c,s);
   	gl_FragColor = c;
}

