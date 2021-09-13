/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "mechanical"
  ],
  "INPUTS" : [
  	{
		"NAME" : 		"offset",
		"TYPE" : 		"point2D",
		"DEFAULT" :	 	[ 0.0, 0.0 ],
		"MAX" : 		[ 1.0, 1.0 ],
     	"MIN" : 		[ -1.0, -1.0 ]
	},
  	 	{
      	"NAME" : 	"rate",
      	"TYPE" : 	"float",
      	"DEFAULT" : 	1.0,
      	"MIN" : 	-5.0,
      	"MAX" :  	5.0
    	}
  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#37317.0"
}
*/

////////////////////////////////////////////////////////////
// CrankShaftMotion  by mojovideotech
//
// copied from :  
// glslsandbox.com/e#37317.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#ifdef GL_ES
precision mediump float;
#endif

float speed = rate;
float armLength = 0.3;
float rodLength = 0.5;
float pivotSize = 0.0675;
float axleSize = 0.1;

vec3 bgTop = vec3(0.35);
vec3 bgBottom = vec3(0.05);
vec3 logoTop = vec3(0.9);
vec3 logoBottom = vec3(0.4);

float Circle(float rad,vec2 pos) { return length(pos)-rad; }

float Square(float rad,vec2 pos) { return max(abs(pos.x),abs(pos.y))-rad; }

float Rectangle(vec2 size,vec2 pos) { return length(pos-clamp(pos,-size/2.0,size/2.0)) - 0.005; }

mat2 Rotate(float angle) {
	return mat2(cos(angle),-sin(angle), sin(angle), cos(angle)); }

float AngleBetween(vec2 v1,vec2 v2) { return atan(v1.y-v2.y,v1.x-v2.x); }

float Reciprocate(float ca,float cr,float rl) { return cr * cos(ca) + sqrt(rl*rl - pow(cr * sin(ca),2.0)); }

float Lerp(float low,float high,float amount) { return clamp(low + (high - low) * amount,0.0,1.0); }
	

void main( void ) {
	vec2 aspect = vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
	vec2 position = ( gl_FragCoord.xy / RENDERSIZE.y ) - offset;
	vec2 center = aspect / 2.0;
	float angle = -TIME*speed;
	float scene = 0.0;
	vec2 transform = vec2(0);
	//Axle
	vec2 axlePos = center - vec2(-0.3,0);
	transform = position - axlePos;
	vec2 armDomain = transform*Rotate(angle);
	float taper = Lerp(axleSize*1.75,pivotSize,-(armDomain.x/armLength));
	armDomain = (armDomain - vec2(-armLength/2.0,0));
	scene = min(scene,Rectangle(vec2(armLength,taper),armDomain));
	scene = min(scene,Circle(axleSize,transform));
	scene = max(scene,-Circle(axleSize-0.023,transform));
	scene = min(scene,Circle(axleSize-0.035,transform));
	//Crank
	vec2 crankPos = axlePos - vec2(armLength,0) * Rotate(-angle);
	transform = position - crankPos;
	scene = min(scene,Circle(pivotSize,transform));
	scene = max(scene,-Circle(pivotSize-0.025,transform));
	scene = min(scene,Circle(pivotSize-0.0315,transform));
	//Piston
	vec2 pistonPos = axlePos - vec2(Reciprocate(angle,armLength,rodLength),0);
	transform = position -pistonPos; 
	scene = min(scene,Circle(pivotSize,transform));
	scene = max(scene,-Circle(pivotSize-0.025,transform));
	scene = min(scene,Circle(pivotSize-0.0315,transform));
	//Push Rod
	vec2 rodPos = pistonPos;
	transform = (position  - rodPos) * Rotate(-AngleBetween(crankPos,pistonPos));
	transform = transform - vec2(rodLength/2.0,0);
	scene = min(scene,Rectangle(vec2(rodLength,(pivotSize-0.045)*2.0-0.01),transform));
	//Shading
	float blend = smoothstep(0.0,0.002,-scene);
	vec3 background = mix(bgBottom,bgTop,position.y);
	background *= abs(sin((position.x + position.y)*400.0));
	vec3 shade = mix(logoBottom,logoTop,position.y);
	vec3 color = mix(background,shade,blend);
	
	gl_FragColor = vec4( vec3(color), 1.0 );
}
