//#SaturdayShader
//2015-01 StroboSphere
//Based on code from http://patriciogonzalezvivo.com/2015/thebookofshaders/07/


/*{
	"CREDIT": "by vjzef",
	"DESCRIPTION": "Stroboscopic sphere",
	"CATEGORIES": [
			"Generator"						
	],
	"INPUTS": [
		{
			"NAME": "pos",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			],
			"MIN": [
				0.0,
				0.0
			],
			"MAX": [
				1.0,
				1.0
			]
		},
		{
			"NAME": "time",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "scale1",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "scale2",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "outline",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 0.99
		},	
		{
			"NAME": "roundness",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

float dist(vec2 st){
	//Calculate distance field
	float _d = length( max(abs(st)-roundness,0.) );
	_d = smoothstep(scale1,scale2,_d);
	return _d;
}


void main(){
  // normalize screen resoltuion to values from 0.0 to 1.0 
  vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
  
  // control the position of circle
  st -= vec2(pos);

  // draw in a 1:1 ratio
  st.x *= RENDERSIZE.x/RENDERSIZE.y;


  // set color variable to black
  vec3 color = vec3(0.0);
  
  // set d variable to hold distance value
  float d = dist(st);

   //color = vec3(fract(d*sin(d+time*speed)));
   color = vec3(fract(d*sin(-time)));
   
   // generate outline
   if (outline > 0.0)
   color = step(outline, color);
   

  // draw to screen
  gl_FragColor = vec4(color,1.0);

}