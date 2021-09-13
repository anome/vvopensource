/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [
    ""
  ],
   "ISFVSN" : "2",
  "INPUTS": [
  	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" : 	30.0,
		"MIN" : 		1.0,
		"MAX" : 		50.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	3.5,
		"MIN" : 		0.001,
		"MAX" : 		5.0
	},
	{
     	"NAME" :		"seed1",
     	"TYPE" : 		"float",
     	"DEFAULT" :		111,
     	"MIN" : 		55,
     	"MAX" :			233
	},
    {
      	"NAME" :		"seed2",
      	"TYPE" :		"float",
      	"DEFAULT" :		277,
      	"MIN" : 		98,
      	"MAX" :			337	
	},
    {
     	"NAME" :		"seed3",
      	"TYPE" :		"float",
     	"DEFAULT" :		497,
     	"MIN" :			301,
     	"MAX" :			579
    },
    {
      	"NAME" :		"depth",
      	"TYPE" :		"float",
      	"DEFAULT" :		234.0,
      	"MIN" :			5.0,
      	"MAX" :			500.0
    }
  ]
}*/

////////////////////////////////////////////////////////////
// ChromathematicalPoem#4  by mojovideotech
//
// based on
// glslsandbox.com/\e#28114.4
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#ifdef GL_ES
precision highp float;
#endif

float TT = TIME * rate;

float hash( float n ) {
	return fract((cos(n)*log2(TT))-(mod(n,-TT))*0.018181818);
}

float noise( in vec3 x ) {
    vec3 p = floor(x), f = fract(x);
    f = f*f*(3.0-2.0*f);
    float n = p.x + p.y*197.0 + 113.0*p.z;
    return mix(mix(mix( hash(n+0.0), hash(n+1.0),f.x),
                   mix( hash(n+197.0), hash(n+198.0),f.x),f.y),
               mix(mix( hash(n+113.0), hash(n+114.0),f.x),
                   mix( hash(n+310.0), hash(n+311.0),f.x),f.y),f.z);
}


float flux(vec3 mo) {
	float v = 0.0, m = 1.0;
	for (int i=0; i < 3; i ++) {
		m *= 0.497149872694134;
		v += noise(mo) * m;
		mo *= 1.1356352767379;
	}
	return v;
}

vec3 random3(vec3 c) {
	float j = 4231.0*sin(dot(c,vec3(seed1, seed2, seed3)));
	vec3 k;
	k.z = fract(seed1*j);
	j *= 0.5;
	k.x = fract(seed2*j);
	j *= 0.25;
	k.y = fract(seed3*j);
	return k-0.5;
}

const float F3 =  0.3333333;
const float G3 =  0.1666667;

float simplex3d(vec3 p) {
	 vec3 s = floor(p + dot(p, vec3(F3)));
	 vec3 x = p - s + dot(s, vec3(G3));
	 vec3 e = step(vec3(0.0), x - x.yzx);
	 vec3 i1 = e*(1.0 - e.zxy);
	 vec3 i2 = 1.0 - e.zxy*(1.0 - e);
	 vec3 x1 = x - i1 + G3;
	 vec3 x2 = x - i2 + 2.0*G3;
	 vec3 x3 = x - 1.0 + 3.0*G3;
	 vec4 w, d;
	 w.x = dot(x, x);
	 w.y = dot(x1, x1);
	 w.z = dot(x2, x2);
	 w.w = dot(x3, x3);
	 w = max(0.6 - w, 0.0);
	 d.x = dot(random3(s), x);
	 d.y = dot(random3(s + i1), x1);
	 d.z = dot(random3(s + i2), x2);
	 d.w = dot(random3(s + 1.0), x3);
	 w *= w;
	 w *= w;
	 d *= w;
	 return dot(d, vec4(depth));
}


float pallet(vec3 s) {
	float f = simplex3d(s);
	float g = flux(s);
	return floor(f*(3.0-2.0/g));
}

void main( void ) {
	vec2 uv  = isf_FragNormCoord*(50.5-scale);
	float f1 = pallet(vec3(uv, TT * 0.05 ));
	float f2 = pallet(vec3(uv, TT * 0.05 + 2.0));
	float f3 = pallet(vec3(uv, TT * 0.05 + 3.0));
	vec4 color  = (f1 < 3.0) ? vec4(0.194, 0.057, 0.295, 1.0) : vec4(0.0, 0.230, 0.837, 1.0);
	color += (f2 < 3.0) ? color : vec4(0.181, 0.073, 0.537, 1.0);
	color *= (f3 < 3.0) ? color : vec4(0.051, 0.120, 0.637, 1.0);

	gl_FragColor = color;
}