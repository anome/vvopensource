/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
    ""
  ],
  "DESCRIPTION": "",
  "INPUTS": [
          {
            "NAME": "offsetx",
            "TYPE": "float",
           "DEFAULT": 5.0,
            "MIN": -12.0,
            "MAX": 12.0
          },
          {
            "NAME": "offsety",
            "TYPE": "float",
           "DEFAULT": 0.425,
            "MIN": -2.0,
            "MAX": 20.0
          },
          {
            "NAME": "offsetz",
            "TYPE": "float",
           "DEFAULT": 39.5,
            "MIN": 0.0,
            "MAX": 40.0
          },
           {
            "NAME": "warpx",
            "TYPE": "float",
           "DEFAULT": 4.0,
            "MIN": 1.0,
            "MAX": 40.0
          },
           {
            "NAME": "warpy",
            "TYPE": "float",
           "DEFAULT": 3.0,
            "MIN": 1.0,
            "MAX": 40.0
          },
          {
            "NAME": "warpz",
            "TYPE": "float",
           "DEFAULT": 1.0,
            "MIN": 1.0,
            "MAX": 20.0
          },
          {
            "NAME": "tint",
            "TYPE": "float",
           "DEFAULT": 0.0,
            "MIN": -0.6,
            "MAX": 0.6
         },
          {
            "NAME": "color",
            "TYPE": "float",
           "DEFAULT": 0.8,
            "MIN": 0.1,
            "MAX": 0.9
          },
           {
            "NAME": "color2",
            "TYPE": "float",
           "DEFAULT": 0.15,
            "MIN": 0.05,
            "MAX": 0.25
          },
           {
            "NAME": "density",
            "TYPE": "float",
           "DEFAULT": 4.67,
            "MIN": 0.1,
            "MAX": 50
          },
           {
            "NAME": "depth",
            "TYPE": "float",
           "DEFAULT": 1.33,
            "MIN": 1.0,
            "MAX": 5.0
           },
           {
            "NAME": "freq",
            "TYPE": "float",
           "DEFAULT": 5.5,
            "MIN": 0.0,
            "MAX": 6.0
          },
           {
            "NAME": "multiplier",
            "TYPE": "float",
           "DEFAULT": 80,
            "MIN": 1,
            "MAX": 150
          },
           {
            "NAME": "rate",
            "TYPE": "float",
           "DEFAULT": 2.5,
            "MIN": -10.0,
            "MAX": 10.0
          },
          {
            "NAME": "horizon",
            "TYPE": "float",
           "DEFAULT": 4,
            "MIN": -10,
            "MAX": 100
        },
          {
            "NAME": "tilt",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": 0,
            "MAX": 1
        },
          {
            "NAME": "melt",
            "TYPE": "float",
           "DEFAULT": 2.5,
            "MIN": 0.0,
            "MAX": 5.0
          }
  ]
}
*/


// GlobularCluster by mojovideotech
// based on:
// http://glslsandbox.com/e#21070.1



#ifdef GL_ES
precision mediump float;
#endif

float m = TIME * rate;

float map(in vec3 p){
	vec3 q = mod(p + freq, 4.0) - 2.0;
	float d1 = length(q) - 1.0;
	d1 += 0.1*sin(warpx*p.x)*sin(warpy*p.y+1.0*m)*sin(warpz*p.z); 
	float d2 = mix(p.x,p.y,tilt) + horizon;
	float h = clamp(0.5 + 0.5*(d1-d2)/melt,0.0,1.0);
	return mix(d1,d2,h)-melt*h*(1.0-h);
}
vec3 calcNormal(in vec3 p){
	vec2 e = vec2(0.0001, 0.0);
	return normalize(vec3( 
					  map(p + e.xyy) - map(p - e.xyy),
					  map(p + e.yxy) - map(p - e.yxy),
					  map(p + e.yyx) - map(p - e.yyx)));
}
void main( void ) {

	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 p = -1.0 + 2.0*uv;
	p.x *= RENDERSIZE.x / RENDERSIZE.y;
	vec3 r0 = vec3(offsetx,offsety,offsetz);
	vec3 rd = normalize(vec3(p,-1.0));
	vec3 col = vec3(1.0);
	float tmax = multiplier;
	float h = 1.0;
	float t = 0.0;
	for (int i=0; i<150; i++){
		if (h < 0.0001 || t > tmax){ 
			break;
		}
		h = map(r0 + t*rd);
		t += h;
	}
	
	float f = 0.0;
	if (t < tmax){
		vec3 pos = r0 + t*rd;
		vec3 norm = calcNormal(pos);
		float s = pos.x + 0.5 * pos.y + 3.0 * pos.z;
		f = pos.z;
		col = vec3(mod(floor (s * density),depth+0.05));
	}
		
	gl_FragColor = vec4(vec3(color-tint, color2 * f/5.0, color+tint) - col, 1.0);
}