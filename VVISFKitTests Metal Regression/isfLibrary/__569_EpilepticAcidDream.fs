/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],

  "INPUTS": [
    {
      "MAX": [
        2,
        3
      ],
      "MIN": [
        -0.1,
        -0.1
      ],
      "DEFAULT":[1.8,2.7],
      "NAME": "mouse",
      "TYPE": "point2D"
    },
    {
      "MAX": 1.00,
      "MIN": 0.01,
      "DEFAULT":0.15,
      "NAME": "loop",
      "TYPE": "float"
    },
        {
      "MAX": 2.00,
      "MIN": 0.05,
      "DEFAULT":0.33,
      "NAME": "rate",
      "TYPE": "float"
    }
    
  ]
}
*/

// EpilepticAcidDream by mojovideotech
// 
// http://glslsandbox.com/e#26638.4

#ifdef GL_ES
precision highp float;
#endif

float gTime = TIME * rate;

void main( void )
{
	float f = 5., g = 5.;
	vec2 res = RENDERSIZE.xy;
	vec2 mou = mouse.xy;
	
	//if (mouse.x < 0.5)
	//{
	mou.x = sin(gTime * .0321)*sin(gTime * .667) * 1.5 + sin(gTime * .333);
	mou.y = (-cos(gTime / 3.21))*sin(gTime * .111)*1.5+cos(gTime * .111);
	mou = (mou+3.5) * res;
	//}
	vec2 z = ((-res+(mouse.x*2.0) * gl_FragCoord.xy) / res.y);
	vec2 p = ((res/(mouse.x*2.0)+mou.x) / res.y);
	     p /= ((-res/loop-mou.y) / res.x);
	for( int i = 1; i < 9; i++) 
	{
		float d = dot(z,z);
		z = (vec2( z.x, -z.y ) / d) + p; 
		z.x =  abs(z.x);
		f = dot( f, cos(dot(z-p,z-p))-0.125);
		g = min( g, sin(dot(z+p,z+p))+2.125);
	}
	f = abs(log2(dot(f,g)) / 1.5);
	g = abs(log(g) / sin(f/g));
	vec4 c = vec4 (vec3(dot(f-g,g/f),min(f*g,f-g),1.0), 1.0);
	       c *= vec4 (min(vec3(1.1-f/g, (f/g)*.25, g/(1.5+f)),f+g), 1.0);
	       c /= vec4 (cos(vec3(f, 1.1-f, g+f)), 1.0);
	gl_FragColor = (c);
}