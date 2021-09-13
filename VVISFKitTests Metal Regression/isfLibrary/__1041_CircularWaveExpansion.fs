/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
	]
}*/


// CircularWaveExpansion by mojovideotech
// glslsandbox.com/e#30123.0

#ifdef GL_ES
precision mediump float;
#endif

float hash(float x)
{
	return fract((sin(x*6324.5986)+31.7811)*2415.7817);
}

float noise(float x)
{
	float a = hash(floor(x));
	float b = hash(ceil(x));
	float t = fract(x);
	return a*(1.0-t) + b*t;
}

void main( void )
{
	vec2 p = gl_FragCoord.xy / RENDERSIZE;
	p -=0.5;
	p.x *= RENDERSIZE.x/RENDERSIZE.y;
	vec3 color = vec3(0.0);
	int id = 0;
	for (int i=0; i<21; i++)
	{
		float h = sin(5.5*noise(length(p) - (TIME*0.1) + noise(float(i))*17.711));
		if (p.y < h) id++;
	}
	
	color.r = sin(float(id)*19.6418);
	color.g = sin(float(id)*92.27465);
	color.b = sin(float(id)*51.4229);
	gl_FragColor = vec4( color, 1.0 );

}