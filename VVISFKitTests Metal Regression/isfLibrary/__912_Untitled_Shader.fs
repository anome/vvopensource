/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "from http://glslsandbox.com/e#35553.0",
  "CATEGORIES": [
    "fluid",
    "liquid"
  ],
  "INPUTS": [
  	{
			"NAME" :	"rate1",
			"TYPE" :	"float",
			"DEFAULT" :	1.9,
			"MIN" :	-3.0,
			"MAX" :	3.0
	},
	 {
			"NAME" :	"rate2",
			"TYPE" :	"float",
			"DEFAULT" :	0.6,
			"MIN" :	-3.0,
			"MAX" :	3.0
	},
	{
			"NAME" :	"loopcycle",
			"TYPE" :	"float",
			"DEFAULT" :	85.0,
			"MIN" :	20.0,
			"MAX" :	100.0
	},
	{
			"NAME" :	"color1",
			"TYPE" :	"float",
			"DEFAULT" :	0.45,
			"MIN" :	-2.5,
			"MAX" :	2.5
	},
	{
			"NAME" :	"color2",
			"TYPE" :	"float",
			"DEFAULT" :	1.0,
			"MIN" :	-1.25,
			"MAX" :	1.125
	},
	{
			"NAME" :	"cycle1",
			"TYPE" :	"float",
			"DEFAULT" :	1.33,
			"MIN" :	0.01,
			"MAX" :	3.1459
	},
	{
			"NAME" :	"cycle2",
			"TYPE" :	"float",
			"DEFAULT" :	0.22,
			"MIN" :	-0.497,
			"MAX" :	0.497
	},
	{
			"NAME" :	"nudge",
			"TYPE" :	"float",
			"DEFAULT" :	0.095,
			"MIN" :	0.001,
			"MAX" :	0.01
	},
	{
      			"NAME" :	"depthX",
      			"TYPE" : 	"float",
      			"DEFAULT" :	0.85,
      			"MIN" : 	0.001,
      			"MAX" :		0.9
    	},
    	{
      			"NAME" :	"depthY",
      			"TYPE" :	"float",
      			"DEFAULT" :	0.25,
      			"MIN" : 	0.001,
      			"MAX" :		0.9
    	}
  ]
}*/

///////////////////////////////////////////
// ColorDiffusionFlow  by mojovideotech
//
// based on :
// glslsandbox.com/\e#35553.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////


#ifdef GL_ES
precision mediump float;
#endif
 
#define 	pi   	3.141592653589793 	// pi

uniform vec2 u_resolution;
uniform vec2 u_mouse;
uniform float u_time;

vec3 mod289(vec3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec2 mod289(vec2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec3 permute(vec3 x) { return mod289(((x*34.0)+1.0)*x); }

float snoise(vec2 v) {
    const vec4 C = vec4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                        0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                        -0.577350269189626,  // -1.0 + 2.0 * C.x
                        0.024390243902439); // 1.0 / 41.0
    vec2 i  = floor(v + dot(v, C.yy) );
    vec2 x0 = v -   i + dot(i, C.xx);
    vec2 i1;
    i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
    vec4 x12 = x0.xyxy + C.xxzz;
    x12.xy -= i1;
    i = mod289(i); // Avoid truncation effects in permutation
    vec3 p = permute( permute( i.y + vec3(0.0, i1.y, 1.0 ))
        + i.x + vec3(0.0, i1.x, 1.0 ));

    vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
    m = m*m ;
    m = m*m ;
    vec3 x = 2.0 * fract(p * C.www) - 1.0;
    vec3 h = abs(x) - 0.5;
    vec3 ox = floor(x + 0.5);
    vec3 a0 = x - ox;
    m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
    vec3 g;
    g.x  = a0.x  * x0.x  + h.x  * x0.y;
    g.yz = a0.yz * x12.xz + h.yz * x12.yw;
    return 130.0 * dot(m, g);
}


void main() {
	float T = TIME * rate1;
	float TT = TIME * rate2;
	vec2 p=(2.*isf_FragNormCoord);
	for(int i=1;i<11;i++) {
    	vec2 newp=p;
		float ii = float(i);  
    	newp.x+=depthX/ii*sin(ii*pi*p.y+T*nudge+cos((TT/(5.0*ii))*ii));
    	newp.y+=depthY/ii*cos(ii*pi*p.x+TT+nudge+sin((T/(5.0*ii))*ii));
    	p=newp+log(DATE.w)/loopcycle;
  }
  vec3 col=vec3(cos(p.x+p.y+3.0*color1)*0.5+0.5,sin(p.x+p.y+6.0*cycle1)*0.5+0.5,(sin(p.x+p.y+9.0*color2)+cos(p.x+p.y+12.0*cycle2))*0.25+.5);
  vec4 fluid_col = vec4(col*col, 1.0);
  ////
  vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;///u_resolution.xy;
    st.x *= RENDERSIZE.x/RENDERSIZE.y;
    vec3 color = vec3(0.0);
    vec2 pos = vec2(st*3.);

    float DF = 0.0;

    // Add a random position
    float a = 0.0;
    vec2 vel = vec2(TIME*.1);
    DF += snoise(pos+vel)*.25+.25;

    // Add a random position
    a = snoise(pos*vec2(cos(u_time*0.15),sin(u_time*0.1))*0.1)*3.1415;
    vel = vec2(cos(a),sin(a));
    DF += snoise(pos+vel)*.25+.25;

    color = vec3( smoothstep(.1,.5,fract(DF)) );

    vec4 dot_color = vec4(1.0-color,1.0);
    vec4 final_color = vec4(0);
    if( dot_color.xyz == vec3(0))
        final_color=fluid_col;
    else
        final_color=vec4(1);
        
    gl_FragColor = final_color;

}