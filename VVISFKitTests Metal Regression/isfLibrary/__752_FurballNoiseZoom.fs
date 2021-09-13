/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "based on https://www.shadertoy.com/view/MlyGWR by ollj.",
  "CATEGORIES": [
    "2d",
    "fractal",
    "noise",
    "tunnel",
    "zoom"
  ],
  "ISFVSN": "2",
  "INPUTS": [
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": -3,
      "MAX": 3
    },
    {
      "NAME": "roughness",
      "TYPE": "float",
      "DEFAULT": 7,
      "MIN": 4,
      "MAX": 12
    },
    {
      "NAME": "layers",
      "TYPE": "float",
      "DEFAULT": 8,
      "MIN": 4,
      "MAX": 16
    },
    {
      "NAME": "pattern",
      "TYPE": "float",
      "DEFAULT": 73,
      "MIN": 1,
      "MAX": 200
    },
    {
      "NAME": "hashOps",
      "TYPE": "long",
      "VALUES": [
        1,
        2,
        3
      ],
      "LABELS": [
        "1",
        "2",
        "3"
      ],
      "DEFAULT": 1
    },
    {
      "NAME": "XY",
      "TYPE": "point2D",
      "DEFAULT": [
        0,
        0
      ],
      "MAX": [
        1,
        1
      ],
      "MIN": [
        -1,
        -1
      ]
    }
  ]
}*/

//////////////////////////////////////
// FurballNoiseZoom  by mojovideotech
//
// based on :
//
// shadertoy.com/MlyGWR  by ollj
// and
// shadertoy.com/Msf3Wr  by mu6k
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
////////////////////////////////////

float hash(float x)
{
	if (hashOps == 1) return fract(sin(x*9801.)*99.);	
	else if (hashOps == 2) return fract(sin(cos(x*99.)*99.)*99.);
	else if (hashOps == 3) return fract(sin(cos(x*12.13)*19.123)*17.321);
}

 float ss01(float x){return smoothstep(0.,1.,x);}

 float noise(vec2 p)
 {
	float z = floor(pattern);
 	vec2 m=fract(p),o=floor(p);o.y*=z;o.y+=o.x;
 	float a=hash(o.y);
 	float b=hash(o.y+1.);
 	float c=hash(o.y+z);
 	float d=hash(o.y+1.+z);
 	return mix(mix(a,b,ss01(m.x)),mix(c,d,ss01(m.x)),ss01(m.y));
 }

void main() {

 float o=roughness/sqrt(float(layers));
 float t=-2.*(TIME*rate)/o;
 vec2 v=(gl_FragCoord.xy/RENDERSIZE.x)+XY;
 v-=vec2(1.,RENDERSIZE.y/RENDERSIZE.x)*.5;
 float w=0.;
 for(int i=0;i<16;i++)
 {
  	float j=float(i);
  	float m=float(layers);
  	float u=mod(t+j,m);
  	float e=pow(o,u);
  	float l=u-t;
  	float z=u/m;
  	w-=noise(v*e+cos(vec2(l)))*(z-z*z);
  	if (m<=j) break;
 }
 w=mix(w+.7,w+1.5,float(layers)*.1-.3);
 gl_FragColor=vec4(vec3(w),1.);
	
}

