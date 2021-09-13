/*{
	"CREDIT": "by gosub7777777",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "colorA",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "colorB",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "par1",
			"TYPE": "float",
			"DEFAULT": 0.5
		},
		{
			"NAME": "par2",
			"TYPE": "float",
			"DEFAULT": 0.5
		},
		{
			"NAME": "par3",
			"TYPE": "float",
			"DEFAULT": 0.5
		}
	]
}*/

// https://www.shadertoy.com/view/MsfGzM
// http://iquilezles.org/www/articles/derivative/derivative.htm

float f(vec3 p) 
{ 
	p.z+=TIME;
	return length(par1*cos(par2*2.0*p.y*p.x)+cos(p)-.4*cos(.1*(p.z+1.*p.x-p.y)))-1.; 
	
}

void mainImage( out vec4 c, vec2 p )
{
    vec3 d=0.5-vec3(p,1)/RENDERSIZE.x;
    vec3 o=d;
    for(int i=0;i<128;i++){
    	o+=f(o)*d;
    };
    c.rgb = abs(f(o-d)*colorA.rgb+f(o-par3)*colorB.rgb)*(1.-.1*o.z);
    c.a = 1.0;
}


void main() {
	mainImage(gl_FragColor, gl_FragCoord.xy);	
}