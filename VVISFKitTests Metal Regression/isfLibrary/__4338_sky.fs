/*{
  "CATEGORIES": [
    "INKA"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#36876.1",
  "INPUTS": [
  ]
}*/

const float PI = 3.141592654;
const float size = 0.5;
float zoom = fract(TIME)/4.;

vec4 star(vec2 uv, float zoom, float seed)
{
	uv *= zoom;
	vec2 s = floor(uv);
	vec2 f = fract(uv);
	vec2 p = .5 + .440 * (sin(s + PI)) * sin(11. * fract(sin((s + seed) * mat2(7.5, 3.3, 6.2, 5.4)) * 55.)) - f;
	float d = length(p);
	float k = smoothstep(d*.9, d, 0.025 * size);
	float shades = 2.0;
	vec4 color = vec4(
		(shades/(shades-1.0))*mod(floor(shades*uv.y)/shades, 1.0),
		(shades/(shades-1.0))*mod(floor(shades*uv.x)/shades, 1.0),
		(shades/(shades-1.0))*mod(floor(shades*uv.x)/shades, 1.0), 
		1.
	);
    color =  vec4(1.0, 1.0, 1., 1.0);
    return vec4(k* color.r, k * color.g, k * color.b, k);
}

void main(void)
{
    float layers = 5.;
	float phase = (fract(TIME)/5.) *PI;
	float blue = 1.-((isf_FragNormCoord.x/2.) + (isf_FragNormCoord.y));
	float opacity = (isf_FragNormCoord.x + isf_FragNormCoord.y);
	vec2 uv = (gl_FragCoord.xy*2.-RENDERSIZE.xy) / min(RENDERSIZE.x,RENDERSIZE.y); 
	uv *= mat2(cos(phase), -sin(phase), sin(phase), cos(phase));
	vec4 c= vec4(0.,0., blue, opacity);
	for(float i = 0.; i < 5. ; i += 1.)
	{
		vec4 dust = star(uv, mod(layers + i - zoom * layers, layers), i * 5.);
		c = mix(c, dust, dust.a*c.a);
	}
	gl_FragColor = vec4(c);
}