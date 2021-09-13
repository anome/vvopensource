/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [ ""
  ],
  "INPUTS": [
             {
            "NAME": "density",
            "TYPE": "float",
           "DEFAULT": 1000,
            "MIN": 99,
            "MAX": 9999
        },
         {
            "NAME": "speed",
            "TYPE": "float",
           "DEFAULT": 9,
            "MIN": 1,
            "MAX": 99
        },
                 {
            "NAME": "offset",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": -1.0,
            "MAX": 1.5
        }
  ]
}
*/

// MoireQuasar by mojovideotech\
// based on :
// http://glslsandbox.com/e#25941.0


#ifdef GL_ES
precision mediump float;
#endif


void main( void ) {
	vec2 p = ( gl_FragCoord.xy / RENDERSIZE.xy ) -offset;
	p.x *= RENDERSIZE.x/RENDERSIZE.y;
	float angle = floor(density) * atan(p.y,p.x) + speed * TIME;
	
	gl_FragColor = vec4(sin(angle));
	
}