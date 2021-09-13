/*{
	"CREDIT": "by lennyjpg",
	"DESCRIPTION": "Lorem ipsum",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
				{
			"NAME": "backgroundImage",
			"TYPE": "image"
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
			"NAME": "Radius",
			"TYPE": "float",
			"DEFAULT": 0.37,
			"MIN": 0.0,
			"MAX": 2.0
		},
			{
			"NAME": "Angle",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
				{
			"NAME": "level",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},

		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		}
	]
}*/

void main() {
	vec2 mouse = pointInput;
	vec2 uv = isf_FragNormCoord.xy;
    float time = TIME *  speed * 1.5;
    float m = 0.5;
    vec2 ak = isf_FragNormCoord.xy-0.5;
    float a = Angle;
    ak = mat2(cos(a),-sin(a),sin(a),cos(a)) * ak + .5;
	ak.y-=0.5;
    ak.y*=0.05;
    ak.y+=0.5;
    ak.y = m;
    vec4 fu = IMG_NORM_PIXEL(inputImage, ak);
    vec4 bgi = IMG_NORM_PIXEL(backgroundImage, isf_FragNormCoord);
    bgi.rgb *= colorInput.rgb;
    
    vec3 blur    = vec3(.1,      .3,     .2) * 3.1;   
    vec3 speed   = vec3(1.3,    .14,    2.12),
    // amplitude  = vec3(.223,  .21,   .14),
     amplitude  = vec3(fu.r,  fu.g,  fu.b),
     frequency  = vec3(2.12,   4.6,   7.5) * .2,
     wave = vec3( sin(uv.x * frequency - time * speed) * amplitude + level ),
     smoothwave = vec3(smoothstep(wave, wave + blur, uv.yyy));
     
     float ratio = RENDERSIZE.x/RENDERSIZE.y;
     vec2 ce = isf_FragNormCoord.xy -0.5;
     ce.x*=ratio;
     
     float k = distance(ce,pointInput-0.5);
     float d = smoothstep(k*0.99,k,Radius);
	 vec3 final = mix(bgi.rgb,smoothwave,d);

	gl_FragColor = vec4(final,1.0);
}



