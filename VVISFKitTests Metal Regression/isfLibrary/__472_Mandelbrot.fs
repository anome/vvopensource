/*{
	"CREDIT": "by PALUCK",
	"CATEGORIES" : [ "Factal de The Art of Code"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
    {
      "MAX": [
        1.0,
        2.0
      ],
      "MIN": [
        -1.0,
        1.0
      ],
      "DEFAULT": [
        0.0,
        1.0
      ],
      "NAME": "Zoom",
      "TYPE": "point2D"
    },
    {
      "MAX": [
        1.0,
        1.0
      ],
      "MIN": [
        -1.0,
        -1.0
      ],
      "DEFAULT": [
        0.0,
        0.0
      ],
      "NAME": "Position",
      "TYPE": "point2D"
    },
    {
      "MAX": [
        1.0,
        1.0
      ],
      "MIN": [
        -1.0,
        -1.0
      ],
      "DEFAULT": [
        0.0,
        0.0
      ],
      "NAME": "PositionFine",
      "TYPE": "point2D"
    },
	{
     	"NAME" :		"Variant",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0.5,
     	"MIN" : 		-2.0,
      	"MAX" :			2.0
	},
  	{
     	"NAME" :		"Variation",
     	"TYPE" : 		"bool",
    	"DEFAULT" :		0
	},
    {
      "MAX": [
        1.0,
        1.0
      ],
      "MIN": [
        -1.0,
        -1.0
      ],
      "DEFAULT": [
        0.2,
        0.0
      ],
      "NAME": "Forme",
      "TYPE": "point2D"
    }, 
    {
			"NAME": "inputImage",
			"TYPE": "image"
	}

    ]
}
*/

uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;


void main() 
{
    vec2 uv = ( gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
	float zoom = pow(10.0, -Zoom.x*Zoom.y*3.0);
	
	vec2 c = uv*zoom*3.0;
	c += vec2 (-0.69955, 0.37999)* (Position+PositionFine*0.01)*5.0;
	
	vec2 z = vec2 (0.0);
	float iter = 0.0;
	
	const float max_iter =1000.0;	
	float ma = 100.0;
	for (float i=0.0; i<max_iter; i++) {
		z = vec2(z.x*z.x - z.y*z.y, Variant*5.0*z.x*z.y) + c;
		ma = min(ma,abs(z.x*Forme.x- z.y*Forme.y));
		if(abs(dot(z,z*c*0.1)) > 4.0) break;
		iter++;
	}
	float f = iter / max_iter;
	if (Variation) {
	f = pow(f, 0.5);
	f = ma;
	};
	vec3 col = vec3(f);
	col = IMG_NORM_PIXEL(inputImage,vec2(f,1.0)).rgb;
	    gl_FragColor = vec4(col, 1.0);
}