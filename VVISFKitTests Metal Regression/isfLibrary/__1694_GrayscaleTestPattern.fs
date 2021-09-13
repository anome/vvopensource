/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "test pattern"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
             {
            "NAME": "bars",
            "TYPE": "float",
           "DEFAULT": 16,
            "MIN": 4,
            "MAX": 32
        },
                     {
            "NAME": "vh",
            "TYPE": "float",
           "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 1.0
        }
  ]
}
*/
// GrayscaleTestPattern by mojovideotech 

# ifdef GL_ES
precision mediump float;
# endif


void main()
{
	float h = (gl_FragCoord.x / RENDERSIZE.x);
	float v = (gl_FragCoord.y / RENDERSIZE.y);
	
	float b = floor(bars);
	float gh = (floor(h*b)/b+(h/b));
	float gv = (floor(v*b)/b+(v/b));
	float g = mix(gh,gv,vh);
	
	gl_FragColor = vec4(g,g,g,1.0);
}