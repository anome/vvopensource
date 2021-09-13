/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "INPUTS": [
    {
      "MAX": [
        1,
        1
      ],
      "MIN": [
        0,
        0
      ],
      "NAME": "mouse",
      "TYPE": "point2D"
    },
    			{
            "NAME": "rate",
            "TYPE": "float",
           "DEFAULT": 0.1,
            "MIN": 0.05,
            "MAX": 1.5
        }
  ]
}
*/


// PrimordialRainbow by mojovideotech

#ifdef GL_ES
precision mediump float;
#endif


vec3 hsv(in float h, in float s, in float v) {
	return mix(vec3(1.0), clamp((abs(fract(h + vec3(3, 2, 1) / 3.0) * 6.0 - 3.0) - 1.0), 0.0 , 1.0), s) * v;
}

void main( void ) {

	vec3 p = vec3(vv_FragNormCoord,mouse.xy)*2.0;
	vec4 color = vec4(0);
	float m = 1.0;
	float t = TIME*rate;
	vec2 c = vec2 (sin(t),cos(t));
	float n = 2.5;
	const int iter = 4;
	for (int i = 0; i < iter; i++) {
		float l = length(p);;
		m *= smoothstep(0.0, 1.0, l);
		p /= l*l*0.2;
		p.xy = vec2(atan(c.x*c.y)*p.x - acos(c.x)*p.y, asin(c.x)*p.y-acos(c.x)*p.x);
		p.xz = vec2(atan(c.x*c.y)*p.x - acos(c.y)*p.z, asin(c.y)*p.z-acos(c.y)*p.x);
		p = abs(mod((p), n)-n/2.0);
		
		color += vec4(hsv(l, 1.0, 1.0), 1.0);
	}

	gl_FragColor = vec4( color/float(iter) )*m;

}