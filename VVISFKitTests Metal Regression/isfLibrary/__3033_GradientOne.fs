/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [],
  "INPUTS": [
    {
        "NAME":"inputImage",
        "TYPE":"image"
    },
    {
      "NAME": "R",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "G",
      "TYPE": "float",
      "DEFAULT": 0.75,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "B",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "C",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "M",
      "TYPE": "float",
      "DEFAULT": 0.9,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "Y",
      "TYPE": "float",
      "DEFAULT": 0.9,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "waves",
      "TYPE": "float",
      "DEFAULT": 0.005,
      "MIN": 0.005,
      "MAX": 3.14
    },
        {
      "NAME": "ZOOM",
      "TYPE": "float",
      "DEFAULT": 0.005,
      "MIN": 0.005,
      "MAX": 3.14
    },
    {
      "NAME": "pos",
      "TYPE": "point2D",
      "DEFAULT": [
        0.5,
        0.5
      ],
      "MIN": [
        -2,
        -2
      ],
      "MAX": [
        2,
        2
      ]
    },
    {
      "NAME": "pos2",
      "TYPE": "point2D",
      "DEFAULT": [
        0.5,
        0.5
      ],
      "MIN": [
        -2,
        -2
      ],
      "MAX": [
        2,
        2
      ]
    }
  ]
}*/
// RotoGradientWave1 by mojovideotech
// based on :
// http://glslsandbox.com/e#26993.2

#ifdef GL_ES
precision mediump float;
#endif

float pi = 3.14159265359;
void main( void ) {

	vec2 position = gl_FragCoord.xy/RENDERSIZE.x-pos*ZOOM*TIME;
    float w = pi-waves;
	float color = length(position.xy/vec2(ZOOM))*pos.x;
	vec4 c = vec4(vec3(cos(color*1.25+w*0.667/2.667),sin(color*1.333+w/3.333),sin(color*4.0/w)),1.0);
    c *= vec4(0.1+R,0.2+G,0.1+B,1.0);
    c += vec4(1.0-C,1.0-M,1.0-Y,1.0);
    
    position = gl_FragCoord.xy/RENDERSIZE.x-pos2*sin(TIME*0.0012);
    color = length(position.xy/vec2(ZOOM))*pos.y;
    c += vec4(vec3(cos(color*1.25+w*0.667/2.667),sin(color*1.333+w/3.333),sin(color*4.0/w)),1.0);
    c *= vec4(0.1+R,0.2+G,0.1+B,1.0);
    c += vec4(1.0-C,1.0-M,1.0-Y,1.0);
    
    
    
    gl_FragColor = c;
}