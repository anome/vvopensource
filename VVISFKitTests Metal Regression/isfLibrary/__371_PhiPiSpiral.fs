/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES" : [
   ""
  ],
  "INPUTS" : [
	
		    {
      "MAX": [
        1.0,
        1.0
      ],
      "MIN": [
        -1.0,
        -1.0
      ],
      "DEFAULT":[0.0,0.0],
      "NAME": "offset",
      "TYPE": "point2D"
    },
     {
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 0.8,
			"MIN": -2,
			"MAX": 2
		},
			{
			"NAME": "radius",
			"TYPE": "float",
			"DEFAULT": 0.9,
			"MIN": 0.1,
			"MAX": 3.0
		},
		{
			"NAME": "multiplier",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.001,
			"MAX": 0.5
		},
     {
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 7.6,
			"MIN": 0.01,
			"MAX": 9.0
		},
     {
			"NAME": "zoomwarp",
			"TYPE": "float",
			"DEFAULT": 0.02,
			"MIN": 0.0,
			"MAX": 1.0
		},
    {
			"NAME": "rotozoom",
			"TYPE": "float",
			"DEFAULT": 0.8,
			"MIN": -1,
			"MAX": 3
		},
		{
			"NAME": "colors",
			"TYPE": "bool",
			"DEFAULT": "FALSE"
		},
		{
		     "NAME": "R",
            "TYPE": "float",
           "DEFAULT": 0.05,
            "MIN": -1.0,
            "MAX": 1.5
        },
         {
            "NAME": "G",
            "TYPE": "float",
           "DEFAULT": 0.67,
            "MIN": -1.0,
            "MAX": 1.5
        },
         {
            "NAME": "B",
            "TYPE": "float",
           "DEFAULT": 1.1,
            "MIN": -1.0,
            "MAX": 1.5
        },
        {
			"NAME": "invert",
			"TYPE": "bool",
	        "DEFAULT": "FALSE"
		}
  ],
  "DESCRIPTION" : ""
}
*/


// PhiPiSpiral by mojovideotech

#ifdef GL_ES
precision mediump float;
#endif

#define     sqfv    2.23606797749979    // sq root of 5
#define 	pepi 	23.140692632779269 	// powe(pi);
#define 	pisq  	9.869604401089359	// pi squared, pi^2
#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	pi   	3.141592653589793 	// pi
#define 	e     	2.718281828459045 	// eulers number
#define 	phisq  	2.6180339887499		// phi squared, phi^2
#define 	prphi	2.028876065463213	// pi root of phi
#define 	sqpi 	1.772453850905516	// square root of pi
#define 	phi   	1.618033988749895 	// golden ratio
#define		erpi   	1.523671054858932   // e root of pi
#define 	cupi  	1.464591887561523   // cube root of pi
#define 	prpi 	1.439619495847591 	// pi root of pi
#define 	phrphi 	1.34636082003487	// phi root of phi
#define 	rcphi  	0.61803398874989	// reciprocal of pi  , 1/phi 
#define 	rcpipi 	0.027425693123298 	// reciprocal of pipi  , 1/pipi

vec3 HRGB(vec3 hue) 
{
  return clamp(abs(mod(hue * 6. + vec3(0.,R,2.),G) - B) - 0.,1.,0.);
}

void main( void ) {
	vec2 pos = ( gl_FragCoord.xy / RENDERSIZE.xy ) - vec2(0.5,0.5);
	pos -= vec2(offset);
	vec2 position = vec2(pos)*(9.9-zoom)/2.0;
	float r = (position.x*position.x+position.y*position.y)+zoomwarp;
	position += vec2(position.x/r, position.y/r);
	vec3 col = vec3 (0.0);
	float T = TIME * rate + 1000.;
	float TT = TIME * 0.25 + 1000.;
	float t2 = pow((atan(TT * rcpipi)/9973.0),multiplier);
	float t2e = phrphi*exp(t2-floor(t2)-rotozoom)*radius;
	float color = (sin( position.x*t2e * cos( T /sqpi ) * 79.0 ) + cos( position.y*t2e * cos( T / twpi ) * 13.0 ))*(1.0-cos(t2*2.0*pi));
	t2 /= prpi;
	t2e -= prphi*exp(t2-floor(t2)-rotozoom);
	color /= (sin( position.y*t2e * sin( T / phisq ) * 43.0 ) + cos( position.x*t2e * sin( T / e ) * 37.0 ))*(1.0-cos(t2*2.0*pi));
    t2 /= cupi;
	t2e -= rcphi*exp(t2-floor(t2)-rotozoom);
	color += (sin( position.x*t2e * sin( T / erpi ) * 101.0 ) + sin( position.y*t2e * sin( T / pisq ) * 83.0 ))*(1.0-cos(t2*2.0*pi));
	r *= pow(phi,sqfv);
	color -= r/log2(pepi-r/inversesqrt(r));
	col.x -= mix(color + sin(TT/pi),color,G+B)-R; 
	if (colors) {
		col.y += mix(color + cos(TT/pi),color,B+R)+G; 
	}
	else {
		col.y -= mix(color + cos(TT/pi),color,B+R)+G; 
	}
	col.z -= mix(color - cos(TT/pi),color,R)-B;
	col *= HRGB(col);
	if (invert) {
		col = col * -1.0 + 1.0;
	}
	gl_FragColor = vec4(vec3(col), 1.0 );
    gl_FragColor.a *= clamp(exp(0.1-color),0.0,2.5);
}


