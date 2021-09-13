/*{
	"CREDIT": "by axiomcrux",
	"DESCRIPTION": "",
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
			"MAX": 1.0
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
	]
}*/
#define rate 1.95
#define push_RGB 1.09
#define billow 0.10
#define push_RGB2 0.9209
#define startFrame 50
#define leak 0.150
#define scale 0.1
#define didYOUeatALLthatACID 1.729
#define feedback 0.45

#define iTime TIME
#define iResolution.xy RENDERSIZE
#define texture IMG_NORM_PIXEL

#define iChannel0 inputImage
#define iChannel1 inputImage


vec4 mainImage( out vec4 fragColor, in vec2 fragCoord ){
    
    
    vec2 uv = fragCoord.xy / iResolution.xy;
    
    
    
    //if (iFrame < startFrame) {fragColor = texture(inputImage, uv);} 
    
    //else {
        
        vec2 vUv = fragCoord.xy / iResolution.xy;
        vec2 texel = rate / iResolution.xy;
        vec3 uv = texture(iChannel0, vUv*(1.0 + leak * 0.005)).xyz;
        
        float gt = mod(iTime*vUv.x*vUv.y, billow * 6.1415)*scale;
        
        vec2 d1 = vec2(uv.x * vec2(texel.x*cos(gt * uv.z), texel.y*sin(gt*uv.y)));
        vec2 d2 = vec2(uv.y * vec2(texel.x*cos(gt * uv.x), texel.y*sin(gt*uv.y)));
        vec2 d3 = vec2(uv.z * vec2(texel.x*cos(gt * uv.y), texel.y*sin(gt*uv.y)));
        
        float bright = (uv.x+uv.y+uv.z)/ push_RGB + push_RGB2;
        
        float r = texture(iChannel0, vUv+ d1 * bright).x;
        float g = texture(iChannel0, vUv+ d2 * bright).y;
        float b = texture(iChannel0, vUv+ d3 * bright).z;
        
        vec3 uvMix = mix(uv, vec3(r,g,b), didYOUeatALLthatACID);
        
        vec3 orig = texture(iChannel1, vUv).xyz;
        
        return vec4(mix(uvMix, orig, 0.50-feedback), 1.0);
        
    //}
} 

void main() {
	gl_FragColor = mainImage(inputImage, isf_FragNormCoord.xy);
}