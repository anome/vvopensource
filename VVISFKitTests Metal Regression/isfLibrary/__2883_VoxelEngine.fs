/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "Generator",
    "voxels"
  ],
  "DESCRIPTION" : "Simple to reuse, fast voxel engine.",
  "INPUTS" : [
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		0.0,
		"MAX" : 		3.0
	},
	{
		"NAME" : 		"rot",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.025,
		"MIN" : 		-0.25,
		"MAX" : 		0.25
	},	
	{
		"NAME" : 		"light",
		"TYPE" : 		"point2D",
		"DEFAULT" :		[ -0.3, -0.2 ],
		"MAX" : 		[ 1.0, 1.0 ],
     	"MIN" : 		[ -1.0, -1.0]
	},
 	{
      	"NAME" : 		"tilt",
      	"TYPE" : 		"float",
      	"MIN" :			1.01,
      	"MAX" :			1.99,
      	"DEFAULT" : 	1.88
    },	
  	{
      	"NAME" : 		"dof",
      	"TYPE" : 		"float",
      	"MIN" :			0.01,
      	"MAX" :			5.0,
      	"DEFAULT" : 	0.88
    },
  	{
      	"NAME" : 		"zoom",
      	"TYPE" : 		"float",
      	"MIN" :			6.0,
      	"MAX" :			50.0,
      	"DEFAULT" : 	30.0
    },  
  	{
      	"NAME" : 		"grow",
      	"TYPE" : 		"float",
      	"MIN" :			0.0,
      	"MAX" :			8.0,
      	"DEFAULT" : 	4.5
    },
  	{
      	"NAME" : 		"seed1",
      	"TYPE" : 		"float",
      	"MIN" :			1.0,
      	"MAX" :			24.0,
      	"DEFAULT" : 	6.84
    },    
    {
      	"NAME" : 		"seed2",
      	"TYPE" : 		"float",
      	"MIN" :			-12.0,
      	"MAX" :			12.0,
      	"DEFAULT" : 	-7.16    
    },
    {
      	"NAME" : 		"seed3",
      	"TYPE" : 		"float",
      	"MIN" :			0.0,
      	"MAX" :			1.0,
      	"DEFAULT" : 	0.53   
    },
    {
		"NAME" : 		"color",
		"TYPE" : 		"float",
		"DEFAULT" : 	8.65,
		"MIN" : 		-8.0,
		"MAX" : 		16.0
	},
	{
		"NAME" : 		"loops",
		"TYPE" : 		"float",
		"DEFAULT" : 	23.0,
		"MIN" : 		8.0,
		"MAX" : 		80.0
	},
	{
      	"NAME" : 		"c1",
      	"TYPE" : 		"color",
      	"DEFAULT" :	[ 1.0, 0.0, 0.0, 1.0 ]
   	},
   	{
      	"NAME" : 		"c2",
      	"TYPE" : 		"color",
      	"DEFAULT" :	[ 0.7, 1.0, 0.1, 1.0 ]
   	}
  ],
  "ISFVSN" : 2.0
}
*/

////////////////////////////////////////////////////////////////////
// VoxelEngine  by mojovideotech
//
// based on :
// shadertoy.com/view/4tlfDn
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////

#define 	twpi  	6.2831853  	// two pi, 2*pi

const vec3 backgroundColor = vec3(0.0, 0.0, 0.0);
const float shadow = 0.5;

float setCamera(out vec3 eye, out vec3 center) {
    vec2 m = vec2(rot * TIME, 0.5);
    m *= twpi * vec2(1.0, tilt);    
    center = vec3(0.0);
    float D = 50.0 - zoom;
    eye = center + vec3(D * sin(m.x) * sin(m.y), D * cos(m.x) * sin(m.y), D * cos(m.y));
    return dof;
}

bool voxelHit(vec3 pos) {
    vec3 hash = fract(pos * vec3(28.657, 51.4229, 1.597));
    hash = mix(hash, dot(hash.zxy, hash.yzx)-hash, seed3);
    return length(pos) + seed2 * fract((hash.x + hash.y) * hash.z) < seed1 + grow * sin(TIME * rate);
}

vec3 voxelColor(vec3 pos, vec3 norm) { return mix(c1.rgb, c2.rgb, (length(floor(pos*fract(seed2))) - color)/seed1); }

float castRay(vec3 eye, vec3 ray, out float dist, out vec3 norm) {
    vec3 pos = floor(eye);
    vec3 ri = 1.0 / ray;
    vec3 rs = sign(ray);
    vec3 ris = ri * rs;
    vec3 dis = (pos - eye + 0.5 + rs * 0.5) * ri;
    vec3 dim = vec3(0.0);
    float II = 0.0;
    for (int i = 0; i < 80; ++i) {
    	if(II>=loops) { break; }
        if (voxelHit(pos)) {
            dist = dot(dis - ris, dim);
            norm = -dim * rs;
            return 1.0;
        }
        dim = step(dis, dis.yzx);
		dim *= (1.0 - dim.zxy);
        dis += dim * ris;
        pos += dim * rs;
        II += 1.0;
    }
	return 0.0;
}

void main() {
	float dist;
    vec3 eye, center, norm;
    float zoom = setCamera(eye, center);
    vec3 lightDir = vec3(light.xy, 0.8);
    vec3 forward = normalize(center - eye);
    vec3 right = normalize(cross(forward, vec3(0.0, 0.0, 1.0)));
    vec3 up = cross(right, forward);
    vec2 xy = 2.0 * gl_FragCoord.xy - RENDERSIZE.xy;
    vec3 ray = normalize(xy.x * right + xy.y * up + zoom * forward * RENDERSIZE.y);
    float hit = castRay(eye, ray, dist, norm);
    vec3 pos = eye + dist * ray;
    vec3 col = voxelColor(pos - 0.001 * norm, norm);
    float shade = dot(norm, lightDir);
    float illuminated = 1.0 - castRay(pos + 0.001 * norm, lightDir, dist, norm);
    float light = (1.0 + shadow * (illuminated * max(shade, 0.0) - 1.0)) * (1.0 - max(-shade, 0.0));

    gl_FragColor = vec4(mix(backgroundColor, light * col, hit), 1.0);
}
