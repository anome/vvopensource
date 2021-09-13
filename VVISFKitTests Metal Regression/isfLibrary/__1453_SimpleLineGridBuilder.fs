/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "utility",
    "grid"
  ],
  "INPUTS" : [
	{
		"NAME" : 		"X",
		"TYPE" : 		"float",
		"DEFAULT" : 	16.0,
		"MIN" : 		2.0,
		"MAX" : 		512.0
	},
	{
		"NAME" : 		"Y",
		"TYPE" : 		"float",
		"DEFAULT" : 	9.0,
		"MIN" : 		2.0,
		"MAX" : 		256.0
	},
	{
		"NAME" : 		"vline",
		"TYPE" : 		"float",
		"DEFAULT" :     3.0,
		"MIN" : 		2.5,
		"MAX" : 		10.0
	},
	{
		"NAME" : 		"hline",
		"TYPE" : 		"float",
		"DEFAULT" :     3.0,
		"MIN" : 		2.5,
		"MAX" : 		10.0
	},
	{
		"NAME" : 		"R",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	},
	{
		"NAME" : 		"G",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.0,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	},
	{
		"NAME" : 		"B",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.0,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	}
  ]
}
*/


////////////////////////////////////////////////////////////
// SimpleLineGridBuilder  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


bool cell(float a,  float e) { return (a<=e); }

void main() {
	vec2 d = vec2(RENDERSIZE.x / (RENDERSIZE.x * floor(X)), RENDERSIZE.y / (RENDERSIZE.y * floor(Y)));
	vec2 t = floor(vec2(hline,vline)/2.0)/RENDERSIZE.xy; 
	vec2 p = gl_FragCoord.xy / RENDERSIZE.xy;
	if (cell(mod(p.x + t.x, d.x) - t.x, t.x) || cell(mod(p.y + t.y, d.y) - t.y, t.y))
		gl_FragColor = vec4(R,G,B,1.0);

	else gl_FragColor = vec4(0.0);
} 