/*{
	"CREDIT": "by isak.burstrom",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "color_1",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				0.0,
				0.0,
				1.0
			]
		},
		{
			"NAME": "color_2",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				0.25,
				0.0,
				1.0
			]
		},
		{
			"NAME": "color_3",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				1.0,
				0.0,
				1.0
			]
		},
		{
			"NAME": "color_4",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				1.0,
				0.0,
				1.0
			]
		},
		{
			"NAME": "color_5",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				1.0,
				0.5,
				1.0
			]
		},
		{
			"NAME": "color_6",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				1.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "color_7",
			"TYPE": "color",
			"DEFAULT": [
				0.25,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "color_1_amount",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "color_2_amount",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "color_3_amount",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "color_4_amount",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "color_5_amount",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "color_6_amount",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "color_7_amount",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		}
	]
}*/

const float stepsize = 0.33;

vec4 treshold_color(vec4 original, vec4 color_1, vec4 color_2, float luma, float index) {
	float from = index * stepsize;
	float to = from + stepsize;
	
	 if(luma >= from && luma <= to) {
	 	return mix(color_1, color_2, luma / to);
	 }
}

vec4 getOutColor(int index) {
	float color[8];
	vec4 color_val[8];
	
	color[1] = color_1_amount;
	color[2] = color_2_amount;
	color[3] = color_3_amount;
	color[4] = color_4_amount;
	color[5] = color_5_amount;
	color[6] = color_6_amount;
	color[7] = color_7_amount;
	
	color_val[1] = color_1;
	color_val[2] = color_2;
	color_val[3] = color_3;
	color_val[4] = color_4;
	color_val[5] = color_5;
	color_val[6] = color_6;
	color_val[7] = color_7;
	
	vec4 outColor;
	
	for(int i = 8; i > 0; i -= 1) {
		if(color[i] == 1.0 && i < index) {
			outColor = color_val[i];
			break;
		}
	}
	
	return outColor;
}

void main() {
	 vec4 pixcol = IMG_NORM_PIXEL(inputImage, vv_FragNormCoord.xy);
	 vec4 colors[4];
	 
	 float lum = (pixcol.r+pixcol.g+pixcol.b) / 3.;
	 
	 // om luma(0.8) < 0.7
	 
	 //if(lum < treshold) {
	 //	thermal = mix(colors[0], colors[1], lum / treshold);
	 //} else {
	 //	thermal = mix(colors[1],colors[2],(lum - treshold) / treshold);
	 //}
	 
	int ix = 0;
	float blend = 1.0;
	float range = 1.0 / 6.0;
	
	//	orange to red
	vec4 startColor;
	vec4 endColor;
	bool foundEndColor = false;
  
	//	green to yellow
	 if (lum > range * 5.0 && color_7_amount == 1.)	{
		startColor = color_7;
		ix = 5;
		blend = color_7_amount;
		endColor = getOutColor(7);
	}
	//	green to green
	else if (lum > range * 4.0 && color_6_amount == 1.)	{
		startColor = color_6;
		ix = 4;
		blend = color_6_amount;
		endColor = getOutColor(6);
	}
	//	cyan to green
	else if (lum > range * 3.0 && color_5_amount == 1.)	{
		startColor = color_5;
		endColor = getOutColor(5);
		ix = 3;
		blend = color_5_amount;
	}
	//	blue to cyan
	else if (lum > range * 2.0 && color_4_amount == 1.)	{
		startColor = color_4;
		endColor = getOutColor(3);
		blend = color_4_amount;
		ix = 2;
	}
	// purple to blue
	else if (lum > range && color_3_amount == 1.)	{
		startColor = color_3;
		endColor = getOutColor(2);
		blend = color_3_amount;
		ix = 1;
	}
	else if (color_2_amount == 1.) {
		blend = color_2_amount;
		startColor = color_2;
		endColor = getOutColor(1);
	} else if (color_1_amount == 1.) {
		blend = color_1_amount;
		startColor = color_1;
		endColor = color_1;
	}
	
	
	 if(boolInput) {
		vec4 thermal = mix(vec4(0), mix(startColor,endColor,(lum-float(ix)*range)/range), blend);
		gl_FragColor = thermal;
	 } else {
	 	gl_FragColor = vec4(lum);
	 }
	 
}