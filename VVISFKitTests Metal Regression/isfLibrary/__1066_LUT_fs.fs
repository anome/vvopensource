/*
{
  "CATEGORIES" : [
    "Film", "Stylize"
  ],
  "DESCRIPTION" : "Example of using LUT for stylizing",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
	{
		"NAME": "lookupTable",
		"TYPE": "long",
		"VALUES": [
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9
		],
		"LABELS": [
			"Basic",
			"Etikate",
			"Selective",
			"Warm",
			"Soft Tones",
			"Bleak Cold",
			"Crisp Winter",
			"Film Stock",
			"Bleak Future",
			"Foggy Night"
		],
		"DEFAULT": 0
	}
  ],
  "CREDIT" : "",
  "IMPORTED": {
    "lookupTable0": {
      "PATH": "lookup.png"
     },
    "lookupTable1": {
      "PATH": "lookup_miss_etikate.png"
     },
    "lookupTable2": {
      "PATH": "lookup_selective_color.png"
     },
    "lookupTable3": {
      "PATH": "lookup_muted_sat.png"
     },
    "lookupTable4": {
      "PATH": "lookup_toned.png"
     },
    "lookupTable5": {
      "PATH": "lookup_dickson.png"
     },
    "lookupTable6": {
      "PATH": "lookup_crispwinter.png"
     },
    "lookupTable7": {
      "PATH": "lookup_filmstock_50.png"
     },
    "lookupTable8": {
      "PATH": "lookup_futuristicbleak.png"
     },
    "lookupTable8": {
      "PATH": "lookup_futuristicbleak.png"
     },
    "lookupTable9": {
      "PATH": "lookup_foggynight.png"
     }
     
  }
}
*/



//	adapted from https://github.com/mattdesl/glsl-lut
//	... which itself is adapted from http://liovch.blogspot.ca/2012/07/add-instagram-like-effects-to-your-ios.html


//	some included LUTs grabbed from
//	https://github.com/pissang/emage/
//	https://github.com/mattdesl/glsl-lut
//	http://stackoverflow.com/questions/24910234/how-to-use-lut-with-javascript
//	hhttp://filma.kr/?s=LUT


vec3 rgb2hsv(vec3 c)	{
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	//vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
	//vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
	vec4 p = c.g < c.b ? vec4(c.bg, K.wz) : vec4(c.gb, K.xy);
	vec4 q = c.r < p.x ? vec4(p.xyw, c.r) : vec4(c.r, p.yzx);
	
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)	{
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}


void main()	{
	vec4		inputPixelColor = IMG_THIS_PIXEL(inputImage);
	vec2		loc = gl_FragCoord.xy;
	
    float blueColor = inputPixelColor.b * 63.0;
    
    vec2		lutDims = vec2(512.0);

    vec2 quad1;
    quad1.y = floor(floor(blueColor) / 8.0);
    quad1.x = floor(blueColor) - (quad1.y * 8.0);

    vec2 quad2;
    quad2.y = floor(ceil(blueColor) / 8.0);
    quad2.x = ceil(blueColor) - (quad2.y * 8.0);

    vec2 texPos1;
    texPos1.x = (quad1.x * 0.125) + 0.5/lutDims.x + ((0.125 - 1.0/lutDims.x) * inputPixelColor.r);
    texPos1.y = (quad1.y * 0.125) + 0.5/lutDims.y + ((0.125 - 1.0/lutDims.y) * inputPixelColor.g);

	texPos1.y = 1.0-texPos1.y;

	/*
    #ifdef LUT_FLIP_Y
        texPos1.y = 1.0-texPos1.y;
    #endif
	*/
	
    vec2 texPos2;
    texPos2.x = (quad2.x * 0.125) + 0.5/lutDims.x + ((0.125 - 1.0/lutDims.x) * inputPixelColor.r);
    texPos2.y = (quad2.y * 0.125) + 0.5/lutDims.y + ((0.125 - 1.0/lutDims.y) * inputPixelColor.g);

	texPos2.y = 1.0-texPos2.y;

	/*
    #ifdef LUT_FLIP_Y
        texPos2.y = 1.0-texPos2.y;
    #endif
	*/
	
    vec4 newColor1 = vec4(0.0);
    vec4 newColor2 = vec4(0.0);
    if (lookupTable == 0)	{
    	newColor1 = IMG_NORM_PIXEL(lookupTable0, texPos1);
    	newColor2 = IMG_NORM_PIXEL(lookupTable0, texPos2);
	}
	else if (lookupTable == 1)	{
    	newColor1 = IMG_NORM_PIXEL(lookupTable1, texPos1);
    	newColor2 = IMG_NORM_PIXEL(lookupTable1, texPos2);
	}
	else if (lookupTable == 2)	{
    	newColor1 = IMG_NORM_PIXEL(lookupTable2, texPos1);
    	newColor2 = IMG_NORM_PIXEL(lookupTable2, texPos2);
	}
	else if (lookupTable == 3)	{
    	newColor1 = IMG_NORM_PIXEL(lookupTable3, texPos1);
    	newColor2 = IMG_NORM_PIXEL(lookupTable3, texPos2);
	}
	else if (lookupTable == 4)	{
    	newColor1 = IMG_NORM_PIXEL(lookupTable4, texPos1);
    	newColor2 = IMG_NORM_PIXEL(lookupTable4, texPos2);
	}
	else if (lookupTable == 5)	{
    	newColor1 = IMG_NORM_PIXEL(lookupTable5, texPos1);
    	newColor2 = IMG_NORM_PIXEL(lookupTable5, texPos2);
	}
	else if (lookupTable == 6)	{
    	newColor1 = IMG_NORM_PIXEL(lookupTable6, texPos1);
    	newColor2 = IMG_NORM_PIXEL(lookupTable6, texPos2);
	}
	else if (lookupTable == 7)	{
    	newColor1 = IMG_NORM_PIXEL(lookupTable7, texPos1);
    	newColor2 = IMG_NORM_PIXEL(lookupTable7, texPos2);
	}
	else if (lookupTable == 8)	{
    	newColor1 = IMG_NORM_PIXEL(lookupTable8, texPos1);
    	newColor2 = IMG_NORM_PIXEL(lookupTable8, texPos2);
	}
	else if (lookupTable == 9)	{
    	newColor1 = IMG_NORM_PIXEL(lookupTable9, texPos1);
    	newColor2 = IMG_NORM_PIXEL(lookupTable9, texPos2);
	}

    vec4 newColor = mix(newColor1, newColor2, fract(blueColor));
	
	gl_FragColor = newColor;
}
