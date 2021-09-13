/*
	{
	"DESCRIPTION": "Import Wrapper",
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
			"MAX": 1.0,
			"MIN": -1.0,
			"DEFAULT": 0.0
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
// Import from: 

#define PI 3.141592653589
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))


void main()
	{
/*
 ***** zoom modifications for a range (-1 <- uZoom -> +1):
	// start with all lines to determine the needed values for ZPRESCALE and ZMAX
	// 1. initial scaling - set for good display valid (0 -> +ve)
	float ZPRESCALE = 1.0;  // applied to the entire range
	// 2. maximum zoom level - set to reasonable level valid (0 -> +ve)
	float ZMAX = 10.0;      // applied from 0 -> 1.0
	// 3. replace ZPRESCALE and ZMAX in the equation below and simplify it before publishing
	float zoom = (uZoom < 0.0) ? (1.0-abs(uZoom))*ZPRESCALE : (1.0+uZoom*ZMAX*(1.0-1.0/ZMAX))*ZPRESCALE;

	vec2 p;
	p = gl_FragCoord.xy / RENDERSIZE;                // normalize coordinates (origin at bottom left)
  p =(gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE;  // normalize coordinates (origin at center)

 ***** aspect ratio correction:
	p.x *= RENDERSIZE.x/RENDERSIZE.y;                // scales x axis to y, normally used with wide viewports
 	p *= RENDERSIZE/RENDERSIZE.y;                    // same as above
	p *= RENDERSIZE/min(RENDERSIZE.x, RENDERSIZE.y); // scales larger axis to the smaller

 ***** other common transformations:
	p -= uOffset;                                    // offset, then zoom does not affect the offset
	p /= zoom;                                      // zoom, then offset increases offset as well
	p *= rotate2D(radians); 												 // z axis rotation, see below for use with JSON above

	
 ***** most common, all on one line: (normalize, aspect correction, zoom, then offset result)
	vec2 p = ((gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE - uOffset)* RENDERSIZE/(RENDERSIZE.y * zoom);

 ***** second most common, all on one line: (normalize, aspect correction, offset, then zoom result)
	vec2 p =((gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE.y- uOffset)/zoom;  // normalize coordinates (origin at center)

	
 ***** add rotation at the center of the effect:
	vec2 p = ((gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE - uOffset)* RENDERSIZE/(RENDERSIZE.y * zoom);  // all on one line	p = uContRot ? p*rotate2D(TIME*uRotate/36.0) : p*rotate2D(uRotate*PI/180.0); // rotate 
	p = uContRot ? p*rotate2D(TIME*uRotate/36.0) : p*rotate2D(uRotate*PI/180.0); // rotate

 ***** rotate the effect around the center of the viewport:
	vec2 p = (gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE * RENDERSIZE/RENDERSIZE.y;   // normalize, correct aspect ratio
	p = uContRot ? p*rotate2D(TIME*uRotate/36.0) : p*rotate2D(uRotate*PI/180.0); // rotate
	p = (p - uOffset)/zoom;                      // apply offset and zoom
*/

/***** Start of Imported Shader Function main() *****/

/*
		This is the standard wrapper I use when importing shaders
		from other repositories such as the GLSLSandbox, and ShaderToy

		The code is modified to remove that not applicable to the
		shader.  JSON dict limits are changed as required.
*/
// placeholder code so this will put something in the viewport
	float zoom = (uZoom < 0.0) ? 1.0-abs(uZoom) : 1.0+uZoom*9.0;
	vec2 p = ((gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE - uOffset)* RENDERSIZE/(RENDERSIZE.y * zoom);  // all on one line
	p*=rotate2D(TIME/36.0);
#ifdef XL_SHADER
	p*=1000.;
#endif

	vec3 color = vec3(0.5) + 0.5*cos(TIME+p.xyx+vec3(1.0,2.0,3.0));

/*****   End of Imported Shader Function main() *****/

// Color replacement: change 'color' to whatever is used by the shader
// ie: for "gl_FragColor = vec4(color, 1.0);"
	vec4 cShad = vec4(color, 1.0);  
	vec3 cOut = cShad.rgb;
	if (uColMode == 1)
		{
		cOut = uC1.rgb * cShad.r;
		cOut += uC2.rgb * cShad.g;
		cOut += uC3.rgb * cShad.b;
		}
	cOut = cOut * uIntensity;
#ifdef XL_SHADER
	cOut*=0.1;
#endif
	gl_FragColor = vec4(cOut.rgb,1.0);	
	}
/*	For single color replacements use this, but
		remember to remove uC2 and uC3 from the JSON:

	vec4 cShad = vec4(col, 0.5);  
	vec3 cOut = cShad.rgb;
	if (uColMode == 1) cOut = cOut* uC1.rgb;
	cOut = cOut * uIntensity;
	gl_FragColor = vec4(cOut.rgb,cShad.a);
*/
	