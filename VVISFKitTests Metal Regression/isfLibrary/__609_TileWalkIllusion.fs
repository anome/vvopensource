/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" : 	5.0,
		"MIN" : 		0.5,
		"MAX" : 		30.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		-1.0,
		"MAX" : 		1.0
	},
	{
		"NAME" : 		"line",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.02,
		"MIN" : 		0.01,
		"MAX" : 		0.05
	},
	{
   		"NAME" : 		"flip",
     	"TYPE" : 		"bool",
     	"DEFAULT" : 	false
   	},
	{
   		"NAME" : 		"flop",
     	"TYPE" : 		"bool",
     	"DEFAULT" : 	true
	}
	]
}*/
////////////////////////////////////////////////////////////
// TileWalkIllusion   by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define ss 5
#define pi 3.1415926535897

float roundf(float v, float d){
    return ceil(v/d-0.5)*d;
}

float checkerboard(vec2 uv){
    vec2 p=mod(uv-vec2(0.5),1.0);
    return mod(step(p.x,0.5)+step(p.y,0.),2.0);
}

vec2 rot(vec2 uv, float r){
    float cr=cos(r),sr=sin(r);
    return vec2(cr*uv.x-sr*uv.y,sr*uv.x+cr*uv.y);
}

void main()
{
    float tv=0.0;
    float t=TIME*rate;
    
    for(int xp=0;xp<ss;xp++){
        for(int yp=0;yp<ss;yp++){
            vec2 uv = 2.0*(gl_FragCoord.xy-RENDERSIZE.xy*0.5+vec2(xp,yp)/float(ss))/RENDERSIZE.x;
            if (flip) { uv *= -1.0; }
			if (flop) { uv.xy = -uv.yx; }
            uv*=scale;
            uv.x=uv.x-roundf(uv.y-0.25,0.5)*t;
            float v=checkerboard(uv);

            if(abs(roundf(uv.y,0.5)-uv.y)<line) v=0.5;
                tv+=v;
        }
    }
    tv=tv/float(ss*ss);
    gl_FragColor = vec4(tv,tv,tv,1.0);
}