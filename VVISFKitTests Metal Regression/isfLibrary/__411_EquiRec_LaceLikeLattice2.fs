/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "Spherical / Equirectangular version of LaceLikeLattice2 - based on www.shadertoy.com/view/XdSSz1",
  "CATEGORIES": [
    "2d",
    "iterations",
    "fractal"
  ],
  "INPUTS": [
    {
      	"NAME": "center",
      	"TYPE": "point2D",
		"DEFAULT" :	[ 0, 0 ],
		"MAX" : 	[ 2.0, 2.0 ],
      	"MIN" :  	[ -1.0, -1.0 ]
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.75,
      "MIN": -3,
      "MAX": 3
    },
    {
      "NAME": "vectorX",
      "TYPE": "float",
      "DEFAULT": 3,
      "MIN": 0,
      "MAX": 6.2831853
    },
    {
      "NAME": "vectorY",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": -0.6283185,
      "MAX": 0.6283185
    },
    {
      "NAME": "rot",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 6.2831853
    },
    {
      "NAME": "width",
      "TYPE": "float",
      "DEFAULT": 0.075,
      "MIN": 0.015,
      "MAX": 1.5
    },
    {
      "NAME": "push",
      "TYPE": "float",
      "DEFAULT": 5,
      "MIN": -10,
      "MAX": 10
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": -16,
      "MIN": -200,
      "MAX": 200
    },
    {
      "NAME": "detail",
      "TYPE": "float",
      "DEFAULT": 16,
      "MIN": 0,
      "MAX": 19.9
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 31,
      "MIN": 7,
      "MAX": 36
    },
    {
      "NAME": "color",
      "TYPE": "float",
      "DEFAULT": 0.75,
      "MIN": 0.025,
      "MAX": 1
    },
    {
      "NAME": "hue",
      "TYPE": "float",
      "DEFAULT": 7,
      "MIN": 1,
      "MAX": 10
    }
  ]
}*/


////////////////////////////////////////////////////////////
// EquiRec_LaceLikeLattice2  by mojovideotech
//
// Spherical / Equirectangular version of 
// interactiveshaderformat.com/sketches/1281
// 
// based on : String Theory by nimitz 
// www.shadertoy.com/\view/\XdSSz1
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////



#define 	pi   	3.141592653589793  // pi


float hash (vec2 xy) { return (log(pow(xy.x,xy.y)*tan(317811.0)));}

mat2 mm2(in float a){float c = cos(a), s = sin(a); return mat2(c,-s,s,c);}

float f(vec2 p, float t) {
	p.y = sin(p.y*1.+t*1.23)*cos(t+p.y*0.05);	
    p += sin(p.y*0.05)*hash(p);
    return smoothstep(-0.01,width,abs(p.x));
}

void main() {
	float time = TIME * rate;
	vec2 uv = (isf_FragNormCoord.xy);
	float th =  uv.y * pi, ph = uv.x * 2.0 * pi;
	vec2 p = vec2(sin(th) * cos(ph), sin(th) * sin(ph));  //, cos(th));
    p += -center.xy;
	p *= mm2(rot)*zoom;	
	vec3 col = vec3(0.0);
	float counter = depth;
	float k = hash(p);
	for(float i=0.;i<36.;i++) {
        p.y -= 20.0 - detail;
        p.x -= sin(time*0.125+push)*1.5+1.5;
		p*= mm2(i*vectorY+vectorX);
        vec2 pa = vec2(abs(p.y-1.5),abs(p.x));
        vec2 pb = vec2(p.x,abs(p.y));
        p = mix(pa,pb,smoothstep(0.5,.1,abs(sin(time*0.05)+.5)));
        vec3 col2 = (sin(vec3(13.0*hue,21.,34.0/hue)+i*color)*color+(1.0-color)+0.125)*(1.-f(p,time));
		col = max(col,col2);
        counter -= 1.0;
        if (counter<1.0) { break; }
	}
	
	gl_FragColor = vec4(col,1.0);
}