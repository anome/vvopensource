/*{
  "CREDIT": "by mojovideotech, Enhanced by zerbzman",
  "CATEGORIES": [
    "test pattern",
    "gradient"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "NAME": "bars",
      "TYPE": "float",
      "DEFAULT": 12,
      "MIN": 4,
      "MAX": 32
    },
    {
      "NAME": "vh",
      "TYPE": "float",
      "DEFAULT": 0.7,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "frontColor",
      "LABEL": "Front Color",
      "TYPE": "color",
      "DEFAULT": [
        1,
        0,
        0,
        1
      ]
    },
    {
      "NAME": "backColor",
      "LABEL": "Back Color",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        1,
        1
      ]
    },
    {
      "NAME": "invert",
      "LABEL": "Invert",
      "TYPE": "bool",
      "DEFAULT": false
    }
  ]
}*/
// GrayscaleTestPattern by mojovideotech 

# ifdef GL_ES
precision mediump float;
# endif


void main()
{
	float h = (gl_FragCoord.x / RENDERSIZE.x);
	float v = (gl_FragCoord.y / RENDERSIZE.y);
	
	float b = floor(bars);
	float gh = (floor(h*b)/b+(h/b));
	float gv = (floor(v*b)/b+(v/b));
	float g = mix(gh,gv,vh);
	
	if (invert) g = 1. - g;
	
	vec4 color = mix(backColor, frontColor, g);
	
	gl_FragColor = color;
}