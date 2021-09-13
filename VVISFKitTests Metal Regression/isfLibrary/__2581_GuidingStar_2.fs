/*{
	"DESCRIPTION": "",
	"CREDIT": "Silvia Fabiani",
	"ISFVSN": "2",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
	{
      "NAME" : "Blur",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.7,
      "MIN" : 0.2
          },
          {
      "NAME" : "RED",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.1,
      "MIN" : 0.0
          },
          {
      "NAME" : "GREEN",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.4,
      "MIN" : 0.0
          },
          {
      "NAME" : "BLUE",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.9,
      "MIN" : 0.0
          },
           {
      "NAME": "pinchPoint",
      "TYPE": "point2D",
      "DEFAULT": [
        0,
        0
      ],
      "MIN": [
        -0.05,
        -0.05
      ],
      "MAX": [
        0.05,
        0.05
      ]
    },
    {
      "NAME": "invert",
      "TYPE": "bool"
    }	
	]
	
}*/

#ifdef GL_ES
precision mediump float;
#endif

void main(){
    vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    vec3 color = vec3(0.0);
    
  float Shine = cos (TIME);
  float Beam = cos (TIME*2.5);
  float Magn = sin (TIME);
  float Bing1 = sin (pinchPoint.y *TIME);
  float Bing2 = sin (pinchPoint.x *TIME);
  float BWWB;

	if (invert) BWWB  = 2.0;
	else  BWWB = 1.0;
 

    vec2 pos = vec2(0.5)-st;

    float r = length(pos)*2.0;
    float a = atan((pos.y+Bing1),(pos.x+Bing2));

    float f = cos(a*3.);
    f = abs(cos(a/(Shine/12.0)/2.0)*sin(a/(Beam/8.0)/2.0))*Magn/2.0+0.1;
        color = vec3( BWWB -smoothstep(f,f+Blur,r) );

    gl_FragColor = vec4(color, 1.0);
     gl_FragColor = vec4(color, 1.0);
    gl_FragColor.b *= BLUE;
	gl_FragColor.g *= GREEN;
	gl_FragColor.r *= RED;

}

