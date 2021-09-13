/*{
  "CREDIT": "by thedantheman",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "DANTHEMAN"
  ],
  "INPUTS": [
    {
      "NAME": "boolInput",
      "TYPE": "bool",
      "DEFAULT": 1
    },
    {
      "NAME": "colorInput",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        1,
        1
      ]
    },
    {
      "NAME": "colorInput2",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        1,
        1
      ]
    },
    {
      "NAME": "colorInput3",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        1,
        1
      ]
    },
    {
      "NAME": "flashInput",
      "TYPE": "event"
    },
    {
      "NAME": "gridSize",
      "TYPE": "float",
      "LABEL": "Grid Size",
      "DEFAULT": 1,
      "MIN": 1,
      "MAX": 10
    },
    {
      "NAME": "longInputIsAPopUpButton",
      "TYPE": "long",
      "VALUES": [
        0,
        1,
        2
      ],
      "LABELS": [
        "red",
        "green",
        "blue"
      ],
      "DEFAULT": 1
    },
    {
      "NAME": "pointInput",
      "TYPE": "point2D",
      "DEFAULT": [
        0,
        0
      ]
    }
  ]
}*/

// Trinity
// By: Brandon Fogerty
// bfogerty at gmail dot com
// xdpixel.com






#ifdef GL_ES
precision mediump float;
#endif




float line( vec2 a, vec2 b, vec2 p )
{
	vec2 aTob = b - a;
	vec2 aTop = p - a;
	
	float t = dot( aTop, aTob ) / dot( aTob, aTob);
	
	t = clamp( t, 0.0, 1.0);
	
	float d = length( p - (a + aTob * t) );
	d = 1.0 / d;
	
	return clamp( d, 0.0, 1.0 );
}


void main( void ) {

	float aspectRatio = RENDERSIZE.x / RENDERSIZE.y;
	
	vec2 uv = ( mod(isf_FragNormCoord.xy*floor(gridSize), 1.0) );
	int xa = int(isf_FragNormCoord.x*floor(gridSize));
	int xb = int(isf_FragNormCoord.y*floor(gridSize));
	if (mod(float(xa), 2.0) == 1.0) {
	  uv = vec2(1.0, 1.0) - uv;
	}
	
	vec2 signedUV = uv * 2.0 - 1.0;
	signedUV.x *= aspectRatio;

	float freqA = mix( 0.4, 1.2, sin(TIME + 30.0) * 0.5 + 0.5 );
	float freqB = mix( 0.4, 1.2, sin(TIME + 20.0) * 0.5 + 0.5 );
	float freqC = mix( 0.4, 1.2, sin(TIME + 10.0) * 0.5 + 0.5 );
	
	
	float scale = 120.0;
	const float v = 70.0;
	vec3 finalColor = vec3( 0.0 );
	float t = line( vec2(-v, -v), vec2(0.0, v), signedUV * scale );
	finalColor = colorInput3.rgb*t * freqA;
	t = line( vec2(0.0, v), vec2(v, -v), signedUV * scale );
	finalColor += colorInput.rgb*t * freqB;
	t = line( vec2(-v, -v), vec2(v, -v), signedUV * scale );
	finalColor += colorInput2.rgb*t * freqC;
	

	

	gl_FragColor = vec4( finalColor, 1.0 );

}