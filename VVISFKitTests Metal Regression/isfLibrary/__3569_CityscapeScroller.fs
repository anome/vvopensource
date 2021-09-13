/*{
	"CREDIT": "by mojovideotech",
	"CATEGORIES" : [
	"generator",
    "scrolling",
    "city"
  ],
  "INPUTS" : [
  	{
		"NAME" : 		"sky",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.25,
		"MIN" : 		0.5,
		"MAX" : 		3.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	-0.5,
		"MIN" : 		-1.0,
		"MAX" : 		1.0
	}
  ],
  "DESCRIPTION" : "based on http://glslsandbox.com/e#40042.0"
}
*/

////////////////////////////////////////////////////////////
// CityscapeScroller   by mojovideotech
//
// based on : 
// glslsandbox.com/e#40042.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////



vec3 atmos(vec2 uv) {
	float z = (0.1 / uv.y * sky);
	vec3 co = vec3(0.3,0.4,1.0) * z;
	co = pow(co, 1.0 - vec3(z));  
	return max(co, 0.0); 
}

float hash(float p) { return fract(sin(p)*28657.); }

float noise(vec2 p) { return hash(p.x + p.y*337.); }

float city(vec2 uv){ return 1.0 - step(hash(floor(uv.x * 13.0)) * 0.5 + 0.05, uv.y); }

void main( void ) {
	float T = TIME * rate;
	vec2 pos = gl_FragCoord.xy / RENDERSIZE.y;
	pos.x += T * 0.1;
	vec3 color = atmos(pos);
	color /= 1.0 + color;
	color = mix(color, vec3(0.0), city(pos)*0.4 );
	color = mix(color, vec3(0.0), city(vec2(pos.x+(T*0.0125),pos.y+0.05)*0.6));

	gl_FragColor = vec4(color, 1.0);
}