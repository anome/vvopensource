/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#25956.0",
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
    }
  ]
}
*/

// PsychedeliaGenerica by mojovideotech
// copied from :
// http://glslsandbox.com/e#25956.0

#ifdef GL_ES
precision mediump float;
#endif


vec3 hueToRGB(float hue) {
    return clamp( 
        abs(mod(hue * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 
        0.0, 1.0);
}

float map(vec3 p) {
	float  t = .0;//length(mod(p.xz, 4.0) - 2.0) - 0.97;
	t = max(-t, 4.0 - dot(p, vec3(0, 1, 0)) + (sin(p.x * 0.2) * sin(p.z * .4)));
	t = min(t, length(mod(p.xz, 20.0) - 10.0) - 1.4);
	//t = min(t, length(mod(p.xy, 10.0) - 5.0) - 0.2);
	//t = min(t, length(mod(p.yz, 10.0) - 5.0) - .2);
	return t;
}

vec2 rot(vec2 p, float t)
{
	return vec2(
		cos(t) * p.x - sin(t) * p.y,
		sin(t) * p.x + cos(t) * p.y);
		
}
void main( void ) {

	vec2 uv = ( gl_FragCoord.xy / RENDERSIZE.xy ) * 2.0 - 1.0;
	uv.x   *= RENDERSIZE.x / RENDERSIZE.y;
	uv.y    = -uv.y;
	//if(abs(uv.y) > 0.75) discard;
	
	vec3 dir = normalize(vec3(uv, 1));
	
	dir.yz = rot(dir.yz, (mouse.y - 0.5) * 3.0);
	dir.xz = rot(dir.xz, (mouse.x - 0.5) * -3.0);
	
	vec3 pos = vec3(0., 0., TIME);
	
	float t = 0.0;
	for(int i = 0 ; i < 20; i++) {
		float temp = map(pos + dir * t);
		if(temp <  0.01) break;
		t += temp;
	}
	
	vec3 ip = pos + dir * hueToRGB(t);
	gl_FragColor.rgb = vec3(hueToRGB(map(ip)));
	gl_FragColor.a = 1.0;
}