/*{
	"CREDIT": "by Carter Rosenberg",
	"CATEGORIES": [
		"Generator"
	],
	"INPUTS": [
		{
			"NAME": "width",
			"TYPE": "float",
			"DEFAULT": 0.25
		},
		{
			"NAME": "offset",
			"TYPE": "float",
			"DEFAULT": 0.0
		},
		{
			"NAME": "vertical",
			"TYPE": "bool",
			"DEFAULT": 0.0
		},
		{
			"NAME": "color1",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				1.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "color2",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				0.0,
				1.0
			]
		}
	]
}*/
float box(vec2 _st, vec2 _size, float _smoothEdges){
    _size = vec2(0.5)-_size*0.5;
    vec2 aa = vec2(_smoothEdges*0.5);
    vec2 uv = smoothstep(_size,_size+aa,_st);
    uv *= smoothstep(_size,_size+aa,vec2(1.0)-_st);
    return uv.x*uv.y;
}

float random (vec2 st) { 
    return fract(sin(dot(st.xy,
                         vec2(12.9898,78.233)))
                 * 43758.5453123);
}
vec2 truchetPattern(in vec2 _st, in float _index){
    _index = fract(((_index-0.5)*2.0));
    if (_index > 0.75) {
        _st = vec2(1.0) - _st;
    } else if (_index > 0.5) {
        _st = vec2(1.0-_st.x,_st.y);
    } else if (_index > 0.25) {
        _st = 1.0-vec2(1.0-_st.x,_st.y);
    }
    return _st;
}

float noise (vec2 st) {
    vec2 i = floor(st);
    vec2 f = fract(st);

    // Four corners in 2D of a tile
    float a = random(i);
    float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0));
    float d = random(i + vec2(1.0, 1.0));

    // Smooth Interpolation

    // Cubic Hermine Curve.  Same as SmoothStep()
    vec2 u = f*f*(3.0-2.0*f);
    u = smoothstep(0.,1.,f);

    // Mix 4 coorners porcentages
    return mix(a, b, u.x) + 
            (c - a)* u.y * (1.0 - u.x) + 
            (d - b) * u.x * u.y;
}
float lines(in vec2 pos, float scale, float b){
    pos *= scale;
    return smoothstep(0.0,
                    .5+b*.5,
                    abs((sin(pos.x*3.1415)+b*1.0))*.5);
}

vec2 rotate2D(vec2 _st, float _angle){
    _st -= 0.5;
    _st =  mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle)) * _st;
    _st += 0.5;
    return _st;
}

vec2 tile(vec2 _st, float _zoom){
    _st *= _zoom;
    return fract(_st);
}


void main() {
	//	determine if we are on an even or odd line
	//	math goes like..
	//	mod(((coord+offset) / width),2)
	
	
	vec4 out_color = color2;
	float coord = vv_FragNormCoord[0];

	if (vertical)	{
		coord = vv_FragNormCoord[1];
	}
	if (width == 0.0)	{
		out_color = color1;
	}
	else if(mod(((coord+offset) / width),2.0) <= 1.0)	{
		out_color = color1;
	}
	
	gl_FragColor = out_color;
}