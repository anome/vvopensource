/*{
	"CREDIT": "by axiomcrux",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
		{
			"NAME":"scale",
			"TYPE":"float",
			"DEFAULT":0.5
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				1.0,
				1.0,
				1.0
			]
		}
	]
}*/

void main() {
// Created by inigo quilez - iq/2014
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

    vec2  px = (isf_FragNormCoord-0.5)*64.*scale;
    float id = 0.5 + 0.5*cos(TIME + sin(dot(floor(px+0.5),vec2(113.1,17.81)))*43758.545);
    vec3  co = 0.5 + 0.5*cos(TIME + 3.5*id + vec3(0.0,1.57,3.14) );
    vec2  pa = smoothstep( 0.0, 0.2, id*(0.5 + 0.5*cos(6.2831*px)) );
    
    gl_FragColor = vec4( co*pa.x*pa.y, 1.0 )*colorInput;
}
