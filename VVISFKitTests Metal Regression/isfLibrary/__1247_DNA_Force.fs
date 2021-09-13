/*{
	"CREDIT": "by joshpbatty",
	"DESCRIPTION": "DNA Force",
	"CATEGORIES": [
		"XXX"
	],
	  "INPUTS": [
	{
		"NAME": "inputImage",
		"TYPE": "image"
	},
   	{
		"NAME": "zoom",
		"TYPE": "float",
		"DEFAULT": 70.0,
		"MIN": 10.0,
		"MAX": 100.0
	},
 	{
		"NAME": "speed",
		"TYPE": "float",
		"DEFAULT": 0.3,
		"MIN": 0.01,
		"MAX": 0.5
	},
   	{
		"NAME": "iter",
		"TYPE": "float",
		"DEFAULT": 1.5,
		"MIN": 0.01,
		"MAX": 2.0
	},
	{
		"NAME": "forceRadiusX",
		"TYPE": "float",
		"DEFAULT": 0.47,
		"MIN": 0.0,
		"MAX": 2.0
	},
 	{
		"NAME": "forceRadiusY",
		"TYPE": "float",
		"DEFAULT": 0.0,
		"MIN": 0.0,
		"MAX": 2.0
	}
  ]
}*/


void main()
{
    //screen coordinates
    vec2 p = isf_FragNormCoord.xy; // fragCoord.xy / iResolution.xy;
    p = p*2.-1.;
    //vec2 p = -1.0 + 2.0 * gl_FragCoord.xy / resolution.xy;
    // mouse	
    //vec2 m = mouse.xy / resolution.xy;

 //   vec2 radius = forceRadius;
 //   radius.y = forceRadius;

    vec2 radius = vec2(forceRadiusX,forceRadiusY);
//    float radius = forceRadius;

    vec2 force = vec2(.0,0.0);

    for( float i=1.0; i<8.0; i++ )
    {
        // nucleus center and power 
        vec3 h;
        h.x = cos(.1*(TIME*speed)*(i))*(radius.x*i);
        h.y = cos(.1*(TIME*speed)*(i+.5))*(radius.y*i);
        h.z = 8.0- i;
        radius = radius * .9;

        // calculate distance and sum up force
        vec2 d1 = vec2(h.x-p.x, h.y-p.y);
        float d = (d1.x*d1.x)+(d1.y*d1.y);
        if (d != 0.0) force = force + (1.0/(zoom*d))*d1*h.z;
    }
    
    vec3 col;
    col.r = 1.0-(abs(force.x*iter));
    col.g = 1.0-(abs(force.y*iter)*abs(force.x*iter)*2.0*abs(1.0-p.y));
    col.b = 1.0-(abs(force.y*iter));

    vec2 uv;
    uv.x = col.r;
    uv.y = col.b;
    
    uv = fract(uv*2.0);
    vec3 texcol = IMG_NORM_PIXEL(inputImage, uv).xyz;

    //set the output color
//    gl_FragColor = vec4(texcol,1.0);
 //   gl_FragColor = vec4(texcol*2.0,1.0);
    gl_FragColor = vec4(texcol*col,1.0);
//    gl_FragColor = vec4(texcol*2.0*col,1.0);
}



