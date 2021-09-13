/*{
  "CREDIT": "by echophons",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "generator"
  ],
  "INPUTS": [
    {
      "NAME": "h",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "j",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 1
    }
  ]
}*/

// edit of http://glslsandbox.com/e#18752.0
uniform vec2 mouse;

vec3   iResolution = vec3(RENDERSIZE, 1.0);
float  iGlobalTime = TIME*.10;

float gTime = iGlobalTime*.10;

void main( void )
{
	float f = 3.0;
	float g = 3.0;
	vec2 res = iResolution.xy;
	vec2 mou = mouse.xy;
	
	if (mouse.x < 0.5)
	{
	mou.x = sin(gTime * .3)*sin(gTime * .17) * 1. + sin(gTime * .3);
	mou.y = (1.1-cos(gTime * .611))*sin(gTime * .200)*1.1+cos(gTime * .3);
	mou = (mou*3.0) * res;
	}
	vec2 z = ((-res+1.0 * gl_FragCoord.xy) / res.y);
	vec2 p = ((-res+1.0+mou) / res.y) * j;
	for( int i = 1; i < 30; i++) 
	{
		float d = dot(z,z);
		z = (vec2( z.x, -z.y ) / d) + p * h; 
		z.x =  1.0-abs(z.x);
		f = max( f-d, (dot(z-p,z-p) ));
		g = min( g*d, sin(dot(z+p,z+p))+1.0);
	}
	f = abs(-log(f) / 5.5);
	g = abs(-log(g) / 10.0);
	gl_FragColor = vec4(min(vec3(g, g*f, f), 1.0),1.0);
}