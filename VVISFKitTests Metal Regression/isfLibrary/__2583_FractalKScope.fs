/*{
	"CREDIT": "by mojovideotech",
  	"CATEGORIES" : [
    "fractal",
    "kaleidoscope"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
	{
			"NAME": "offset",
			"TYPE": "float",
			"DEFAULT": 0.13,
			"MIN": -1.0,
			"MAX": 2.0
		},
		{
			"NAME": "range",
			"TYPE": "float",
			"DEFAULT": 1.02,
			"MIN": 0.5,
			"MAX": 2.5
		},
		{
			"NAME": "subdivisions",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 2.0,
			"MAX": 24.0
		},
		{
			"NAME": "rot1",
			"TYPE": "float",
			"DEFAULT": -2.0,
			"MIN": -3.0,
			"MAX": 3.0
		},
		{
			"NAME": "rot2",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": 0.0,
			"MAX": 3.0
		},
		{
			"NAME": "depth",
			"TYPE": "float",
			"DEFAULT": 0.24,
			"MIN": 0.01,
			"MAX": 2.0
		},
			{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 3.0,
			"MIN": 0.125,
			"MAX": 40.0
		},
		{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": -0.66,
			"MIN": -1.5,
			"MAX": 1.5
		},
		{
			"NAME": "loops",
			"TYPE": "float",
			"DEFAULT":60.0,
			"MIN": 10.0,
			"MAX": 200.0
		},
				{
			"NAME": "center",
			"TYPE": "point2D",
			"DEFAULT":	[ 0.05, 0.075 ],
			"MAX" : 	[ 0.125, 0.125 ],
      		"MIN" : 	[ 0.0, 0.0 ]
		}
  ]
}
*/

////////////////////////////////////////////////////////////
// FractalKScope  by mojovideotech
//
// based on :
// www.shadertoy/\XtffDX  by balkhan  
// www.iquilezles.org/www/articles/mset_smooth/mset_smooth.htm
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	pi   	3.141592653589793 	// pi

vec2 Kscope (vec2 p, float count) {
    float an = twpi/count;
    float a = atan(p.y,p.x)+an*0.5;
    a = mod(a, an)-an*0.5;
    return vec2(cos(a),sin(a))*length(p);
}

vec2 	cmult(vec2 a, vec2 b) { return (vec2(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x)); }

vec2	cadd(vec2 a, vec2 b) { return (vec2(a.x + b.x, a.y + b.y)); }

void main() 
{
	float TT = TIME * rate;
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 z = vec4(0.0, 0.0, 0.0, 0.0);
    vec2 of = vec2((uv.x -0.5)/0.25, (uv.y -0.5)/0.25);
    vec3 col = vec3(0.0);
    vec2 dist = vec2(0.0);
    z.xy = of / zoom;
    float ii = -1.0;
   	z.xy = Kscope(z.xy, floor(subdivisions));
	z.x -= offset;
    for (int i = -1; i < 201; ++i) {
        ++ii;
        z.xy = cadd((cmult( abs(z.xy)-1.0*sin(TT*center.x)/pi , abs(z.xy)-1.0*sin(TT*center.y)/pi)), vec2(-depth, -2.0));
        vec2 fz = fract(z.xy*1.1)-0.5;
        z.xy += 1.0/max(fz*fz, range);
        z.xy = cadd(z.xy, vec2(sin(rot1)/twpi, cos(rot2)/pi) );
        z.z = 2.0 * (z.x*z.z - z.y*z.w);
        z.w = 2.0 * (z.y*z.z - z.x*z.w);
        dist.x = dot(z.xy,z.xy);
		dist.y = dot(z.zw,z.zw);
		if (float(i) > floor(loops)) break;
        if (dist.x > 100000000000.0 || dist.y > 100000000000.0) break;
    }
    vec3 tmp = vec3(0.0);
        if( ii < floor(loops)-0.5 ) {
            float sit = ii - log2(log2(dot(z.xy,z.xy))/(log2(2.0)))/log2(2.0); 
            tmp = + vec3(sin(+sit*0.25 + 0.0)*0.25 + 0.5, sin(+sit*0.25 + 1.04)*0.25 + 0.3, sin(+sit*0.25 + 2.08)*0.25 + 0.2);
        }
    col = tmp;
    gl_FragColor = vec4(col, 1.0);
}


