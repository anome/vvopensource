/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"SDF"
	],
	"INPUTS": [
		{
		"NAME": 	"distance",
		"TYPE": 	"float",
		"DEFAULT":	60.0,
		"MIN": 		10.0,
		"MAX":	 	200.0
		},
		{
		"NAME": 	"lightdistance",
		"TYPE": 	"float",
		"DEFAULT": 	0.0,
		"MIN": 		-100.0,
		"MAX": 		100.0
		},
		{
		"NAME" : 	"lightposition",
		"TYPE" : 	"point2D",
		"DEFAULT" :	[ 10.0, 10.0 ],
		"MAX" : 	[ 100.0, 100.0 ],
     	"MIN" : 	[ -10.0, -10.0 ]
		},
		{
		"NAME" : 		"camposition",
		"TYPE" : 		"point2D",
		"DEFAULT" :		[ 0.5, 0.25 ],
		"MAX" : 		[ 1.0, 1.0 ],
     		"MIN" : 	[ 0.0, 0.0 ]
		},
		{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.25,
		"MIN" : 		-1.5,
		"MAX" : 		1.5
		},
		{
      	"NAME" : 	"numballs",
      	"TYPE" : 	"float",
      	"DEFAULT" :	0.0,
      	"MIN" : 	0.0,
      	"MAX" : 	3.0
    	},
    	{
      	"NAME" : 	"ballSize",
      	"TYPE" : 	"float",
      	"DEFAULT" : 0.0,
      	"MIN" : 	0.0,
      	"MAX" : 	6.0
    	},
		{
      	"NAME" : 	"numboxes",
      	"TYPE" : 	"float",
      	"DEFAULT" :	5.0,
      	"MIN" : 	1.0,
      	"MAX" : 	9.0
    	},
		{
      	"NAME" : 	"boxSize",
      	"TYPE" : 	"float",
      	"DEFAULT" : 0.25,
      	"MIN" : 	0.1,
      	"MAX" : 	1.0
    	},
		{
      	"NAME" : 	"baseX",
      	"TYPE" : 	"float",
      	"DEFAULT" :	2.0,
      	"MIN" : 	0.0,
      	"MAX" : 	6.0
    	},
    	{
      	"NAME" : 	"baseY",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	2.0,
      	"MIN" : 	0.0,
      	"MAX" : 	6.0
    	},
    	{
      	"NAME" : 	"baseZ",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	2.45,
      	"MIN" : 	0.0,
      	"MAX" : 	6.0
    	},
    	{
      	"NAME" : 	"multX",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	7.5,
      	"MIN" : 	1.0,
      	"MAX" : 	24.0
    	},
    	{
      	"NAME" : 	"multY",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	4.0,
      	"MIN" : 	1.0,
      	"MAX" : 	6.0
    	},
	{
      	"NAME" : 	"multZ",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	9.0,
      	"MIN" : 	1.0,
      	"MAX" : 	24.0
    	},
    	{
      	"NAME" : 	"sinX",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	2.125,
      	"MIN" : 	1.0,
      	"MAX" : 	6.0
    	},
    	{
      	"NAME" : 	"sinY",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	3.5,
      	"MIN" : 	1.0,
      	"MAX" : 	6.0
    	},
    	{
      	"NAME" : 	"sinZ",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	1.75,
      	"MIN" : 	1.0,
      	"MAX" : 	6.0
    	},
    	{
      	"NAME" : 	"cosX",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	5.25,
      	"MIN" : 	1.0,
      	"MAX" : 	6.0
    	},
    	{
      	"NAME" : 	"cosY",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	5.15,
      	"MIN" : 	1.0,
      	"MAX" : 	6.0
    	},
    	{
      	"NAME" : 	"cosZ",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	2.0,
      	"MIN" : 	1.0,
      	"MAX" : 	6.0
    	},
    	    	{
      	"NAME" : 		"c1",
      	"TYPE" : 		"color",
      	"DEFAULT" :	[ 0.9, 0.2, 0.2, 1.0 ]
    	},
    	{
      	"NAME" : 		"c2",
      	"TYPE" : 		"color",
      	"DEFAULT" :	[ 0.3, 0.2, 0.9, 1.0 ]
    	},
    	{
      	"NAME" : 		"c3",
      	"TYPE" : 		"color",
      	"DEFAULT" :	[ 0.4, 0.9, 0.2, 1.0 ]
    	}
    	
	]
}*/

////////////////////////////////////////////////////////////
// DFPlayground  by mojovideotech
//
// based on :
// shadertoy.com/\ldVGRV
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	pi   	3.141592653589793 	// pi

const int renderDepth = 300;
const bool showRenderDepth = false;

const vec3 background = vec3(0.2, 0.2, 0.2);
const vec3 black = vec3(0, 0, 0);
vec3 lightPos = vec3(lightdistance, lightposition.x, lightposition.y);
const float a = 1.0;
const float b = 3.0;
vec3 forces[3];


vec3 getRayDir(vec3 camDir, vec2 fragCoord) {
  vec3 yAxis = vec3(0, 1, 0);
  vec3 xAxis = normalize(cross(camDir, yAxis));
  vec2 q = fragCoord / RENDERSIZE.xy;
  vec2 p = 2.0 * q - 1.0;
  p.x *= RENDERSIZE.x / RENDERSIZE.y;
  return normalize(p.x * xAxis + p.y * yAxis + 5.0 * camDir);
}

// http://iquilezles.org/www/articles/distfunctions/distfunctions.htm
float sdBox(vec3 p, vec3 b) {
  vec3 d = abs(p) - b;
  return min(max(d.x, max(d.y, d.z)), 0.0) + length(max(d, 0.0)) - 0.1;
}

float sdPlane(vec3 p, vec4 n) {
  return dot(p,n.xyz) + n.w;
}

float getMetaball(vec3 p, vec3 v) {
  float r = length(p - v);
  if (r < b / 3.0) {
    return a * (1.0 - 3.0 * r * r / b * b);
  } else if (r < b) {
    return (3.0 * a / 2.0) * (1.0 - r / b) * (1.0 - r / b);
  } else {
    return 0.0;
  }
}

float sdImplicitSurface(vec3 p) {
  float mb = 0.0;
  float minDist = 10000.0;
  float ballcount = 1.0;
  for (int i = 0; i < 3; i++) {
  	if (ballcount>numballs) break;
    mb += getMetaball(p, forces[i]);
    minDist = min(minDist, length(p - forces[i]));
    ballcount += 1.0;
  }
  if (minDist > b) {
    return max (minDist - b, b - 1.2679529);
  } else if (mb == 0.0) {
    return b - 1.2679529;  // 1.2679529 is the x-intercept of the metaball expression - 0.5.
  } else {
    return b - sqrt((5.0  + ballSize) * mb) - 1.2679529;
  }
}

float getSdf(vec3 p) {
  float f = sdImplicitSurface(p);
  float boxcount = 1.0;
  for (int i = 0; i < 9; i++) {
  	if(boxcount>numboxes) break;
    float t = float(i) + TIME * rate;
    f = min(f, sdBox(
        p - vec3(
            baseX + multX * sin(t * pi / sinX) * cos(t * pi / sinX), 
            baseY + multY * sin(t * pi / sinY) * cos(t * pi / sinY), 
            baseZ + multZ * sin(t * pi / sinZ) * cos(t * pi / sinZ)),
        vec3(boxSize)));
   	boxcount += 1.0;
  }
  return f;
}

float getSdfWithPlane(vec3 p) {
  return min(getSdf(p), sdPlane(p, vec4(0,1,0,1)));
}

float diffuse(vec3 point,vec3 normal) {
  return clamp(dot(normal, normalize(lightPos - point)), 0.0, 1.0);
}

float getShadow(vec3 pt) {
  vec3 lightDir = normalize(lightPos - pt);
  float kd = 1.0;
  int step = 0;
  float t = 0.1;

  for (int step = 0; step < renderDepth; step++) {
    float d = getSdf(pt + t * lightDir);
    if (d < 0.001) {
      kd = 0.0;
    } else {
      kd = min(kd, 16.0 * d / t);
    }
    t += d;
    if (t > length(lightPos - pt) || step >= renderDepth || kd < 0.001) {
      break;
    }
  }
  return kd;
}

vec3 getGradient(vec3 pt) {
  return vec3(
    getSdfWithPlane(vec3(pt.x + 0.0001, pt.y, pt.z)) - getSdfWithPlane(vec3(pt.x - 0.0001, pt.y, pt.z)),
    getSdfWithPlane(vec3(pt.x, pt.y + 0.0001, pt.z)) - getSdfWithPlane(vec3(pt.x, pt.y - 0.0001, pt.z)),
    getSdfWithPlane(vec3(pt.x, pt.y, pt.z + 0.0001)) - getSdfWithPlane(vec3(pt.x, pt.y, pt.z - 0.0001)));
}

vec3 getDistanceColor(vec3 pt) {
  float d = getSdf(pt);
  vec3 color = mix(c1.rgb, c2.rgb, 0.5 + 0.5 * sin(d * 3.141592));
  if (fract(d) < 0.05) {
    color = mix(color, black, smoothstep(0.0, 0.05, fract(d)));
  } else if (fract(d) < 0.1) {
    color = mix(black, color, smoothstep(0.05, 0.1, fract(d)));
  }
  return color;
}

vec3 illuminate(vec3 pt) {
  vec3 color = (abs(pt.y + 1.0) < 0.001) ? getDistanceColor(pt) : c3.rgb;
  vec3 gradient = getGradient(pt);
  float diff = diffuse(pt.xyz, normalize(gradient));
  return (0.25 + diff * getShadow(pt))  * color;
}

vec3 raymarch(vec3 rayorig, vec3 raydir) {
  vec3 pos = rayorig;
  float d = getSdfWithPlane(pos);
  int work = 0;

  for (int step = 0; step < renderDepth; step++) {
    work++;
    pos = pos + raydir * d;
    d = getSdfWithPlane(pos);
    if (abs(d) < 0.001) {
      break;
    }
  }

  return showRenderDepth
    ? vec3(float(work) / float(renderDepth))
    : (abs(d) < 0.001) 
      ? illuminate(pos)
      : background;
}

void main() {
  forces[0] = vec3(-3, 0, 0);
  forces[1] = vec3(3.0 * sin(TIME), 4.0 * abs(cos(TIME)), 0.0);
  forces[2] = vec3(3, 0, 0);

  vec3 camPos = distance * vec3(camposition.x, camposition.y, 0.5);
  vec3 camDir = normalize(-camPos);
  gl_FragColor = vec4(raymarch(camPos, getRayDir(camDir, gl_FragCoord.xy)), 1.0);
}

