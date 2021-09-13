/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "generator",
    "2d",
    "rotation",
    "circular"
  ],
  "INPUTS" : [
  	
		{
      			"NAME" :	"valeurDeY",
      			"TYPE" : 	"float",
      			"DEFAULT" :	-9.0,
      			"MIN" : 	-12.0,
      			"MAX" :		12.0
    	},
    	{
      			"NAME" :	"valeurDeX",
      			"TYPE" : 	"float",
      			"DEFAULT" :	-0.9,
      			"MIN" : 	-0.91,
      			"MAX" :		0.1
    	},
    	{
      			"NAME" :	"seed1",
      			"TYPE" : 	"float",
      			"DEFAULT" :	55,
      			"MIN" : 	8,
      			"MAX" :		233
    	},
    	{
      			"NAME" :	"seed2",
      			"TYPE" :	"float",
      			"DEFAULT" :	89,
      			"MIN" : 	55,
      			"MAX" :		987
    	},
    	{
      			"NAME" :	"seed3",
      			"TYPE" :	"float",
      			"DEFAULT" :	28657,
      			"MIN" :		17711,
      			"MAX" :		75025
    	},
    	{
				"NAME" :	"radius1",
				"TYPE" :	"float",
				"DEFAULT" :	0.1,
				"MIN" :		0.1,
				"MAX" :		0.5
		},
		{
				"NAME" :	"radius2",
				"TYPE" :	"float",
				"DEFAULT" :	1.5,
				"MIN" :		0.6,
				"MAX" :		2.5
		},

    	{
      			"NAME" :	"offset1",
      			"TYPE" :	"float",
      			"DEFAULT" :	19.0,
      			"MIN" :		15,
      			"MAX" :		50
    	},
    	{
      			"NAME" :	"offset2",
      			"TYPE":		"float",
      			"DEFAULT" :	17.0,
      			"MIN" :		0.5,
      			"MAX" :		29.5
    	},
    	{
      			"NAME" :	"offset3",
      			"TYPE":		"float",
      			"DEFAULT" :	9.0,
      			"MIN" :		0.5,
      			"MAX" :		19
    	},
    	{
				"NAME" :	"rate",
				"TYPE" :	"float",
				"DEFAULT" :	1.0,
				"MIN" :		-2.0,
				"MAX" :		2.0
		}
  ],
  "DESCRIPTION" : ""
}
*/

///////////////////////////////////////////
// BitWheelRGB  by mojovideotech
//
// mod of :
// interactiveshaderformat.com/\sketches/\1474
//
// based on :
// --- arcs ---
// by Catzpaw
// glslsandbox.com/e#36148.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////

#ifdef GL_ES
precision highp float;
#endif

#define 	ptpi 	1385.455731367011089 	// powten(pi)
#define 	chpi 	11.591953275521521  	// cosh(pi)
#define 	shpi 	25.548739357257749 		// sinh(pi)
#define 	twpi  	25.283185307179586		// 2*(pi)


float hash(float s){return fract(sin(s*seed3)*seed1);}

vec2 rot(vec2 p,float a){return p*mat2(sin(a),cos(a),cos(a),-sin(a));}

void main( void ) 
{
	vec2 uv=(gl_FragCoord.xy*2.-RENDERSIZE.xy)/min(RENDERSIZE.x,RENDERSIZE.y); 
	vec3 finalColor=vec3(0.0);
	float d=length(uv);
	if(d<radius2&&d>radius1) {
		float s=floor(d*(valeurDeX*5.+4.))/ptpi;
		float SS = floor(seed2);
		float e=hash(s+SS+valeurDeX)*twpi;
		float t=hash(s+SS*0.833)*twpi;
		float slice = (sin (TIME/12.0))/15.0;
		uv=rot(uv,t*rate+(hash(s)*6.-3.));
		float a;
		if (uv.y>0.0)a=atan(uv.y,uv.x);
		else a=twpi+atan(uv.y,uv.x);
		float k = 1.1;
		vec3 col = vec3(smoothstep (0.5, 0.5, 0.5));
		if(e<a&&mod(a*shpi,k)<hash(s+ valeurDeY +2.0)*k)finalColor+=vec3(col);
		float k1 = k+offset1;
		if(e<a&&mod(a*chpi,k)<hash(s+ valeurDeY +2.0)*k1)finalColor+=vec3(1.0,1.0,1.0);
		float k2 = k+offset2;
		if(e<a&&mod(a*shpi,k1 )<hash(s+ valeurDeX+2.0)*k2)finalColor=vec3(0.0,0.0,0.0);
		float k3 = k+offset3;
		if(e<a&&mod(a*chpi,k2)<hash(s+ valeurDeX+2.0)*k3)finalColor=vec3(0.0,0.0,0.0);
	}
	else finalColor=vec3(0.0);	
gl_FragColor = vec4(finalColor,1.0);


}
