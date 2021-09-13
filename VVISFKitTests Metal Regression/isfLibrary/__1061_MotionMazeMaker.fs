/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator",
		"pattern",
		"maze"
	],
	"INPUTS": [
	{
		"NAME" : 		"pattern",
		"TYPE" : 		"point2D",
		"DEFAULT" :		[ -0.5, 0.5 ],
		"MAX" : 		[ 1.0, 1.0 ],
     	"MIN" : 		[ -1.0, -1.0 ]
	},
	{
		"NAME" : 		"motion",
		"TYPE" : 		"point2D",
		"DEFAULT" :		[ 0.1, -0.1 ],
		"MAX" : 		[ 1.0, 1.0 ],
     	"MIN" : 		[ -1.0, -1.0 ]
	},
	{
     	"NAME" :		"seed",
     	"TYPE" : 		"float",
     	"DEFAULT" :		23519.62531,
     	"MIN" : 		4334.94437,
     	"MAX" :			29712.15073
	},
	{
		"NAME" : 		"delta",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.125,
		"MIN" : 		0.0,
		"MAX" : 		0.5
	},
	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" : 	40.0,
		"MIN" : 		0.0,
		"MAX" : 		50.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.15,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	},
	{
		"NAME":			"color",
		"TYPE": 		"color",
		"DEFAULT": 		[ 0.7, 1.0, 0.0, 1.0 ]
	},
	{
		"NAME":			"bgcolor",
		"TYPE": 		"color",
		"DEFAULT": 		[ 0.2, 0.0, 8.0, 1.0 ]
	},
	{
      "NAME": "style",
      "TYPE": "long",
      "VALUES": [
        0,
        1,
        2,
        3
      ],
      "LABELS": [
        "Maze",
        "Circles",
        "Triangles",
        "Wild"
      ],
      "DEFAULT": 1
	}
	]
}*/

////////////////////////////////////////////////////////////
// MotionMazeMaker  by mojovideotech
//
// License: 
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

float hash (in vec2 p) { 
	float TT = TIME*rate*0.00001;
    return fract(sin(dot(p.xy,pattern.xy*vec2(sin(TT),cos(TT)))*seed));
}

vec2 Pattern(in vec2 st, in float index){
    index = fract(((index-0.5)*2.0));
    if (index > 0.85+(delta*0.2)) { st = vec2(1.0) - st; } 
    else if (index > 0.75-(delta*delta)) { st = vec2(1.0-st.x,st.y); } 
    else if (index > 0.65-delta) { st = 1.0-vec2(1.0-st.x,st.y); }
    return st;
}

void main() {
    vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
    uv.x *= RENDERSIZE.x/RENDERSIZE.y;
    uv.xy += TIME*-(motion.xy)*0.5;
    uv *= 55.0-floor(scale);
    vec2 ipos = floor(uv);
    vec2 fpos = fract(uv); 
    vec2 tile = Pattern(fpos, hash(ipos));
    float Mcol = smoothstep(tile.x-0.3,tile.x,tile.y)-smoothstep(tile.x,tile.x+0.3,tile.y);
	float Ccol = (step(length(tile),0.6)-step(length(tile),0.4)) +
             	 (step(length(tile-vec2(1.0)),0.6)-step(length(tile-vec2(1.0)),0.4));
	float Tcol = step(tile.x,tile.y);
	vec3 col=bgcolor.rgb;
	if (style==3) { 
		col = vec3((1.0-Tcol)-Ccol,(Ccol-Tcol)+(0.5-Tcol)-Mcol,Tcol-Ccol); }
	else if (style==0) { col += Mcol*color.rgb-Mcol*bgcolor.rgb; }
	else if (style==1) { col += Ccol*color.rgb-Ccol*bgcolor.rgb; }
	else if (style==2) { col += Tcol*color.rgb-Tcol*bgcolor.rgb; }
	gl_FragColor = vec4(col,1.0);
}

