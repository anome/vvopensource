/*{
    "CATEGORIES": [
        "XXX"
    ],
    "CREDIT": "",
    "DESCRIPTION": "",
    "INPUTS": [
        {
            "DEFAULT": 0,
            "LABEL": "x_pos",
            "MAX": 1,
            "MIN": 0,
            "NAME": "x_pos",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.1,
            "LABEL": "x_size",
            "MAX": 1,
            "MIN": 0,
            "NAME": "x_size",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0,
            "LABEL": "y_pos",
            "MAX": 1,
            "MIN": 0,
            "NAME": "y_pos",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.1,
            "LABEL": "y_size",
            "MAX": 1,
            "MIN": 0,
            "NAME": "y_size",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.001,
            "LABEL": "smooth_shape",
            "MAX": 1,
            "MIN": 0.001,
            "NAME": "smooth_shape",
            "TYPE": "float"
        },
        {
            "DEFAULT": [
                1,
                1,
                1,
                1
            ],
            "LABEL": "color_input",
            "NAME": "color_input",
            "TYPE": "color"
        }
    ],
    "ISFVSN": "2",
    "VSN": null
}
*/

float res = (RENDERSIZE.x/RENDERSIZE.y);

float smoothedge(float v) {
    return smoothstep(smooth_shape, 0. / RENDERSIZE.x, v);
}

float line(vec2 p, vec2 size) {  
  vec2 d = abs(p) - size;
  return min(max(d.x, d .y), 0.0) + length(max(d,0.0));
}

void main()	{

	vec2 screenspace = isf_FragNormCoord;
	screenspace.x *= (RENDERSIZE.x/RENDERSIZE.y);
	
	
	float d = line(screenspace - vec2(0.0, (x_pos+x_size*.025)), vec2(screenspace.x, x_size));
    d = min(d, line(screenspace - vec2((y_pos*res)-(y_size*.025), 0.), vec2(y_size, screenspace.y)));
   
    vec3 color = vec3(smoothedge(d));
   
    gl_FragColor = vec4(color.r*color_input.r, color.g*color_input.g,color.b*color_input.b, 1.);
   
}