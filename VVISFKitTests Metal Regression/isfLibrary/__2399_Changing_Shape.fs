// SaturdayShader Week 26 : Soft Patterns
// by Joseph Fiola (http://www.joefiola.com)
// 2016-02-13

// Based on Interferance, Color Waves by @gabrieldunne
// https://twitter.com/gabrieldunne/status/671398225593561090
// http://glslsandbox.com/e#29006.1



/*{
  "CREDIT": "",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "Generator"
  ],
  "INPUTS": [
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 4,
      "MIN": 0,
      "MAX": 50
    },
    {
      "NAME": "iterations",
      "TYPE": "float",
      "DEFAULT": 10,
      "MIN": 0,
      "MAX": 10
    },
    {
      "NAME": "contrast",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -20,
      "MAX": 20
    },
    {
      "NAME": "offset",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "pattern",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "rotate",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "color1",
      "TYPE": "color",
      "DEFAULT": [
        1,
        0.4,
        0.64,
        1
      ]
    },
    {
      "NAME": "color2",
      "TYPE": "color",
      "DEFAULT": [
        0.3,
        0.1,
        0.14,
        1
      ]
    }
  ]
}*/



#define PI 3.14159
#define TWO_PI (PI*2.0)
#define HASHSCALE3 vec3(.1031, .1030, .0973)
#define HASHSCALE1 .1031


vec2 rot(vec2 uv,float a){
	return vec2(uv.x*cos(a) -uv.y*sin(a),uv.y*cos(a)+uv.x*sin(a));
}

float hash12(vec2 p)
{
	vec3 p3  = fract(vec3(p.xyx) * HASHSCALE1);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}

//----------------------------------------------------------------------------------------
//  1 out, 3 in...
float hash13(vec3 p3)
{
	p3  = fract(p3 * HASHSCALE1);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}
vec2 hash23(vec3 p3)
{
	p3 = fract(p3 * HASHSCALE3);
    p3 += dot(p3, p3.yzx+19.19);
    return fract(vec2((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y));
}
///  3 out, 3 in...
vec3 hash33(vec3 p3)
{
	p3 = fract(p3 * HASHSCALE3);
    p3 += dot(p3, p3.yxz+19.19);
    return fract(vec3((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y, (p3.y+p3.z)*p3.x));
}


float UDFatLineSegment (in vec2 coords, in vec2 A, in vec2 B, in float height)
{    
    // calculate x and y axis of box
    vec2 xAxis = normalize(B-A);
    vec2 yAxis = vec2(xAxis.y, -xAxis.x);
    float width = length(B-A);
    
	// make coords relative to A
    coords -= A;
    
    vec2 relCoords;
    relCoords.x = dot(coords, xAxis);
    relCoords.y = dot(coords, yAxis);
    
    // calculate closest point
    vec2 closestPoint;
    closestPoint.x = clamp(relCoords.x, 0.0, width);
    closestPoint.y = clamp(relCoords.y, -height * 0.5, height * 0.5);
    
    return length(relCoords - closestPoint);
}



float shape(vec2 uv) {
	float ret = 1.0;
	vec4 c_squiggleSize = vec4(1.0);
    float aspectRatio = RENDERSIZE.x / RENDERSIZE.y;
	
    // draw squiggle lines
    for (float i = 0.; i < 1.; ++i)
    {
        vec2 startuv = vec2(hash23(vec3(i, pattern, 2.635)));
        startuv.x *= aspectRatio;
        
        vec2 squiggleDir = normalize(hash23(vec3(i, pattern, 0.912)));
        
        vec3 color = hash33(vec3(i, TIME, 0.123));
        float width = mix(c_squiggleSize.x, c_squiggleSize.z, hash13(vec3(i, pattern, 0.342)));
        float height = mix(c_squiggleSize.y, c_squiggleSize.w, hash13(vec3(i, pattern, 1.847)));
		vec2 enduv = startuv + squiggleDir * width;
        
        vec2 uvOffset = vec2(0.0);
        float uvdp = dot(uv - startuv, squiggleDir);
        vec2 normal = vec2(squiggleDir.y, -squiggleDir.x);
        if (uvdp >= 0.0 && uvdp <= width)
            uv += normal * sin(uvdp * 100.0) * 0.01;
        
        float dist = UDFatLineSegment(uv, startuv, enduv, height);
        
        if (dist <= 0.0) {
            ret = 0.;
        }
    }        

    return ret;
}


void main() 
{
	vec2 center = (gl_FragCoord.xy);
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	uv -= vec2(0.5);
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;
	uv *= zoom;
	uv=rot(uv,rotate * PI);

	float col = contrast;

	for(float i = 0.; i < 10.0; i++) 
	{
	  	float a = i * 4. * (TWO_PI * pattern * 0.25);
		col += cos(TWO_PI*(uv.y * cos(a) + uv.x * sin(a) + offset)) +cos(TWO_PI*(uv.y * cos(a) + uv.x * sin(-a) + offset));
		
		if (i >= iterations) break;
		
	}
	
	col = shape(uv);
	
	gl_FragColor = col > 0.5 ? color1 : color2;
}