/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#26998.0",
  "INPUTS": []
}*/


#ifdef GL_ES
precision mediump float;
#endif


const float thresh = 1.0;
const float lowThresh = 0.25;

// effect ringblobs

void main( void ) {

	vec2 p = ( gl_FragCoord.xy / RENDERSIZE.x ) - vec2(0.5, 0.5 * (RENDERSIZE.y / RENDERSIZE.x));

	vec3 rgb = vec3(0.0);
	float c = 0.0;
	for (int i=0; i<9; i++)
	{
		vec2 pos = p + vec2(sin(float(i*i) + 1.75 * TIME) * 0.45, sin(float(i*i*i) + 0.65 * inversesqrt(TIME)) * 0.2667);
		c += pow(0.175 / length(pos/0.125), 4.0);
	}
	if (c > 1.0 && c < thresh) c = (thresh - c) / (thresh - 1.0);
	if (c >= thresh)
	{
		// do effect
		c = mod(sin(p.x * 10.0) + sin(p.y * 54.0) + sin(p.x * 111.0 + sin(p.y * 96.0 + 11.0 * TIME)) + sin(p.y * 64.0 + 33.0 * TIME + sin(p.x * 72.0 + p.y * 90.0)) + 4.0, 2.0);
		if (c > 1.0) c = 2.0 - c;
		rgb = vec3(c, c * 2.0, c * 3.0);
	}
/*	else if (c > lowThresh)
	{
		rgb = vec3(c);
	}
	else
	{
		float gr = sin(length(p) * p.y);
		float rx = floor(sin(gr * 15.0) + sin(gr * 25.0 - 3.0 * TIME) * 1.125);
		float ry = floor(sin(gr * 25.0 + TIME) + sin(gr * 35.0 + 4.0 * TIME) * 3.333);
		float rz = floor(sin(gr * 45.0) + sin(gr * 20.0 + sin(gr * 30.0 + 2.0 * TIME)) * 1.5);
		vec3 raster = vec3(rx, ry, rz) * 0.25;
		     raster -= vec3(ry, rz, rx) * 0.25;
		float d = c / lowThresh;
		rgb = d * vec3(c) + (1.0 - d) * raster;
	}
*/
	gl_FragColor = vec4(vec3(c), 1.0);

}