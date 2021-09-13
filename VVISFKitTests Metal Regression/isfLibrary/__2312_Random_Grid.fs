/*{
	"CREDIT": "by isadoratelesdecastro",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": 
	[
	{
      "NAME": "offset",
      "TYPE": "point2D",
      "DEFAULT": [ 0.5, 0.5 ],
      "MAX": [ 10, 10 ],
      "MIN": [ -10, -10 ]
    },
    {
      "NAME": "range",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0.0,
      "MAX": 1.0
    }
	]
}*/

float random (vec2 st) 
{
    return fract(sin(dot(st.xy, offset))*43758.5453123);
}

void main() 
{
	vec2 st = gl_FragCoord.xy / RENDERSIZE.xy;
	st *= 10.0;
	vec2 floorPos = floor(st);
	vec2 fractPos = fract(st);
	vec3 color = vec3(random(floorPos));
	//vec3 color = vec3(fractPos,range);

    gl_FragColor = vec4(color,1.0);
}




