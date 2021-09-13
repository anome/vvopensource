/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#27762.0",
  "INPUTS": []
}*/

// 8BitsMarching by mojovideotech
// http://glslsandbox.com/e#27762.0

#ifdef GL_ES
precision mediump float;
#endif


float extract_bit(float n, float b)
{
	n = floor(n);
	b = floor(b);
	b = floor(n/pow(2.,b)-TIME);
	return float(mod(b,2.) == 1.);
}

void main( void ) {

	vec2 uv 	= gl_FragCoord.xy / RENDERSIZE.xy;

	float position	= floor(uv.y *8.);
	float number 	= floor(uv.x * 256.);
	float bits 	= extract_bit(number, position);;
		

	gl_FragColor = vec4(0.0,bits,0.0,1.0);
}