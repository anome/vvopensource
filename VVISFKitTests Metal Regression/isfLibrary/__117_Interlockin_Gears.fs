/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#27870.0",
  "INPUTS": [
      {
            "NAME": "size",
            "TYPE": "float",
           "DEFAULT": 2.0,
            "MIN": 0.5,
            "MAX": 10.0
          },
           {
            "NAME": "rate1",
            "TYPE": "float",
           "DEFAULT": 1.5,
            "MIN": 0.25,
            "MAX": 16.0
          },
          {
            "NAME": "rate2",
            "TYPE": "float",
           "DEFAULT": 2.0,
            "MIN": 0.33,
            "MAX": 20.0
          },
                {
            "NAME": "offset",
            "TYPE": "float",
           "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 0.5
          }
  ]
}
*/

// Interlockin'Gears by mojovideotech
// http://glslsandbox.com/e#27870.0

#ifdef GL_ES
precision mediump float;
#endif


void main( void ) {

	vec2 distanceVector = (gl_FragCoord.xy / RENDERSIZE.xy/size)+offset;
	     distanceVector.x *= RENDERSIZE.x;
	     distanceVector.y *= RENDERSIZE.y;
	float d = 135.0;
	float newtime = TIME * sign(mod(distanceVector.x, d * 2.0) - d) * sign(mod(distanceVector.y, d * 2.0) - d);
	
	     distanceVector.x = mod(distanceVector.x, d) - d / 2.0;	
	     distanceVector.y = mod(distanceVector.y, d) - d / 2.0;
	
	float angle = atan(distanceVector.x, distanceVector.y);
	
	float scaleA = 0.0002;
	float scaleB = 0.00067;
	float cogMinA = 0.1;   // outer edge 
	float cogMinB = 0.025;
	float cogMaxA = 0.667;  // inner edge, size of hole
        float cogMaxB = 0.85;
	
	float distanceA = 1.0 - scaleA * ((distanceVector.x * distanceVector.x) + (distanceVector.y * distanceVector.y));
	float distanceB = 1.125 - scaleB * ((distanceVector.x * distanceVector.x) + (distanceVector.y * distanceVector.y));
	float toothAdjustA = 2.5 * (distanceA - cogMinA);
	float toothAdjustB = 2.25 * (distanceB - cogMinB);
	float angleFooA = sin(angle*12.0 - newtime * rate1) - toothAdjustA;
	float angleFooB = sin(angle*11.0 + newtime * rate2) - toothAdjustB;
     
             cogMinA += 12.0 * clamp(1.0 * (angleFooA + 0.25), 0.0, 0.02); // thickness of cog ring
	     cogMinB += 10.0 * clamp(1.0 * (angleFooB + 0.3), 0.0, 0.033);
	
	float isSolidA = clamp(2.0 * (distanceA - cogMinA) / cogMaxA, 0.0, 1.0);  // anti-aliasing
	      isSolidA = clamp(isSolidA * 50.0 * (cogMaxA - distanceA), 0.0, 1.0);
	float isSolidB = clamp(2.0 * (distanceB - cogMinB) / cogMaxB, 0.0, 1.0);
	      isSolidB = clamp(isSolidB * 40.0 * (cogMaxB - distanceB), 0.0, 1.0);
	
	float brightness = isSolidA *(1.0-sin(angle*2.0+newtime)*0.5);
	      brightness += isSolidB *(0.95-cos(angle*2.0-newtime)*0.35);
	
	gl_FragColor = vec4(brightness*0.4,brightness*0.4,brightness*0.5, 1.0 );
}