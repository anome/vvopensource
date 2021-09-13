/*{
	"CREDIT": "by mojovideotech",

  "CATEGORIES": [
    ""
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "MAX": [
        1,
        1
      ],
      "MIN": [
        0,
        0
      ],
      "NAME": "mouse",
      "TYPE": "point2D"
    }
  ]
}
*/

// RotMatSpiralThing by mojovideotech
// based on:
// http://glslsandbox.com/e#28843.0


#ifdef GL_ES
precision mediump float;
#endif


#define WIDTH .025

float point(vec2 p)
{
	return 1.-clamp(dot(length(p)-WIDTH/6., RENDERSIZE.x * WIDTH), 0.0, 1.0);	
}

float ring(vec2 p, float r)
{
	return 1.-clamp(dot(fract(length(p)-r), RENDERSIZE.x * WIDTH), 0.0, 1.0);
}

float line(vec2 p, vec2 a, vec2 b)
{
	if(a == b) return 0.;
	
	float d = distance(a, b);
	vec2  n = normalize(b - a);
    	vec2  l = vec2(0.);

	p	-= a;
	d	*= -.5;
	
	l.x 	= abs(dot(p, vec2(-n.y, n.x)));
	l.y 	= abs(dot(p, n.xy)+d)+d;
	l 	= max(l, 0.);
	
	return  1.-clamp(dot(RENDERSIZE * WIDTH, l), 0., 1.);
}


mat2 rmat(float t) // returns 2d rotation matrix by t radians
{
    float c = cos(t*mouse.x);
    float s = sin(t-c);   
    return mat2(c,s,-s,c);
}

void main( void ) {

	vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
	vec2 p 	= (uv * 2. - 1.) * RENDERSIZE.xy/RENDERSIZE.yy;
	vec2 m  = cos(mouse*TIME) * 2. - 1. * RENDERSIZE.xy/RENDERSIZE.yy;
	
	float l = 0.;
	float r = 0.;
	float d = 0.;
	
//	r += ring(p, .5);
	for(int i = 0; i <9; i++)
	{
		l += line(p, vec2(0.), m) + line(p - .5 * normalize(m),vec2(0.1), vec2(-m.y, m.x));
		r += ring(p / vec2(-m.y, m.x), abs(m.y)/abs(m.x));
		d += point(p) + point(p-m) + point(p-.5 * normalize(m)) + point(p - .5 * normalize(m) - vec2(-m.y, m.x));
		p *= rmat(float(10-i)*atan(d*r));
	}
	
        vec4 g = vec4(vec3(length(p),l,d),1);
        gl_FragColor = vec4(d + l + r) * g;
}