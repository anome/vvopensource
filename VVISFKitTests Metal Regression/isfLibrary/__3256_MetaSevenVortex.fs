/*
{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "generator",
    "vortex"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
	{
		"NAME" : 		"center",
		"TYPE" : 		"point2D",
		"DEFAULT" :	 	[ 0.0, 0.0 ],
		"MAX" : 		[ 1.0, 1.0 ],
     	"MIN" : 		[ -1.0, -1.0 ]
	},
	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" : 	10.0,
		"MIN" : 		0.0,
		"MAX" : 		30.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		-3.0,
		"MAX" : 		3.0
	},
    {
      	"NAME" : 	 	"fov",
      	"TYPE" :		"float",
      	"DEFAULT" :		1.0,
      	"MIN" : 		0.25,
      	"MAX" : 		2.0
    },
    {
      	"NAME" : 		"light",
      	"TYPE" : 		"float",
      	"DEFAULT" :		0.4,
      	"MIN" : 		-2.0,
      	"MAX" : 		2.0
    },
    {
      	"NAME" : 		"hue",
      	"TYPE" : 		"float",
      	"DEFAULT" :		0.0,
      	"MIN" : 		-0.5,
      	"MAX" : 		0.5
    },
    {
      	"NAME" : 		"tint",
      	"TYPE" : 		"float",
      	"DEFAULT" :		0.0,
      	"MIN" : 		-0.5,
      	"MAX" : 		0.5
    },
	{
			"NAME": "pulse",
			"TYPE": "bool",
			"DEFAULT": false
	}
  ],
   "ISFVSN" : 2.0
}
*/


////////////////////////////////////////////////////////////////////
// MetaSevenVortex  by mojovideotech
//
// based on :
// shadertoy.com/view/lt2fDz
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////

vec2	march(vec3 pos, vec3 dir);
vec3	camera(vec2 uv);
void	rotate(inout vec2 v, float angle);

vec3	ret_col;	// torus color
vec3	h; 			// light amount

#define 	I_MAX		400.
#define 	E			0.00001
#define 	FAR			50.



vec3 blackbody(float Temp) {
	vec3 col = vec3(255.);
    col.x = 56100000. * pow(Temp,(-3.0 / 2.0)) + 148.0;
   	col.y = 100.04 * log(Temp) - 623.6;
   	if (Temp > 6500.0) col.y = 35200000.0 * pow(Temp,(-3.0 / 2.0)) + 184.0;
   	col.z = 194.18 * log(Temp) - 1448.6;
   	col = clamp(col, 0.0, 255.0)/255.0;
    if (Temp < 1000.0) col *= Temp/1000.0;
   	return col;
}

float scene(vec3 p) {  
    float var, mind = 1e5, T = TIME*rate;
    p.z += 10.0;
    rotate(p.xz, 1.57-0.5*T );
    rotate(p.yz, 1.57-0.5*T );
    var = atan(p.x,p.y);
    vec2 q = vec2( ( length(p.xy) )-6.0,p.z);
    rotate(q, var*0.25+T*2.0*0.0);
    vec2 oq = q ;
    q = abs(q)-2.5;
    if (oq.x < q.x && oq.y > q.y)
    	rotate(q, ( (var*1.0)+T*0.0)*3.14+T*0.0);
    else
        rotate(q, ( 0.28-(var*1.0)+T*0.0)*3.14+T*0.0);
    float	oldvar = var;
    ret_col = 1.0-vec3(0.5 + tint, 1.0 - abs(hue + tint), 0.5 - hue);
    mind = length(q)+0.5+1.05*(length(fract(q*0.5*(3.0+3.0*sin(oldvar*1.0 - T*2.0)) )-.5)-1.215);
    h -= vec3(-3.20,0.20,1.0)*vec3(1.0)*0.0025/(0.051+(mind-sin(oldvar*1.0 - T*2.0 + 3.14)*0.125 )*(mind-sin(oldvar*1.0 - T*2.0 + 3.14)*0.125 ) );
    h -= vec3(1.20,-0.50,-0.50)*vec3(1.0)*0.025/(0.501+(mind-sin(oldvar*1.0 - T*2.0)*0.5 )*(mind-sin(oldvar*1.0 - T*2.0)*0.5 ) );
    h += vec3(0.25, 0.4, 0.05)*0.0025/(0.021+mind*mind);
    return (mind);
}

vec2 march(vec3 pos, vec3 dir) {
    vec2 dist = vec2(0.0, 0.0), s = vec2(0.0, 0.0);
    vec3 p = vec3(0.0, 0.0, 0.0);
	for (float i = -1.0; i < I_MAX; ++i) {
	    p = pos + dir * dist.y;
	    dist.x = scene(p);
	    dist.y += dist.x*0.2; 
	    if (log(dist.y*dist.y/dist.x/1e5) > 0.0 || dist.x < E || dist.y > FAR)
        { break; }
	    s.x++;
    }
    s.y = dist.y;
    return (s);
}

void rotate(inout vec2 v, float angle) { v = vec2(cos(angle)*v.x+sin(angle)*v.y,-sin(angle)*v.x+cos(angle)*v.y); }

vec3 camera(vec2 uv) {
	vec3 forw  = vec3(0.0, 0.0, -1.0);
	vec3 right = vec3(1.0, 0.0, 0.0);
	vec3 up = vec3(0.0, 1.0, 0.0);
    return (normalize((uv.x) * right + (uv.y) * up + fov * forw));
}

void main() 
{
    vec3 col= vec3(0.0);
	vec2 R = RENDERSIZE.xy,
    uv  = (vec2(gl_FragCoord.xy-R/2.0) / R.y) - center;
	vec3	dir = camera(uv);
    vec3	pos = vec3(0.0, 0.0, 20.0-scale);
    if (pulse) { pos.z = 4.5+1.5*sin(TIME*abs(rate)*5.0); }
    h*= 0.0;
    vec2 inter = (march(pos, dir));
    col.xyz = ret_col*(1.0-inter.x*0.0125);
    col += h * light;
    gl_FragColor = vec4(sqrt(max(col, 0.0)), 1.0);
}

