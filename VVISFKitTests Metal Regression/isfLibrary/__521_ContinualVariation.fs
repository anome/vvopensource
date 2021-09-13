/*{
  "CREDIT": "by mojovideotech, Enhanced by zerbzman.",
  "CATEGORIES": [
    "Generator"
  ],
  "INPUTS": [
    {
      "NAME": "speed",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "rotation",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "frontColor",
      "LABEL": "Front Color",
      "TYPE": "color",
      "DEFAULT": [
        0.2,
        0,
        1,
        1
      ]
    },
    {
      "NAME": "backColor",
      "LABEL": "Back Color",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        0,
        1
      ]
    },
    {
      "NAME": "invert",
      "LABEL": "Invert",
      "TYPE": "bool",
      "DEFAULT": false
    },
    {
      "NAME": "brightness",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "contrast",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": -4,
      "MAX": 4
    },
    {
      "NAME": "hue",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "saturation",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": -1,
      "MAX": 4
    },
    {
      "NAME": "pos",
      "TYPE": "point2D",
      "DEFAULT": [
        0.5,
        0.5
      ],
      "MIN": [
        0,
        0
      ],
      "MAX": [
        1,
        1
      ]
    }
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#32770.3. Work in Progress."
}*/

// ContinuaVariation by mojovideotech
// glslsandbox.com/e#32770.3

// ======================================
// Corrente Continua - 1k intro (win32)
// Zerothehero of Topopiccione
// 25/jul/2012
//@zerothehero: better formulas, visually improved
// mod by mojovideotech
// ======================================

#ifdef GL_ES
precision mediump float;
#endif

const float di = 0.5772156649;
const float dh = 0.69314718;
const float twpi = 6.2831853;
	
mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}

vec3 rgb2hsv(vec3 c)	{
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	//vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
	//vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
	vec4 p = c.g < c.b ? vec4(c.bg, K.wz) : vec4(c.gb, K.xy);
	vec4 q = c.r < p.x ? vec4(p.xyw, c.r) : vec4(c.r, p.yzx);
	
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)	{
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main( void ) 
{
	float t = TIME * 12. * speed;
	float tt = TIME * 0.005 * speed;
	vec2 p = ((gl_FragCoord.xy / RENDERSIZE.xy ) - 0.5) * 27.;
	p.x *= RENDERSIZE.x / RENDERSIZE.y;
	float a=0., b=0., c=0., d=0., e=0.;
	for (int i=-4; i<4; i++) {
		p = rotate2d(tt*-twpi * rotation)*p;
		float x = (p.x*di-p.y*dh*0.125);
		float y = (p.x*di*0.125+p.y*dh);
		c = (sin(x + t * (float(i)) / 18.0) + b + y + 4.0);
		d = (cos(y+t*(float(i))/20.0)+x+a+3.0);
		e = (sin(y+t*(float(i))/17.5)+x-e+1.0);
		a -= .25/(c*c);
		b += .5/(d*d);
		e += .125;
	}
	vec4		tmpColorB;
	float red = mix(backColor.r, frontColor.r, log(-e + a + b - 1.) / 8. - 0.2);
	float green = mix(backColor.g, frontColor.g, log(-e - a - b - 1.) / 8. - 0.2);
	float blue = mix(backColor.b, frontColor.b, log(e - a + b - 1.) / 5. - 0.1);
// 	vec4 tmpColorA = vec4(log(-e+a+b-1.)/8.-0.2,log(-e-a-b-1.)/8.-0.2,log(e-a+b-1.)/5.-0.1, 1.0) ;
    
    if (invert) {
        red = 1. - red;
        green = 1. - green;
        blue = 1. - blue;
    }
    
    vec4 tmpColorA = vec4(red, green, blue, 1.0);
    // gl_FragColor = blue;
    
    //	bright
	tmpColorB = tmpColorA + vec4(brightness, brightness, brightness, 0.0);
	//	contrast
	tmpColorA.rgb = ((vec3(2.0) * (tmpColorB.rgb - vec3(0.5))) * vec3(contrast) / vec3(2.0)) + vec3(0.5);
	tmpColorA.a = ((2.0 * (tmpColorB.a - 0.5)) * abs(contrast) / 2.0) + 0.5;
	
	//	convert RGB to HSV
	tmpColorB.xyz = rgb2hsv(clamp(tmpColorA.rgb, 0.0, 1.0));
	tmpColorB.a = tmpColorA.a;
	
	
	//	hue
	tmpColorB.x = mod((tmpColorB.x + hue), 1.0);
	//	saturation
	tmpColorB.y = tmpColorB.y * saturation;
	
	
	//	convert HSV back to RGB
	tmpColorA.rgb = hsv2rgb(clamp(tmpColorB.xyz, 0.0, 1.0));
	tmpColorA.a = tmpColorB.a;
	
	
	gl_FragColor = clamp(tmpColorA, 0.0, 1.0);
}