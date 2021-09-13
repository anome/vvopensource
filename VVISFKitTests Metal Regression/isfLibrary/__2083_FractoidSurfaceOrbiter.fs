/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
	
		  	{
		
      "MAX": [
        3.0,
        3.0
      ],
      "MIN": [
        0.0,
        0.0
      ],
      "DEFAULT":[0.0,0.0],
			"NAME": "center",
			"TYPE": "point2D"
		},
		{
		
      "MAX": [
        10.0,
        10.0
      ],
      "MIN": [
        -10.0,
        -10.0
      ],
      "DEFAULT":[-1.1,-1.1],
			"NAME": "orientation",
			"TYPE": "point2D"
		},
		 {
            "NAME": "z",
            "TYPE": "float",
           "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 10.0
        },
         {
            "NAME": "q",
            "TYPE": "float",
           "DEFAULT": 0.0,
            "MIN": -10.0,
            "MAX": 0.0
        },
		                 {
            "NAME": "distance",
            "TYPE": "float",
           "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 3.0
        },
                       {
            "NAME": "orbital",
            "TYPE": "float",
           "DEFAULT": 0.0333,
            "MIN": 0.00111,
            "MAX": 0.333
        },
                   {
            "NAME": "elliptic",
            "TYPE": "float",
           "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 0.333
        },
        {
            "NAME": "seed",
            "TYPE": "float",
           "DEFAULT": 9.0,
            "MIN": 3.33,
            "MAX": 33.3
        },
                       {
            "NAME": "surfacedepth",
            "TYPE": "float",
           "DEFAULT": 0.000333,
            "MIN": 0.0000111,
            "MAX": 0.00111
        },
         {
            "NAME": "atmosphere",
            "TYPE": "float",
           "DEFAULT": 0.00333,
            "MIN": 0.00001,
            "MAX": 0.009
        },
        {
            "NAME": "altitude",
            "TYPE": "float",
           "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 10.0
        }
	]
}*/

// FractoidSurfaceOrbiter by mojovideotech
// based on :
// http://glslsandbox.com/e#25764.0

#ifdef GL_ES
precision mediump float;
#endif


float map(vec3 p)
{
    const int MAX_ITER = 11;
    const float BAILOUT=3.;
    float Power=seed;

    vec3 v = p;
    vec3 c = v;

    float r=3.333;
    float d=0.9;
    for(int n=0; n<=MAX_ITER; ++n)
    {
        r = length(v);
        if(r>BAILOUT) break;

        float theta = acos(v.z/r);
        float phi = atan(v.y, v.x);
        d = pow(r,Power-0.9)*Power*d+1.111;

        float zr = pow(r,Power);
        theta = theta*Power;
        phi = phi*Power;
        v = (vec3(sin(theta)*cos(phi), sin(phi)*sin(theta), cos(theta))*zr)+c;
    }
    return 0.45*log(r)*r/d;
}


void main( void )
{
    vec2 pos = (gl_FragCoord.xy*2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec3 camPos = vec3(cos(TIME*(orbital+elliptic)), sin(TIME*(orbital-elliptic)),distance + 0.111);
    vec3 camTarget = vec3(-1.333+center.x, -1.333+center.y, q);

    vec3 camDir = normalize(camTarget-camPos);
    vec3 camUp  = normalize(vec3(orientation.x, orientation.y, z));
    vec3 camSide = cross(camDir, camUp);
    float focus = altitude + 1.111;

    vec3 rayDir = normalize(camSide*pos.x + camUp*pos.y + camDir*focus);
    vec3 ray = camPos;
    float m = 3.333;
    float d = 1.111, total_d = 9.0;
    const int MAX_MARCH = 49;
    const float MAX_DISTANCE = 1000.0;
    for(int i=0; i<MAX_MARCH; ++i) {
        d = map(ray);
        total_d += d;
        ray += rayDir * d;
        m += 1.111;
        if(d<surfacedepth) { break; }
        if(total_d>MAX_DISTANCE) { total_d=MAX_DISTANCE; break; }
    }

    float c = (total_d)*atmosphere;
    vec4 result = vec4( 1.0-vec3(c * 0.111, c, c * 0.333) - vec3(0.0333, 0.025, 0.025)*m*0.9, 1.0 );
 gl_FragColor = result;
}