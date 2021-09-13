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
      "MAX": 2
    },
    {
      "NAME": "j",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 2
    },
    {
      "NAME": "F",
      "TYPE": "float",
      "DEFAULT": 5.0,
      "MIN": 0.0,
      "MAX": 10.0
    },
    {
      "NAME": "G",
      "TYPE": "float",
      "DEFAULT": 5.0,
      "MIN": 0.0,
      "MAX": 10.0
    }
  ]
}*/


vec3 iResolution = vec3(RENDERSIZE, 1.0);
float iGlobalTime = TIME;

float gTime = iGlobalTime*0.2;

void main( void )
{
	float f = F;
	float g = G;
	
	vec2 res = iResolution.xy;
	vec2 mou = vec2(0., 0.);
	
	mou.x = sin(gTime * .3) * sin(gTime * .15) * 2. + sin(gTime * .3);
	mou.y = (1.0-sin(gTime * .3 * 2.) )* sin(gTime * .15) + cos(gTime * .3);
	mou *= res;

	vec2 z = ( (-res+2.0 * gl_FragCoord.xy) / res.y);
	vec2 p = ( (-res+2.0 + mou) / res.y) * j ;
	
	for( int i = 0; i < 30; i++) {
		
		float d = dot(z, z) + 0.0 * dot(p*0.1,z*0.1);
		z = (vec2( z.x, -z.y ) / d) + p * (h)/(j); 
		z.x = 1.0- abs(z.x);
		f = max( d-f, sin(dot(z-p,z-p) ));
		g = min( g*d, sin(dot(z+p,z+p))+1.0);
	}
	
	f = abs(-log(f) / 3.5);
	g = abs(+log(g) / 3.5 );
	
	vec3 col =  vec3(g*j, g*f, f*g);
	gl_FragColor = vec4( min( col, 1.0), 1.0);
}