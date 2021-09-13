/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [
  		"generator",
  		"grid"
  ],
  "INPUTS": [
    {
      "MAX":    [ 10, 10 ],
      "MIN":    [ -10, -10 ],
      "DEFAULT":[ 0.5, -0.5 ],
      "NAME":   "offset",
      "TYPE":   "point2D"
    },
    {
      "MAX":    [ 0.1, 0.01 ],
      "MIN":    [ 0.0001, 0.00001 ],
      "DEFAULT":[ 0.0003, 0.00125 ],
      "NAME":   "matrix",
      "TYPE":   "point2D"
    },
    {
      "NAME":   "size",
      "TYPE":   "float",
      "DEFAULT":30,
      "MIN":    9,
      "MAX":    120
    },
    {
      "NAME":   "speed",
      "TYPE":   "float",
      "DEFAULT":20,
      "MIN": 	1.1,
      "MAX": 	32
    },
    {
      "NAME": 	"hue",
      "TYPE": 	"float",
      "DEFAULT":0,
      "MIN": 	-0.3,
      "MAX": 	0.3
    }
  ]
}*/

///////////////////////////////////////////
// RetroTechicolorComputer  by mojovideotech
//
// based on :
// glslsandbox.com/\e#3613.1
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////

#ifdef GL_ES
precision mediump float;
#endif


float round(float v) {
	if(v - floor(v) >= 0.5) return floor(v)+1.0;
	else return floor(v);
}

vec2 round(vec2 v) {
	vec2 ret = vec2(0.0);
	if(v.x - floor(v.x) >= 0.5) ret.x = floor(v.x)+1.0;
	else ret.x = floor(v.x);
	if(v.y - floor(v.y) >= 0.5) ret.y = floor(v.y)+1.0;
	else ret.y = floor(v.y);
	return ret;
}

float triwave(float x) { return 1.0-4.0*abs(0.5-fract(0.5*x + 0.25)); }

float rand(vec2 co) {
	float t = round(TIME*speed), tt = round(log2(TIME));
    return fract(sin(dot(co.xy, vec2(matrix.xy)))*tt*t);
}

void main() {
	float pixelsize = floor(size);
	vec2 position = (gl_FragCoord.xy);
	vec3 color = vec3(0.0);
	vec2 rposition = round(((position-(pixelsize/2.0))/pixelsize));
	color = vec3(rand(rposition)-hue,rand(rposition+offset.x)-abs(hue*1.25),rand(rposition+offset.y)+hue);
	color *= vec3(abs(log2((position.x+size))*0.5) / abs(log2((position.y+size))*0.5));
	color *= vec3(clamp(abs(triwave(position.x/pixelsize))*3.0 , 0.125, 0.9 ));
	color *= vec3(clamp(abs(triwave(position.y/pixelsize))*3.0 , 0.125 , 0.9));
	gl_FragColor = vec4(color, 1.0 );
}