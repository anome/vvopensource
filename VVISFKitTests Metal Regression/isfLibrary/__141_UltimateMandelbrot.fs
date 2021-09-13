/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "fractal",
    "mandelbrot"
  ],
  "INPUTS": [
  	{
		"NAME" : 		"center",
		"TYPE" : 		"point2D",
		"DEFAULT" :		[ 0.0, 0.0 ],
		"MAX" : 		[ 10.0, 10.0 ],
     	"MIN" : 		[ -10.0, -10.0 ]
	},
	{
		"NAME" : 		"target",
		"TYPE" : 		"point2D",
		"DEFAULT" :		[ 0.702985, 0.299 ],
		"MAX" : 		[ 1.0, 1.0 ],
     	"MIN" : 		[ -1.0, -1.0 ]
	},
	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.00002,
		"MIN" : 		0.00001,
		"MAX" : 		0.001
	},
	{
		"NAME" : 		"zoom",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		0.025,
		"MAX" : 		4.0
	},
	{
		"NAME" : 		"loops",
		"TYPE" : 		"float",
		"DEFAULT" :		60.0,
		"MIN" : 		16.0,
		"MAX" : 		512.0
	},
	{
		"NAME" : 		"cycle",
		"TYPE" : 		"float",
		"DEFAULT" :		4.0,
		"MIN" : 		-16.0,
		"MAX" : 		16.0
	},
  	{
		"NAME" : 		"freq",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.25,
		"MIN" : 		0.1,
		"MAX" : 		1.0
	},
	{
      "NAME": "Zx",
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
        8
      ],
      "LABELS": [
        "-",
        "Sin",
        "SinSin",
        "TanSin",
        "ExpSinCos",
        "SinModSin",
        "SinLen",
        "SinSinSinCosUV",
        "CosSinATanTIME"
      ],
      "DEFAULT": 2
	},
	{
   		"NAME" : 		"flip",
     	"TYPE" : 		"bool",
     	"DEFAULT" : 	false
   	},
	{
   		"NAME" : 		"rot",
     	"TYPE" : 		"bool",
     	"DEFAULT" : 	true
   	}
  ],
  "DESCRIPTION": "based on http://www.glslsandbox.com/e#40432.0"
}*/

////////////////////////////////////////////////////////////
// UltimateMandelbrot   by mojovideotech
//
// based on : 
// Vlad's fractal canyon  glslsandbox/e#40432.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////



vec4 color(float m) {
	m += cycle * TIME;
	float r = (1.0 + sin(freq * m + 1.0)) / 2.0;
	float g = (1.0 + sin(freq * m + 2.0)) / 2.0;
	float b = (1.0 + sin(freq * m + 4.0)) / 2.0;
	return (vec4(r, g, b, 1.0));
}

void main() {
	vec2 uv = isf_FragNormCoord - vec2(0.5) + center;
	uv.x *= RENDERSIZE.x / RENDERSIZE.y;
	if (flip) { uv *= -1.0; }
	if (rot) { uv.xy = -uv.yx; }
	vec2 z = vec2(0), z0, zT;
	float T = TIME * zoom; 
	z0.x = uv.x / (scale * T) / (T * 40.0) - (target.x + (T * 0.00000005));
	z0.y = uv.y / (scale * T) / (T * 40.0) - (target.y + (T * 0.00000005));
	float F = 0.0, G = 0.0, H = 0.0;
	for(int i = 0; i < 512; i++) {
		if(dot(z,z) > 16.0) break;
		if(G > loops) break;
		zT.x = (z.x * z.x - z.y * z.y) + z0.x;
		zT.y = (z.y * z.x + z.x * z.y) + z0.y;
		z = zT;
		F++;
		G += 1.0;
	}
	if (Zx == 0)		{	H = log2(log2(dot((z),(z))));	            	}
	else if (Zx == 1)	{	H = log2(log2(dot(sin(z),(z))));	        	}
	else if (Zx == 2)	{	H = log2(log2(dot(sin(z),sin(z))));	        	}
	else if (Zx == 3)	{	H = log2(log2(dot(tan(z),sin(z))));	        	}
	else if (Zx == 4)	{	H = log2(log2(dot(exp(sin(z)/z),cos(z*z))));	}
	else if (Zx == 5)	{	H = log2(log2(dot(sin(1.0/z),mod(sin(z),z))));	}
	else if (Zx == 6)	{	H = log2(log2(dot(sin(z),vec2(length(z)))));	}
	else if (Zx == 7)	{	H = log2(log2(dot(1.0/sin(z-sin(uv.yy)),1.0/sin(z+cos(uv.xx))))); }
	else if (Zx == 8)	{	H = log2(log2(dot(cos(TIME)/sin(z.yy+atan(z.x,z.x)),sin(z.yy)-sin(TIME)/atan(z.y,z.y)))); }
	
	gl_FragColor = (F == floor(loops+1.0)) ? vec4(0.0, 0.0, 0.0, 1) : color(F - H);
}
	