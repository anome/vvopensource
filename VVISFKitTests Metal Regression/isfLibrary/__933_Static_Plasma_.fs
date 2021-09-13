/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
  "CATEGORIES" : [
    "Generator",
    "Glitch"
  ],
  "INPUTS" : [
  ]
}
*/

////////////////////////////////////////////////////////////
// GlitchyFractalPlasma  by mojovideotech
//
// automatically converted from :
// glslsandbox.com/\e#42715.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

float t=TIME/75.*90.;

vec2 B(vec2 a) { return vec2(log(length(a)),atan(a.y,a.x)-6.3); }

vec3 F(vec2 E) {
vec2 e_=E;
float c=2.;
const int i_max=30;
for(int i=3; i<i_max; i++) {
	e_=B(vec2(e_.x,abs(e_.y)))+vec2(.4*sin(t/3.)-.1,5.+.1*cos(t/5.));
	c += length(e_);
	}
float d = log2(log2(c*.05))*6.;
return vec3(.7+tan(.7*cos(d)),.5+.5*cos(d-.7),.7+sin(.7*cos(d-.7)));
}

void main(void) {
gl_FragColor=vec4(F(vec2(dot(F(gl_FragCoord.xy/RENDERSIZE.x).zx,F(vec2(gl_FragCoord.y,cos(t))).yz),cos(9.0-9.1*sin(-t)))),1.);
}
