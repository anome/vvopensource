/*{
  "CREDIT": "by VIDVOX",
  "CATEGORIES": [
    "Stylize"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    }
  ]
}*/


//	partly adapted from http://coding-experiments.blogspot.com/2010/10/thermal-vision-pixel-shader.html

varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;


void main ()	{
	//vec4 pixcol = IMG_THIS_PIXEL(inputImage);
	vec4 colors[9];
	//	8 color stages with variable ranges to get this to look right
	//	black, purple, blue, cyan, green, yellow, orange, red, red
	colors[0] = vec4(0.0,0.0,0.0,1.0);
	colors[1] = vec4(0.272,0.0,0.4,1.0);	//	dark deep purple, (RGB: 139, 0, 204)
	colors[2] = vec4(0.0,0.0,1.0,1.0);		//	full blue
	colors[3] = vec4(0.0,1.0,1.0,1.0);		//	cyan
	colors[4] = vec4(0.0,1.0,0.5,1.0);		//	green
	colors[5] = vec4(0.0,1.0,0.0,1.0);		//	green
	colors[6] = vec4(1.0,1.0,0.0,1.0);		//	yellow
	colors[7] = vec4(1.0,0.5,0.0,1.0);		//	orange
	colors[8] = vec4(1.0,0.0,0.0,1.0);		//	red
	
	vec4 color = IMG_THIS_NORM_PIXEL(inputImage);
	vec4 thermal;
	//float lum = (color.r+color.g+color.b)/3.0;
	float lum = dot(vec3(0.33, 0.57, 0.10), color.rgb);
	lum = pow(lum,1.3);

	float range = 1.0 / 8.0;
	
	//	orange to red
	if (lum > 0.9)	{
		thermal = mix(colors[7],colors[8],(lum-float(7)*range)/range);
	}
	//	yellow to orange
	else if (lum > range * 6.0)	{
		thermal = mix(colors[6],colors[7],(lum-float(6)*range)/range);
	}
	//	green to yellow
	else if (lum > range * 5.0)	{
		thermal = mix(colors[5],colors[5+1],(lum-float(5)*range)/range);
	}
	//	green to green
	else if (lum > range * 4.0)	{
		thermal = mix(colors[4],colors[4+1],(lum-float(4)*range)/range);
	}
	//	cyan to green
	else if (lum > range * 3.0)	{
		thermal = mix(colors[3],colors[3+1],(lum-float(3)*range)/range);
	}
	//	blue to cyan
	else if (lum > range * 2.0)	{
		thermal = mix(colors[2],colors[2+1],(lum-float(2)*range)/range);
	}
	// purple to blue
	else if (lum > range)	{
		thermal = mix(colors[1],colors[1+1],(lum-float(1)*range)/range);
	} else {
	  thermal = mix(colors[0],colors[1],(lum-float(0)*range)/range);
	}
	
	gl_FragColor = thermal;

}


