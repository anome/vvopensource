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

vec3   iResolution = vec3(RENDERSIZE, .1);
float  iGlobalTime = TIME;

float gTime = iGlobalTime*4.0;

void main( void )
{
	float f = 90.0;
	float g = 30.0;
	vec2 res = iResolution.xy;
	vec2 mou = mouse.xy;
	
	//if (mouse.x < 0.5)
	//{
	mou.x = sin(gTime * .3)*sin(gTime * .17) * 0. + sin(gTime * .33);
	mou.y = (0.0-cos(gTime * .29))*sin(gTime * .1)*1.9+cos(gTime * .30);
	mou = (mou+1.0) * res;
	//}
	vec2 z = ((-res+2.4 * gl_FragCoord.xy) / res.y);
	vec2 p = ((-res+9.0+mou) / res.y) * j;
	for( int i = 7; i < 90; i++) 
	{
		float d = dot(z,z);
		z = (vec2( z.x, -z.y ) / d) + p * h; 
		z.x =  1.0-abs(z.x);
		f = max( f-d, (dot(z-p,z-p) ));
		g = min( g*d, sin(dot(z+p,z+p))+1.0);
	}
	f = abs(-log(f) / 3.5);
	g = abs(-log(g) / 8.0);
	gl_FragColor = vec4(min(vec3(g, g*f, f), 9.0),1.0);
}