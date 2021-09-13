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
				"NAME" :	"center",
				"TYPE" :	"point2D",
				"DEFAULT" :	[ -5.0, -3.0 ],
				"MAX" : 	[ 10.0, 10.0 ],
      			"MIN" :  	[ -10.0, -10.0 ]
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
      			"MIN" :		1597,
      			"MAX" :		75025
    	},
    	{
				"NAME" :	"radius1",
				"TYPE" :	"float",
				"DEFAULT" :	0.3,
				"MIN" :		0.1,
				"MAX" :		0.5
		},
		{
				"NAME" :	"radius2",
				"TYPE" :	"float",
				"DEFAULT" :	0.8,
				"MIN" :		0.6,
				"MAX" :		1.5
		},

    	{
      			"NAME" :	"offset1",
      			"TYPE" :	"float",
      			"DEFAULT" :	0.1,
      			"MIN" :		49.1,
      			"MAX" :		100
    	},
    	{
      			"NAME" :	"offset2",
      			"TYPE":		"float",
      			"DEFAULT" :	11.1,
      			"MIN" :		11.0,
      			"MAX" :		100
    	},
    	{
				"NAME" :	"rate",
				"TYPE" :	"float",
				"DEFAULT" :	1.0,
				"MIN" :		-2.0,
				"MAX" :		2.0
		}
  ],
  "DESCRIPTION" : "mod of http://glslsandbox.com/e#36148.0"
}
*/

///////////////////////////////////////////
// BitWheel  by mojovideotech
//
// mod of :
//
// --- arcs ---
// by Catzpaw
// glslsandbox.com/e#36148.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////

#ifdef GL_ES
precision highp float;
#endif

#define 	twpi  	6.283185307179586

float hash(float s){return fract(sin(s*seed3)*seed1);}

vec2 rot(vec2 p,float a){return p*mat2(sin(a),cos(a),cos(a),-sin(a));}

void main( void ) 
{
	vec2 uv=(gl_FragCoord.xy*2.-RENDERSIZE.xy)/min(RENDERSIZE.x,RENDERSIZE.y); 
	vec3 finalColor=vec3(0.0);
	float d=length(uv), TT = TIME*rate;
	if(d<radius2&&d>radius1) {
		float s=floor(d*(center.x*5.+4.))/800.;
		float SS = floor(seed2);
		float e=hash(s+SS+center.y)*twpi;
		float t=hash(s+SS*.833)*twpi;
		uv=rot(uv,t+TT*(hash(s)*6.-3.));
		float a;
		if (uv.y>0.0)a=atan(uv.y,uv.x);
		else a=twpi+atan(uv.y,uv.x);
		float k = 1.1;
		vec3 col = vec3(0.0,1.0,1.0);
			if(e<a&&mod(a*10.1,1.1)<hash(s+center.y+2.0)*k)finalColor+=vec3(col);
			k=k+offset1;
			if(e<a&&mod(a*10.1,k)<hash(s+center.y+2.0)*k)finalColor*=vec3(0.0,0.0,1.0);
			k =k-offset2;
			if(e<a&&mod(a*10.1,k)<hash(s+center.y+2.0)*k)finalColor*=vec3(1.0,0.0,0.0);
	}
	//else finalColor=vec3(0.0);	
	gl_FragColor = vec4(finalColor,1.0);
}