/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "oldschool",
    "circle",
    "moire",
    "pink",
    "bitplane",
    "Automatically Converted"
  ],
  "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/ldX3DN by Lanza.  Classic oldschool effect, pink flavour. <br/>Remembering Amiga bitplanes...<br/>No ray-thinger, nor 3D, just plain plane 2D effect.",
  "IMPORTED": [],
  "INPUTS": [
    {
      "NAME": "UVSpace",
      "TYPE": "bool",
      "DEFAULT": 1
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0.1,
      "MAX": 1.5
    },
    {
      "NAME": "phase1",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "phase3",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -0.99,
      "MAX": 0.99
    },
    {
      "NAME": "phase2",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "phase4",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -0.99,
      "MAX": 0.99
    },
    {
      "NAME": "thickness",
      "TYPE": "float",
      "DEFAULT": 0.8,
      "MIN": 0.5,
      "MAX": 0.9
    },
    {
      "NAME": "freq",
      "TYPE": "float",
      "DEFAULT": 17,
      "MIN": 3,
      "MAX": 99
    },
    {
      "NAME": "offset",
      "TYPE": "float",
      "DEFAULT": 300,
      "MIN": 1,
      "MAX": 900
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 6,
      "MIN": 1,
      "MAX": 20
    },
    {
      "NAME": "color",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0.05,
      "MAX": 1
    }
  ]
}*/


void main()
{
	// Fragment coords relative to the center of viewport, in a 1 by 1 coords sytem.
//	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

     float T = sqrt(TIME)*sin(TIME*0.05);
	
	    vec2 uv = (2.0*gl_FragCoord.xy-RENDERSIZE)/RENDERSIZE.y*1.5;

	// But I want circles, not ovales, so I adjust y with x resolution.
	vec2 homoCoords = vec2( uv.x, zoom* (gl_FragCoord.y -offset)/RENDERSIZE.x );

	// Sin of distance from a moving origin to current fragment will give us..... 
	vec2 movingOrigin1 = vec2(sin(phase1*23.125+T),+sin(phase2*22.999-T));
	
	// ...numerous... 
	float frequencyBoost = freq; 
	
	// ... awesome concentric circles.
	float wavePoint1 = sin(distance(movingOrigin1, homoCoords)*frequencyBoost);
	
	// I want sharp circles, not blurry ones.
	float blackOrWhite1 = max(wavePoint1,movingOrigin1.x);
	
	// That was cool ! Let's do it again ! (No, I dont want to write a function today, I'm tired).
	vec2 movingOrigin2 = vec2(-cos(phase3*13.1313+T),-sin(phase4*6.667-T));
	float wavePoint2 = sin(distance(movingOrigin2, homoCoords)*frequencyBoost);
	float blackOrWhite2 = mod(wavePoint2,T);
	
	// I love pink.
	vec3 pink = vec3(1.0, 0.6, 1.0 );
	vec3 darkPink = vec3(0.1, 0.3,1.0);
	
	// XOR virtual machine.
	float composite = blackOrWhite1 * blackOrWhite2;
	
	// Pinkization
	gl_FragColor = vec4(max( pink * composite, darkPink), 1.0);
}