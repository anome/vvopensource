/*{
	"CREDIT": "by joshpbatty",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "rot_speed",
			"TYPE": "float",
			"DEFAULT": 0.025,
			"MIN": 0.01,
			"MAX": 0.2
		},
		{
			"NAME": "rot_offset",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

void main() {
	
	vec2 R = RENDERSIZE.xy;
	vec2 U = gl_FragCoord.xy;
    U = (U+U-R)/R.x;
    float t = rot_speed*(TIME-(rot_offset * 100.)), r = 1.0, c,s;
    
    vec4 O;
    //O -= O;
    for( int i=0; i< 49; i++){
	    U *= mat2(c=cos(t),s=sin(t),-s,c),
        r /= abs(c) + abs(s),
        O = smoothstep(3./R.y, 0., max(abs(U.x),abs(U.y)) - r) - O;
    }
    
	gl_FragColor = O;
}