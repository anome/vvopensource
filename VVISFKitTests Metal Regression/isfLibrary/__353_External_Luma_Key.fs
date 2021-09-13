/*{
	"DESCRIPTION": "Apply any input like a geometric animation or whatever to another video or image.",
	"CREDIT": "zerbzman",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Masking"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "keyImage",
			"TYPE": "image"
		},
		{
      "NAME" : "invertKey",
      "TYPE" : "bool",
      "DEFAULT" : false,
      "LABEL" : "Invert Key"
    },
		{
      "NAME" : "invertY",
      "TYPE" : "bool",
      "DEFAULT" : true,
      "LABEL" : "Invert Square 2"
    },
    {
      "NAME" : "angle",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "invertColorInput",
      "TYPE" : "bool",
      "DEFAULT" : false,
      "LABEL" : "Invert Square 2 Color"
    },
    {
			"NAME": "threshold",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.00,
			"MAX": 1.0
		},
		{
			"NAME": "softness",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.00,
			"MAX": 1.0
		},
		{
      "NAME": "keyPos",
      "TYPE": "point2D",
      "DEFAULT": [
        0.0,
        0.0
      ],
      "MIN": [
        -1,
        -1
      ],
      "MAX": [
        1,
        1
      ]
    }
	]
}*/
const float pi = 3.14159265359;

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

float extractLuma(vec4 image, float threshold, float softness) {
    float fValue = (image.r * 0.29 + image.g * 0.6 + image.b * 0.11);
    float l1 = threshold - softness * 0.5;
    float l2 = l1 + softness;
    fValue = smoothstep(max(l1, 0.0), min(l2, 1.0), fValue);
    return fValue;
}

void main() {
    vec4 srcPixel = IMG_THIS_PIXEL(inputImage);
    vec2 normSrcCoord;

    normSrcCoord.x = isf_FragNormCoord[0];
    normSrcCoord.y = isf_FragNormCoord[1];
    
    vec2 keyCoord = normSrcCoord;
    keyCoord -= vec2(keyPos);
    // keyCoord += 0.5;

    if (invertY) {
        normSrcCoord.y = (1.0 - normSrcCoord.y);
    }

    normSrcCoord = rotatePoint(normSrcCoord, angle, vec2(0.5, 0.5));

    vec4 pixelAdjusted = IMG_NORM_PIXEL(inputImage, normSrcCoord);

    if (invertColorInput) {
        pixelAdjusted.rgb = invertColor(pixelAdjusted.rgb);
    }
    
    float lumaValue = extractLuma(IMG_NORM_PIXEL(keyImage, keyCoord), threshold, softness);

    if (invertKey) {
        lumaValue = 1.0 - lumaValue;
    }
    

    pixelAdjusted.a = mix(0., 1., lumaValue);

    gl_FragColor = pixelAdjusted;
}