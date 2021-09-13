
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 200.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/
float sin01(float a){
    return sin(a)*0.5 + 0.5;
}

vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec4 hsvGlitch(){
    vec2 uv = isf_FragNormCoord.xy;
    vec4		inputPixelColor;

    inputPixelColor = IMG_THIS_PIXEL(inputImage);
    float maxx = max (1., 0.);

    vec3 hsv = rgb2hsv(inputPixelColor.xyz);
    
    float angle = hsv.x + atan(uv.y, uv.x) + TIME;
    mat2 RotationMatrix = mat2( cos( angle ), -sin( angle ), sin( angle ),  cos( angle ));
  //  vec3 col = texture(iChannel0, uv + RotationMatrix * vec2(log(max(SIN01(iTime*0.7)-0.2, 0.)*0.2 + 1.  ),0) * hsv.y).rgb;

    float sinn = log (max ( (sin(TIME*0.7)*0.5 + 0.5) - 0.2 , -10.)*0.1 + 2. );
    vec2 veee = vec2(sinn,0.);
    vec3 col = IMG_NORM_PIXEL(inputImage, uv + RotationMatrix * veee * hsv.y).xyz;


	return vec4(col,1.0);
    

}
void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	//	both of these are also the same
	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
		//gl_FragColor = inputPixelColor;
	gl_FragColor = hsvGlitch();
}
