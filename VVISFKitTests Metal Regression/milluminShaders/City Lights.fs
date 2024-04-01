/*{
	"CREDIT": "by VIDVOX",
	"CATEGORIES": [
		"Stylize"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "intensity",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
		}
	],
	"PASSES": [
		{
			"TARGET": "edgesBuffer",
			"WIDTH": "$WIDTH",
			"HEIGHT": "$HEIGHT"
		},
		{
			"TARGET": "smallA",
			"WIDTH": "floor($WIDTH*min((0.2),1.0))",
			"HEIGHT": "floor($HEIGHT*min((0.2),1.0))"
		},
		{
			"TARGET": "smallB",
			"WIDTH": "floor($WIDTH*min((0.2),1.0))",
			"HEIGHT": "floor($HEIGHT*min((0.2),1.0))"
		},
		{
			"TARGET": "smallC",
			"WIDTH": "floor($WIDTH*min((0.3),1.0))",
			"HEIGHT": "floor($HEIGHT*min((0.3),1.0))"
		},
		{
			"TARGET": "smallD",
			"WIDTH": "floor($WIDTH*min((0.3),1.0))",
			"HEIGHT": "floor($HEIGHT*min((0.3),1.0))"
		},
		{
			"TARGET": "smallE",
			"WIDTH": "floor($WIDTH*min((0.5),1.0))",
			"HEIGHT": "floor($HEIGHT*min((0.5),1.0))"
		},
		{
			"TARGET": "smallF",
			"WIDTH": "floor($WIDTH*min((0.5),1.0))",
			"HEIGHT": "floor($HEIGHT*min((0.5),1.0))"
		},
		{
			"TARGET": "smallG",
			"WIDTH": "floor($WIDTH*min((0.8),1.0))",
			"HEIGHT": "floor($HEIGHT*min((0.8),1.0))"
		},
		{
			"TARGET": "smallH",
			"WIDTH": "floor($WIDTH*min((0.8),1.0))",
			"HEIGHT": "floor($HEIGHT*min((0.8),1.0))"
		},
		{
			"TARGET": "smallI"
		},
		{
			
		}
	]
}*/


//	original blur implementation as v002.blur in QC by anton marini and tom butterworth, ported by zoidberg


varying vec2 texOffsets[3];
varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;


float gray(vec4 n)
{
	return (n.r + n.g + n.b)/3.0;
}


void main() {
	if (PASSINDEX == 0)	{
		vec4 color = IMG_THIS_PIXEL(inputImage);
		vec4 colorL = IMG_NORM_PIXEL(inputImage, left_coord);
		vec4 colorR = IMG_NORM_PIXEL(inputImage, right_coord);
		vec4 colorA = IMG_NORM_PIXEL(inputImage, above_coord);
		vec4 colorB = IMG_NORM_PIXEL(inputImage, below_coord);

		vec4 colorLA = IMG_NORM_PIXEL(inputImage, lefta_coord);
		vec4 colorRA = IMG_NORM_PIXEL(inputImage, righta_coord);
		vec4 colorLB = IMG_NORM_PIXEL(inputImage, leftb_coord);
		vec4 colorRB = IMG_NORM_PIXEL(inputImage, rightb_coord);

		float gx = (-1.0 * gray(colorLA)) + (-2.0 * gray(colorL)) + (-1.0 * gray(colorLB)) + (1.0 * gray(colorRA)) + (2.0 * gray(colorR)) + (1.0 * gray(colorRB));
		float gy = (1.0 * gray(colorLA)) + (2.0 * gray(colorA)) + (1.0 * gray(colorRA)) + (-1.0 * gray(colorRB)) + (-2.0 * gray(colorB)) + (-1.0 * gray(colorLB));

		float bright = pow(gx*gx + gy*gy,0.5);
		gl_FragColor = color * bright * intensity * 10.0;
	}
	else if (0 < PASSINDEX)	{
		vec4		sample0;
		vec4		sample1;
		vec4		sample2;
		if (PASSINDEX == 1)	{
			sample0 = IMG_NORM_PIXEL(edgesBuffer,texOffsets[0]);
			sample1 = IMG_NORM_PIXEL(edgesBuffer,texOffsets[1]);
			sample2 = IMG_NORM_PIXEL(edgesBuffer,texOffsets[2]);
		}
		else if (PASSINDEX == 2)	{
			sample0 = IMG_NORM_PIXEL(smallA,texOffsets[0]);
			sample1 = IMG_NORM_PIXEL(smallA,texOffsets[1]);
			sample2 = IMG_NORM_PIXEL(smallA,texOffsets[2]);
		}
		else if (PASSINDEX == 3)	{
			sample0 = IMG_NORM_PIXEL(smallB,texOffsets[0]);
			sample1 = IMG_NORM_PIXEL(smallB,texOffsets[1]);
			sample2 = IMG_NORM_PIXEL(smallB,texOffsets[2]);
		}
		else if (PASSINDEX == 4)	{
			sample0 = IMG_NORM_PIXEL(smallC,texOffsets[0]);
			sample1 = IMG_NORM_PIXEL(smallC,texOffsets[1]);
			sample2 = IMG_NORM_PIXEL(smallC,texOffsets[2]);
		}
		else if (PASSINDEX == 5)	{
			sample0 = IMG_NORM_PIXEL(smallD,texOffsets[0]);
			sample1 = IMG_NORM_PIXEL(smallD,texOffsets[1]);
			sample2 = IMG_NORM_PIXEL(smallD,texOffsets[2]);
		}
		else if (PASSINDEX == 6)	{
			sample0 = IMG_NORM_PIXEL(smallE,texOffsets[0]);
			sample1 = IMG_NORM_PIXEL(smallE,texOffsets[1]);
			sample2 = IMG_NORM_PIXEL(smallE,texOffsets[2]);
		}
		else if (PASSINDEX == 7)	{
			sample0 = IMG_NORM_PIXEL(smallF,texOffsets[0]);
			sample1 = IMG_NORM_PIXEL(smallF,texOffsets[1]);
			sample2 = IMG_NORM_PIXEL(smallF,texOffsets[2]);
		}
		else if (PASSINDEX == 8)	{
			sample0 = IMG_NORM_PIXEL(smallG,texOffsets[0]);
			sample1 = IMG_NORM_PIXEL(smallG,texOffsets[1]);
			sample2 = IMG_NORM_PIXEL(smallG,texOffsets[2]);
		}
		else if (PASSINDEX == 9)	{
			sample0 = IMG_NORM_PIXEL(smallH,texOffsets[0]);
			sample1 = IMG_NORM_PIXEL(smallH,texOffsets[1]);
			sample2 = IMG_NORM_PIXEL(smallH,texOffsets[2]);
		}
		else if (PASSINDEX == 10)	{
			sample0 = IMG_NORM_PIXEL(smallI,texOffsets[0]) * 3.0;
			sample1 = IMG_NORM_PIXEL(smallI,texOffsets[1]) * 3.0;
			sample2 = IMG_NORM_PIXEL(smallI,texOffsets[2]) * 3.0;
		}
		else	{
			sample0 = vec4(1,0,0,1);
			sample1 = vec4(1,0,0,1);
			sample2 = vec4(1,0,0,1);
		}
		vec4 final = vec4((sample0 + sample1 + sample2).rgb / (3.0), 1.0);
		if (PASSINDEX == 10)	{
			vec4 edge = IMG_NORM_PIXEL(edgesBuffer,texOffsets[0]);
			final = min((final + final * edge)/(2.0-intensity), 1.0);
			
			//	do a color tweak – based on the v002 technicolor 3 style
			vec3 redmatte = vec3(final.r - ((final.g + final.b)/2.0));
			vec3 greenmatte = vec3(final.g - ((final.r + final.b)/2.0));
			vec3 bluematte = vec3(final.b - ((final.r + final.g)/2.0));

			redmatte = 1.0 - redmatte;
			greenmatte = 1.0 - greenmatte;
			bluematte = 1.0 - bluematte;

			vec3 red =  greenmatte * bluematte * final.r;
			vec3 green = redmatte * bluematte * final.g;
			vec3 blue = redmatte * greenmatte * final.b;

			final = vec4(red.r, green.g, blue.b, final.a);
		}
		gl_FragColor = final;
	}
}
