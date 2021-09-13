/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [
    ""
  ],
  "INPUTS": []
}*/

////////////////////////////////////////////////////////////
// ChromathematicalPoem#1  by mojovideotech
//
// based on
// glslsandbox.com/\e#28114.4
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#ifdef GL_ES
precision highp float;
#endif


float hash( float n ) {
	float T = TIME;
	return fract((sin(n)/log2(T))-(mod(n,-T))/55.0);
}

float noise( in vec3 x ) {
    vec3 p = floor(x), f = fract(x);
    f = f*f*(3.0-2.0*f);
    float n = p.x + p.y*157.0 + 113.0*p.z;
    return mix(mix(mix( hash(n+0.0), hash(n+1.0),f.x),
                   mix( hash(n+157.0), hash(n+158.0),f.x),f.y),
               mix(mix( hash(n+113.0), hash(n+114.0),f.x),
                   mix( hash(n+270.0), hash(n+271.0),f.x),f.y),f.z);
}

float flux(vec3 mo) {
	float v = 0.0, m = 1.0;
	for (int i=0; i < 3; i ++) {
		m *= 0.497149872694134;
		v += noise(mo) * m;
		mo *= 1.1356352767379;
	}
	return v;
}

float pallet(vec3 s) {
	float f = flux(s);
	return floor(f*8.0);
}

void main( void ) {
	vec2 uv  = isf_FragNormCoord*21.0;
	float f1 = pallet(vec3(uv, TIME * 0.05 ));
	float f2 = pallet(vec3(uv, TIME * 0.05 + 2.));
	float f3 = pallet(vec3(uv, TIME * 0.05 + 3.));
	vec4 color  = (f1 < 3.0) ? vec4(0.294, 0.057, 0.355, 1.0) : vec4(1.0, 0.280, 0.037, 1.0);
	color += (f2 < 3.0) ? color : vec4(0.541, 0.073, 0.337, 1.0);
	color *= (f3 < 3.0) ? color : vec4(0.851, 0.020, 0.237, 1.0);

	gl_FragColor = color;
}