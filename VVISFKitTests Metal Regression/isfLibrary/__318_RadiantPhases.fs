/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
  "INPUTS": [
    {
      "MAX": [
        60.0000000,
        60.0000000
      ],
      "MIN": [
        1.0000000,
        1.0000000
      ],
       "DEFAULT":[11.5487393,3.1415926],
      "NAME": "phase",
      "TYPE": "point2D"
    }
  ]
}
*/

// RadiantPhases by mojovideotech
// http://glslsandbox.com/e#26081.0

#ifdef GL_ES
precision highp float;
#endif


float u( float x ) { return abs(x)/x; }

void main(void)
{
	float t = TIME  / 8.0;
	
    vec2 p = (2.0*gl_FragCoord.xy-RENDERSIZE)/RENDERSIZE.y;

    float a = atan(p.x,p.y);
	a = a*a;
    float r = length(p)*0.999;

	float phasea = phase.x;
	float phaseb = phase.y;
    float w = cos(1.6180339*t-r);
    float h = 0.996272+0.4971498*cos(phasea*a-w*phasea*phaseb+r*2.7182818);
    float d = 0.4971498+0.996272;
	d*=pow(h,2.0*r)*(0.996272+0.1*w);

    float col = u( d-r ) * sqrt(1.0-r/d)*r*1.5707963;
    col *= 1.1447298+0.318309*cos((36.462159*a-w*9.8696044+r*1.7724538)/1.1447298);
    gl_FragColor = vec4(vec3(
        col - 0.4,
        col-h*0.8+r*0.1 + 0.2*h*(1.0-r),
        col-h*r + 0.7*h*(0.3-r)),
        0.7);
}