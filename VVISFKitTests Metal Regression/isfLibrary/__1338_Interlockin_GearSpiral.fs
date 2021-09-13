/*{
  "CATEGORIES": [
    "gears",
    "rotation",
    "spiral",
    "Automatically Converted"
  ],
  "INPUTS": [],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#33169.0"
}*/


#ifdef GL_ES
precision mediump float;
#endif

#extension GL_OES_standard_derivatives : enable

////////////////////////////////////////////////////////////
// Interlockin'GearSpiral  by mojovideotech
//
// from 
// glslsandbox.com/\e#33169.0
// LICENSE of upper half: public domain, MIT or BSD or WTFPL or whatever, take your pick
////////////////////////////////////////////////////////////

#define PI 3.14159265358979

float line_left_dist(vec2 p, vec2 a, vec2 b){
    vec2 n = normalize(b - a);
    float u = clamp(dot(p - a, n), 0.0, 1.0);
    return dot(p - mix(a, b, u), vec2(-n.y, n.x));
}

float tooth(vec2 p){
    float d0 = line_left_dist(p, vec2(0.15, 0.0), vec2(0.35, 1.0)); 
    float d1 = line_left_dist(p, vec2(0.65, 1.0), vec2(0.85, 0.0));
    float dst = min(p.y, max(p.y - 1.0, max(d0, d1)));
    // droste does not work well with fwidth(dst)
    return smoothstep(0.0, -0.01, dst);
}

float gear(vec2 p, float radius, float n_teeth, float tooth_length, float phase){
    p /= radius;
    float r = length(p);
    float angle = atan(p.y, p.x) + phase;
    float x = fract((angle/PI*0.5 + 0.5)*n_teeth);
    float y = (r - 1.0)/tooth_length*radius + 1.0;
    return tooth(vec2(x, y));
}

vec2 polar(float angle){
    return vec2(cos(angle), sin(angle));
}

float get_angle1(float angle1, float angle2, float n1, float n2){
    return -n1/n2*(angle1 + PI/n1 + angle2) - angle2;
}

float get_angle2(float angle1, float angle2, float n1, float n2){
    return n1/n2*(angle1 + angle2) - angle2;
}

float unit_angle(float angle){
    return atan(sin(angle), cos(angle))/PI*0.5 + 0.5;
}

vec4 gears(vec2 p, float inner_phase, float middle_angle){
    vec4  inner_color = vec4(0.25, 0.4, 0.7, 1.0);
    vec4 middle_color = vec4(0.2, 0.6, 0.4, 1.0);
    vec4  outer_color = vec4(0.3, 0.25, 0.8, 1.0);
    
    // some combinations of values don't work because of physics
    float n_middle_gears = 10.0;
    float  inner_radius  = 0.6;
    float middle_radius  = 0.15;
    float  inner_teeth   = 32.0;
    float tooth_length   = 0.05;

    float  outer_radius  = inner_radius + 2.0*middle_radius;
    float middle_teeth   = inner_teeth*middle_radius/inner_radius;
    float  outer_teeth   = inner_teeth*outer_radius/inner_radius;
    
    float gear_index = floor(unit_angle(atan(p.y, p.x) - middle_angle + PI/n_middle_gears)*n_middle_gears);

    middle_angle += 2.0*PI*gear_index/n_middle_gears + PI;

    vec2 middle_pos = (inner_radius + middle_radius - tooth_length*0.5)*polar(middle_angle);
   
    float middle_phase = get_angle1(inner_phase, middle_angle, inner_teeth, middle_teeth);
    float outer_phase  = get_angle2(middle_phase, middle_angle, middle_teeth, outer_teeth);
    
    vec4 col = vec4(0.0);
    middle_radius *= 1.13; // cheat
    col += inner_color *       gear(p             ,  inner_radius,  inner_teeth, tooth_length,  inner_phase);
    col += middle_color*       gear(p - middle_pos, middle_radius, middle_teeth, tooth_length, middle_phase);
    col += outer_color *(1.0 - gear(p -  vec2(0.0),  outer_radius,  outer_teeth, tooth_length,  outer_phase));
    return col;
}

// LICENSE stuff below: unknown
vec2 cLog(vec2 a) {
    float b =  atan(a.y,a.x);
    if (b>0.0) b-=2.0*3.1415;
    return vec2(log(length(a)),b);
}
vec2 cMul(vec2 a, vec2 b) {
    return vec2( a.x*b.x -  a.y*b.y,a.x*b.y + a.y * b.x);
}
vec2 cPower(vec2 z, float n) {
    float r2 = dot(z,z);
    return pow(r2,n/2.0)*vec2(cos(n*atan(z.y,z.x)),sin(n*atan(z.y,z.x)));
}
vec2 cInverse(vec2 a) {
    return  vec2(a.x,-a.y)/dot(a,a);
}
vec2 cDiv(vec2 a, vec2 b) {
    return cMul( a,cInverse(b));
}
vec2 cExp(in vec2 z){
    return vec2(exp(z.x)*cos(z.y),exp(z.x)*sin(z.y));
}
vec2 droste_(vec2 z,float r1, float r2) {
    z = cLog(z);
    float scale = log(r2/r1);
    float angle = atan(scale/(2.0*PI));
    z = cDiv(z, cExp(vec2(0,angle))*cos(angle)); 
    z.x = mod(z.x,scale);
    z = cExp(z)*r1;
    return z;
}

vec4 domain(vec2 z){
    float shade = 1.5-0.8*length(z);
    z = droste_(z,0.5,1.0);
    return gears(z, cos(TIME*0.4)*3.0, cos(TIME*0.3)*3.)*shade;
}

void main(){
    vec2 p = gl_FragCoord.xy / RENDERSIZE.xy*2.0 - 1.0;
    p.x *= RENDERSIZE.x / RENDERSIZE.y;
	
    gl_FragColor = domain(p);
}