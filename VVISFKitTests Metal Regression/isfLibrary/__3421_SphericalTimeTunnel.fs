/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator",
		"tunnel"
	],
	"INPUTS": [
	]
}*/

////////////////////////////////////////////////////////////
// SphericalTimeTunnel  by mojovideotech
// 
// License: 
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	twpi  	6.283185307179586  // two pi, 2*pi
#define 	pi   		3.141592653589793  // pi

float t = mod(TIME,300.0);

vec3 hex(vec2 coords) {
	float q = (coords.x * atan(0.05/coords.x) / 2.5) / log2(length(coords))+(t+coords.y)*0.0125;
	float r = (coords.x * (1.66667/coords.y) - coords.y ) + (q -t) ;
	return vec3(q,-r,q-r);
}

float round(float a) {
	float t = floor(a);
	if(fract(a)>fract(t*a)) t=ceil(a);
	return t;
}

vec3 cubed(vec3 cube) {
    float rx = round(cube.x);
    float ry = round(cube.y);
    float rz = round(cube.z);
    float x_diff = abs(rx - cube.x);
    float y_diff = abs(ry - cube.y);
    float z_diff = abs(rz - cube.z);
    if (x_diff > y_diff && x_diff > z_diff)
        rx = -ry-rz;
    else if (y_diff > z_diff)
        ry = -rx-rz;
    else
        rz = -rx-ry;
    return vec3(rx, ry, rz);
}

vec3 edged(vec3 lo) { return cubed(vec3(lo.x+lo.y,lo.y+lo.x,lo.z+lo.x)); }

void main() {
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	float th =  uv.t * pi, ph = uv.s * 2.0 * pi;		
	vec3 p = vec3(sin(th) * cos(ph), sin(th) * sin(ph), cos(th));
	p.x += 1.0;
	vec3 cell = cubed((hex(p.xx)));
	vec3 pc = hex(p.xz);
	float d = floor(dot(pc,cell)*2.0);
	vec3 lo = vec3(pc.y-cell.x,pc.y+d,pc.z-cell);
	vec3 ec = edged(lo);
	float em = min(mix(ec.x,max(ec.y,ec.z),d),max(ec.x,ec.y));
	vec3 res, col;
	res.x = smoothstep(0.5,-1.0,em);
	res.y = smoothstep(d,d+0.5,em-d);
	res.z = smoothstep(res.y,res.x,d+em);
	col = vec3(res.x*pi,th-(t*res.y),res.z);
	res = sqrt(vec3(pow(mod(res,cell+t),col))+th);
	col.y -= res.y/th;
	gl_FragColor = vec4(col/res,1.0);
}