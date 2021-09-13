/*{
	"DESCRIPTION": "Messing around trying to learn how to work with shapes and video content.",
	"CREDIT": "zerbzman",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
      "NAME" : "rotation",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.25,
      "MIN" : 0
    },
		{
			"NAME": "width1",
			"TYPE": "float",
			"DEFAULT": 0.3,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "height1",
			"TYPE": "float",
			"DEFAULT": 0.3,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
      "NAME" : "square1Angle",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "invertSquare1",
      "TYPE" : "bool",
      "DEFAULT" : false,
      "LABEL" : "Invert Square 1"
    },
    {
      "NAME" : "invertColor1",
      "TYPE" : "bool",
      "DEFAULT" : false,
      "LABEL" : "Invert Square 1 Color"
    },
    {
      "NAME" : "square1Transparency",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.25,
      "MIN" : 0
    },
    {
			"NAME": "width2",
			"TYPE": "float",
			"DEFAULT": 0.4,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "height2",
			"TYPE": "float",
			"DEFAULT": 0.4,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
      "NAME" : "invertSquare2",
      "TYPE" : "bool",
      "DEFAULT" : true,
      "LABEL" : "Invert Square 2"
    },
    {
      "NAME" : "square2Angle",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "invertColor2",
      "TYPE" : "bool",
      "DEFAULT" : false,
      "LABEL" : "Invert Square 2 Color"
    },
    {
      "NAME" : "square2Transparency",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.25,
      "MIN" : 0
    },
    {
			"NAME": "width3",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "height3",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
      "NAME" : "invertSquare3",
      "TYPE" : "bool",
      "DEFAULT" : false,
      "LABEL" : "Invert Square 3"
    },
    {
      "NAME" : "square3Angle",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "invertColor3",
      "TYPE" : "bool",
      "DEFAULT" : false,
      "LABEL" : "Invert Square 3 Color"
    },
    {
      "NAME" : "square3Transparency",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.25,
      "MIN" : 0
    },
    {
			"NAME": "width4",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "height4",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
      "NAME" : "invertSquare4",
      "TYPE" : "bool",
      "DEFAULT" : true,
      "LABEL" : "Invert Square 4"
    },
    {
      "NAME" : "square4Angle",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "invertColor4",
      "TYPE" : "bool",
      "DEFAULT" : false,
      "LABEL" : "Invert Square 4 Color"
    },
    {
      "NAME" : "square4Transparency",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 1,
      "MIN" : 0
    },
    {
      "NAME" : "invertBackground",
      "TYPE" : "bool",
      "DEFAULT" : false,
      "LABEL" : "Invert Background"
    },
    {
      "NAME" : "angleBackground",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "invertBackgroundColor",
      "TYPE" : "bool",
      "DEFAULT" : false,
      "LABEL" : "Invert Background Color"
    },
		{
			"NAME": "pos",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

/*
Features I want to add:
- different shapes
- multiple shapes (grid)
- offset shapes (offset grid)
- fade shapes
- drop shadow behind shapes
- rotate background behind shapes
- rotate background of shapes
- rotate background of individual shapes* probably couldn't have a variable number
  shapes or if I did I could change the rotation a little more per shape
- foreground image and background image are different
- 


*/
const float pi = 3.14159265359;

float rect(vec2 p, vec2 size) {
    vec2 d = abs(p) - size;
    float value = clamp(d.x, d.y, 0.0) + length(max(d, 0.0));
    return step(0.0, value);
}

float rectShadow(vec2 p, vec2 size) {
    vec2 d = abs(p) - size;
    float value = clamp(d.x, d.y, 0.0) + length(max(d, 0.0));
    return smoothstep(0.0, 0.5, value);
}

vec2 rotatePoint(vec2 pt, float angle, vec2 center) {
    vec2 returnMe;
    float s = sin(angle * pi);
    float c = cos(angle * pi);

    returnMe = pt;

    // translate point back to origin:
    returnMe.x -= center.x;
    returnMe.y -= center.y;

    // rotate point
    float xnew = returnMe.x * c - returnMe.y * s;
    float ynew = returnMe.x * s + returnMe.y * c;

    // translate point back:
    returnMe.x = xnew + center.x;
    returnMe.y = ynew + center.y;
    return returnMe;
}

vec3 invertColor(vec3 c) {
    return 1.0 - c;
}

vec2 rotate(vec2 v, float a) {
  float sinA = sin(a);
  float cosA = cos(a);
  return vec2(v.x * cosA - v.y * sinA, v.y * cosA + v.x * sinA);
}

void main() {
    vec2 loc = gl_FragCoord.xy / RENDERSIZE.xy;
  loc -= vec2(pos);

  loc.x *= RENDERSIZE.x / RENDERSIZE.y;
  
  vec2 normSrcCoord = isf_FragNormCoord;

    normSrcCoord.x = isf_FragNormCoord[0];
    normSrcCoord.y = isf_FragNormCoord[1];
    // vec2 loc = isf_FragNormCoord;
    // loc -= .5;
    
    loc = rotate(loc, rotation * pi);
    // 	vec4 outputPixelColor = vec4(0.0)
    vec4 srcPixel = IMG_THIS_PIXEL(inputImage);


    float rectangle1 = rect(loc, vec2(width1, height1));
    float rectangle2 = rect(loc, vec2(width2, height2));
    float rectangle3 = rect(loc, vec2(width3, height3));
    float rectangle4 = rect(loc, vec2(width4, height4));


    if (rectangle1 == 0.0) { // if we are not in the square
        

        if (invertSquare1) {
            normSrcCoord.y = (1.0 - normSrcCoord.y);
        }

        vec4 pixelAdjusted = IMG_NORM_PIXEL(inputImage, normSrcCoord);

        if (invertColor1) {
            pixelAdjusted.rgb = invertColor(pixelAdjusted.rgb);
        }
        
        srcPixel = mix(srcPixel, pixelAdjusted, square2Transparency);

    } else if (rectangle2 == 0.0) {

        if (invertSquare2) {
            normSrcCoord.y = (1.0 - normSrcCoord.y);
        }

        normSrcCoord = rotatePoint(normSrcCoord, square2Angle, vec2(0.5, 0.5));

        vec4 pixelAdjusted = IMG_NORM_PIXEL(inputImage, normSrcCoord);

        if (invertColor2) {
            pixelAdjusted.rgb = invertColor(pixelAdjusted.rgb);
        }
        
        srcPixel = mix(srcPixel, pixelAdjusted, square2Transparency);

    } else if (rectangle3 ==  0.0) {
        // vec2 normSrcCoord3;

        // normSrcCoord3.x = isf_FragNormCoord[0];
        // normSrcCoord3.y = isf_FragNormCoord[1];

        if (invertSquare3) {
            normSrcCoord.y = (1.0 - normSrcCoord.y);
        }

        normSrcCoord = rotatePoint(normSrcCoord, square3Angle, vec2(0.5, 0.5));

        vec4 pixelAdjusted = IMG_NORM_PIXEL(inputImage, normSrcCoord);

        if (invertColor3) {
            pixelAdjusted.rgb = invertColor(pixelAdjusted.rgb);
        }
        
        srcPixel = mix(srcPixel, pixelAdjusted, square3Transparency);
                
    } else if (rectangle4 ==  0.0) {
        // vec2 normSrcCoord4;

        // normSrcCoord4.x = isf_FragNormCoord[0];
        // normSrcCoord4.y = isf_FragNormCoord[1];

        if (invertSquare4) {
            normSrcCoord.y = (1.0 - normSrcCoord.y);
        }

        normSrcCoord = rotatePoint(normSrcCoord, square4Angle, vec2(0.5, 0.5));

        vec4 pixelAdjusted = IMG_NORM_PIXEL(inputImage, normSrcCoord);

        if (invertColor4) {
            pixelAdjusted.rgb = invertColor(pixelAdjusted.rgb);
        }
        
        srcPixel = mix(srcPixel, pixelAdjusted, square4Transparency);
                
    } else {
        // vec2 normSrcCoord3;

        // normSrcCoord3.x = isf_FragNormCoord[0];
        // normSrcCoord3.y = isf_FragNormCoord[1];

        if (invertBackground) {
            normSrcCoord.y = (1.0 - normSrcCoord.y);
        }

        normSrcCoord = rotatePoint(normSrcCoord, angleBackground, vec2(0.5, 0.5));

        srcPixel = IMG_NORM_PIXEL(inputImage, normSrcCoord);
        // srcPixel = srcPixel * g;
        if (invertBackgroundColor) {
            srcPixel.rgb = invertColor(srcPixel.rgb);
        }
    }


    // 	vec3 color = vec3(rectangle);

    // 	gl_FragColor = IMG_NORM_PIXEL(inputImage, vec2(rectangle));
    gl_FragColor = srcPixel;
}