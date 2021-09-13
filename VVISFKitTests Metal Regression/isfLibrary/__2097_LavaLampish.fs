/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
  "INPUTS": [
        {
      "MAX": [
        20.0,
        1000.0
      ],
      "MIN": [
        1.0,
        10.0
      ],
      "DEFAULT":[4.0,400.0],
      "NAME": "mouse",
      "TYPE": "point2D"
    },
                     {
            "NAME": "slider",
            "TYPE": "float",
           "DEFAULT": 6.66,
            "MIN": -10.00,
            "MAX": 20.00
        }
  ]
}
*/

// LavaLampish by mojovideotech
// based on :
// http://glslsandbox.com/e#26106.1

#ifdef GL_ES
precision mediump float;
#endif



void main( void ) {

	vec4 final = vec4(0,0,0,0);
	vec2 position = gl_FragCoord.xy / vec2(RENDERSIZE.x,RENDERSIZE.y);
	float t = TIME-sin(TIME*1.6180339)/mouse.y;
	float width = mouse.x+(log(TIME)/log2(TIME));
	// float distanceFromPoint = distance(sin(log2(position*2.5)+1.5),vec2(0.505,0.4995));
	float distanceFromPoint = distance(tan(cos(log2(position/sqrt(TIME))/width)-(slider*2.0)),vec2(0.5,0.0));
	if(floor(-width/distanceFromPoint) == floor(-width/distanceFromPoint-cos(distanceFromPoint-TIME)))
	{
		final.x = acos(position.y);
	}
	float distanceFromPoint2 = distance(sin(log2(position)*slider),vec2(-2.0,-.5));
	if(floor(width/distanceFromPoint2) == floor(width/distanceFromPoint2-sin(distanceFromPoint2+TIME)))
	{
		final.y = cos(distanceFromPoint2);
	float distanceFromPoint3 = distance(sin(position/(log2(width+t)*0.025)+slider),vec2(-0.05,1.1665));
	if(floor(width/distanceFromPoint3) == floor(width/distanceFromPoint3-tan(distanceFromPoint3+TIME)))
	{
		final.z = tan(distanceFromPoint);
	}
	
	gl_FragColor = vec4(final.x-0.6,final.y-0.2,final.z+0.3,0.7);
	}
}