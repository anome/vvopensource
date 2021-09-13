/*{
	"CREDIT": "by thedantheman",
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
			"NAME": "modValue",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "hatch_y_offset",
			"TYPE": "float",
			"DEFAULT": 5.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "lum_threshold_1",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "lum_threshold_2",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "lum_threshold_3",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "lum_threshold_4",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/





void main() {


	vec2 pos = isf_FragNormCoord*RENDERSIZE;
	vec3 color = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord).rgb;
	
	vec3 tc = vec3(0.0, 0.0, 0.0);

	float lum = length(color);
	
	if (lum <= lum_threshold_1) 
	{
	    if (mod(pos.x + pos.y + hatch_y_offset,  floor(modValue)) == 0.0){ 
	        tc = vec3(1.0);
	    }
	}
	if (lum <= lum_threshold_2) 
	{
	    if (mod(pos.x - pos.y - hatch_y_offset,  floor(modValue)) == 0.0)
	    { 
	         tc = vec3(1.0);
	    }
	}
	if (lum <= lum_threshold_3) 
	{
	    if (mod(pos.x + pos.y - hatch_y_offset,  floor(modValue)) == 0.0){ 
	         tc = vec3(1.0);
	    }
	}
	if (lum <= lum_threshold_4) 
	{
	    if (mod(pos.x - pos.y + hatch_y_offset, floor(modValue)) == 0.0){ 
	         tc = vec3(1.0);
	    }
	}
			
	

	gl_FragColor = vec4(tc, 1.0);
}