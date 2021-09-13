/*
	{
	"DESCRIPTION": "Christmas Tree",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "ISF Import by: Old Salt",
	"VSN": "1.0",
	"INPUTS":
		[
			{
			"NAME": "uC1",
			"TYPE": "color",
			"DEFAULT":[0.0,1.0,0.0,1.0]
			},
			{
			"NAME": "uC2",
			"TYPE": "color",
			"DEFAULT":[0.0,0.0,1.0,1.0]
			},
			{
			"NAME": "uC3",
			"TYPE": "color",
			"DEFAULT":[1.0,0.0,0.0,1.0]
			},
			{
			"LABEL": "Offset: ",
			"NAME": "uOffset",
			"TYPE": "point2D",
			"MAX": [1.0,1.0],
			"MIN": [-1.0,-1.0],
			"DEFAULT": [0.0,0.0]
			},
			{
			"LABEL": "Zoom: ",
			"NAME": "uZoom",
			"TYPE": "float",
			"MAX": 10.0,
			"MIN": 0.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Rotation(or R Speed):",
			"NAME": "uRotate",
			"TYPE": "float",
			"MAX": 180.0,
			"MIN": -180.0,
			"DEFAULT": 0.0
			},
			{
			"LABEL": "Continuous Rotation? ",
			"NAME": "uContRot",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "Color Mode: ",
			"LABELS":
				[
				"Shader Defaults ",
				"Alternate Color Palette (3 used) "
				],
			"NAME": "uColMode",
			"TYPE": "long",
			"VALUES": [0,1],
			"DEFAULT": 0
			},
			{
			"LABEL": "Intensity: ",
			"NAME": "uIntensity",
			"TYPE": "float",
			"MAX": 4.0,
			"MIN": 0,
			"DEFAULT": 1.0
			}
		]
	}
*/
// Import from: http://glslsandbox.com/e#71522.0

#define PI 3.141592653589
#define size 0.1e-4
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))

// Functions from import:
float grow(vec2 uv,float growWidth,float filmWidth)
	{
	if(abs(uv.y) > growWidth)return 0.;
	float y = uv.y / growWidth;
	float d = abs(uv.x / filmWidth);
	return sqrt(1. - y * y) / (d) * .1;
	}

float star(vec2 uv,float growWidth,float filmWidth)
	{
	vec2 rotUV1 = mat2(
		cos(PI / 3.),-sin(PI / 3.),
		sin(PI / 3.),cos(PI / 3.)
    ) * uv;
	vec2 rotUV2 = mat2(
		cos(PI / 3.),sin(PI / 3.),
		-sin(PI / 3.),cos(PI / 3.)
	) * uv;
	return grow(uv,growWidth,filmWidth) + grow(rotUV1,growWidth,filmWidth) +grow(rotUV2,growWidth,filmWidth) ;
	}

vec3 hsv2rgb(vec3 c)
	{
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
	}

float lineDist(vec2 a, vec2 b, vec2 p)
	{
	vec2 v = a, w = b;
	float l2 = pow(distance(w, v), 2.);
	if(l2 == 0.0) return distance(p, v);
	float t = clamp(dot(p - v, w - v) / l2, 0., 1.);
	vec2 j = v + t * (w - v);
	return distance(p, j);
	}

vec3 addVertex(vec4 vertex, vec3 color, mat4 projMat, vec2 camUV)
	{
	vec4 p_proj1 = projMat * vertex;
	vec2 p = p_proj1.xy / p_proj1.z;
	float dist = length((camUV-vec2(.5))-p);
	return color * star(camUV-vec2(.5)-p.xy,.01,.2);
	}

vec3 addLine(vec4 vertex1, vec4 vertex2, vec3 color, mat4 projMat, vec2 camUV)
	{
	vec4 p_proj1 = projMat * vertex1;
	vec2 p1 = p_proj1.xy / p_proj1.z;
	vec4 p_proj2 = projMat * vertex2;
	vec2 p2 = p_proj2.xy / p_proj2.z;
	float dist = lineDist((camUV-vec2(.5))-p1, (camUV-vec2(.5))-p2, vec2(0., 0.0));
	return color * 1. / pow(dist, 2.) * size;
	}

void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5; // normalize coordinates
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;          // correct aspect ratio
	uv = (uv-uOffset) * 1.0/uZoom;              // zoom at original location, then offset result
	uv = uContRot ? uv*rotate2D(TIME*uRotate/36.0) : uv*rotate2D(uRotate*PI/180.0);
	uv.x *= RENDERSIZE.y/RENDERSIZE.x;
  uv = (uv + 0.5)	* RENDERSIZE;								

/**** Start of Imported Shader Code main() *****/
	uv = (uv - vec2(RENDERSIZE.x * 0.25, 0.0)) / RENDERSIZE.y;
	/**** Start of Core Imported Shader Code *****/
	float theta = TIME*0.2;

	// standard rotation matrix around Y axis.
	mat4 projMat = mat4(
		vec4(cos(theta), 0.0, sin(theta), 0.0),
		vec4(0.0, 1.0, 0.0, 0.0),
		vec4(-sin(theta), 0.0, cos(theta), 0.0),
		vec4(0.0, 0.0, 1.0, 0.0));

	vec3 imageColors = vec3(0.);
	vec3 green = vec3(0.0, 0.2, 0.);
	vec3 brown = vec3(0.5, 0.1, 0.);
	float height = .7;
	float width = .8;
	float trunkThickness = 0.05;

	// tree points
	imageColors += addVertex(vec4(width * 0.0, -.3 + height * 1.0, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(width * 0.12, -.3 + height * 0.8, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(width * 0.02, -.3 + height * 0.8, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(width * 0.24, -.3 + height * 0.5, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(width * 0.08, -.3 + height * 0.5, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(width * 0.36, -.3 + height * 0.2, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(width * trunkThickness, -.3 + height * 0.2, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(width * trunkThickness, -.3 + height * 0.0, 0., 1.), brown, projMat, uv);
	imageColors += addVertex(vec4(-width * 0.12, -.3 + height * 0.8, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(-width * 0.02, -.3 + height * 0.8, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(-width * 0.24, -.3 + height * 0.5, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(-width * 0.08, -.3 + height * 0.5, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(-width * 0.36, -.3 + height * 0.2, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(-width * trunkThickness, -.3 + height * 0.2, 0., 1.), green, projMat, uv);
	imageColors += addVertex(vec4(-width * trunkThickness, -.3 + height * 0.0, 0., 1.), brown, projMat, uv);

	// light points
	imageColors += addVertex(vec4(width * 0.02, -.3 + height * 0.85, 0., 1.), hsv2rgb(vec3(fract(TIME*0.05), 1., 1.)), projMat, uv);
	imageColors += addVertex(vec4(-width * 0.05, -.3 + height * 0.65, 0., 1.), hsv2rgb(vec3(fract(TIME*0.071+.5), 1., 1.)), projMat, uv);
	imageColors += addVertex(vec4(width * 0.07, -.3 + height * 0.58, 0., 1.), hsv2rgb(vec3(fract(TIME*0.066+.2), 1., 1.)), projMat, uv);
	imageColors += addVertex(vec4(-width * 0.081, -.3 + height * 0.35, 0., 1.), hsv2rgb(vec3(fract(TIME*0.062+.25), 1., 1.)), projMat, uv);
	imageColors += addVertex(vec4(width * 0.12, -.3 + height * 0.31, 0., 1.), hsv2rgb(vec3(fract(TIME*0.042+.87), 1., 1.)), projMat, uv);

	// tree outline connections
	imageColors += addLine(vec4(width * 0.0, -.3 + height * 1.0, 0., 1.), vec4(width * 0.12, -.3 + height * 0.8, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(width * 0.12, -.3 + height * 0.8, 0., 1.), vec4(width * 0.02, -.3 + height * 0.8, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(width * 0.02, -.3 + height * 0.8, 0., 1.), vec4(width * 0.24, -.3 + height * 0.5, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(width * 0.24, -.3 + height * 0.5, 0., 1.), vec4(width * 0.08, -.3 + height * 0.5, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(width * 0.08, -.3 + height * 0.5, 0., 1.), vec4(width * 0.36, -.3 + height * 0.2, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(width * 0.36, -.3 + height * 0.2, 0., 1.), vec4(width * trunkThickness, -.3 + height * 0.2, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(width * trunkThickness, -.3 + height * 0.2, 0., 1.), vec4(width * trunkThickness, -.3 + height * 0.0, 0., 1.), brown, projMat, uv);
	imageColors += addLine(vec4(-width * 0.0, -.3 + height * 1.0, 0., 1.), vec4(-width * 0.12, -.3 + height * 0.8, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(-width * 0.12, -.3 + height * 0.8, 0., 1.), vec4(-width * 0.02, -.3 + height * 0.8, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(-width * 0.02, -.3 + height * 0.8, 0., 1.), vec4(-width * 0.24, -.3 + height * 0.5, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(-width * 0.24, -.3 + height * 0.5, 0., 1.), vec4(-width * 0.08, -.3 + height * 0.5, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(-width * 0.08, -.3 + height * 0.5, 0., 1.), vec4(-width * 0.36, -.3 + height * 0.2, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(-width * 0.36, -.3 + height * 0.2, 0., 1.), vec4(-width * trunkThickness, -.3 + height * 0.2, 0., 1.), green, projMat, uv);
	imageColors += addLine(vec4(-width * trunkThickness, -.3 + height * 0.2, 0., 1.), vec4(-width * trunkThickness, -.3 + height * 0.0, 0., 1.), brown, projMat, uv);
	imageColors += addLine(vec4(width * trunkThickness, -.3 + height * 0.0, 0., 1.), vec4(-width * trunkThickness, -.3 + height * 0.0, 0., 1.), brown, projMat, uv);
/****    End of Imported Shader main()     *****/

	vec4 cShad = vec4(imageColors, 1.0);  
	vec3 cOut = cShad.rgb;
	if (uColMode == 1)
		{
		cOut = uC1.rgb * cShad.r;
		cOut += uC2.rgb * cShad.g;
		cOut += uC3.rgb * cShad.b;
		}
	cOut = cOut * uIntensity;
	cOut = clamp(cOut, vec3(0.0), vec3(1.0));
	gl_FragColor = vec4(cOut.rgb,cShad.a);
	}
	