/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	{
      "NAME" : "Radius1",
      "TYPE" : "float",
      "MAX" : 3.0,
      "DEFAULT" : 2.0,
      "MIN" :2.0
    },
    {
      "NAME" : "BEAMS",
      "TYPE" : "float",
      "MAX" : 60.0,
      "DEFAULT" : 12.0,
      "MIN" : 6.0
    },
    {
      "NAME" : "petals",
      "TYPE" : "float",
      "MAX" : 8.0,
      "DEFAULT" : 3.0,
      "MIN" : 1.0
    },
     {
      "NAME" : "Limbs",
      "TYPE" : "float",
      "MAX" : 0.45,
      "DEFAULT" : 0.3,
      "MIN" : -2.0
    },
    {
      "NAME" : "Edge",
      "TYPE" : "float",
      "MAX" : 12.0,
      "DEFAULT" : 0.8,
      "MIN" : 0.1
    },
    {
      "NAME" : "Fade",
      "TYPE" : "float",
      "MAX" : 0.5,
      "DEFAULT" : 0.05,
      "MIN" : 0.0
    }
    ]
	
}*/

#ifdef GL_ES
precision mediump float;
#endif
#define PI 3.14



void main(){
     vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;

    vec3 color = vec3(0.0);

    vec2 pos = vec2(0.5,0.5)-st ;
     float tempo;
 tempo = clamp (TIME, 1.0, 9.0);
    float RADIUS = length(pos)*Radius1;
    float a = atan(pos.y,pos.x);

    float f = cos(TIME *(a*petals));
    f = abs(cos( a / BEAMS)* sin((tempo * a * petals))) + (sin (TIME * Edge))/2.0 + Limbs;
  
   

    color = vec3( smoothstep(f ,f+2.
   ,RADIUS ) ) - vec3 (Fade);
    
   
    gl_FragColor = vec4(color,1.0);
   
	gl_FragColor.b *= 0.0 ;
	gl_FragColor.g *= 0.3 ;
	gl_FragColor.r *= 1. ;

}

