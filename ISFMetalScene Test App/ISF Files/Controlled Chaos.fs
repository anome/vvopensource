// SaturdayShader Week 14 : ControlledChaos
// by Joseph Fiola (http://www.joefiola.com)
// 2015-11-21
// Based on Patricio Gonzalez Vivo's "Using the Chaos" example on http://patriciogonzalezvivo.com/2015/thebookofshaders/10/ @patriciogv ( patriciogonzalezvivo.com ) - 2015


/*{
    "CREDIT": "Joseph Fiola",
	"INPUTS": [
		{
			"NAME": "pos",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			],
			"MIN": [
				0.0,
				0.0
			],
			"MAX": [
				1.0,
				1.0
			]
		},
		{
			"NAME": "invert",
			"TYPE": "bool"
		},
		{
			"NAME": "function",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8
			],
			"LABELS": [
				"floor",
				"fract",
				"abs",
				"tan",
				"atan",
				"sin",
				"mod",
				"mod grid",
				"clamp"
			],
			"DEFAULT": 8
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": -2.0,
			"MAX": 2.0
		},
		{
			"NAME": "multiplier",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": -2.0,
			"MAX": 2.0
		},
		{
			"NAME": "grid",
			"TYPE": "float",
			"DEFAULT": 10.0,
			"MIN": 1e-4,
			"MAX": 20.0
		},
		{
			"NAME": "detail",
			"TYPE": "float",
			"DEFAULT": 0.05,
			"MIN": 1e-4,
			"MAX": 0.1
		},

		{
			"NAME": "contrast",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 0.5
		},
		{
			"NAME": "contrastShift",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -0.5,
			"MAX": 0.5
		},
		{
			"NAME": "mode",
			"TYPE": "long",
			"VALUES": [
				0,
				1
			],
			"LABELS": [
				"clean",
				"noisy"
			],
			"DEFAULT": 0
		}
	]
}*/



#ifdef GL_ES
/* TRANSPILER ERROR: <unknown token 490> */;
#endif
float random (float2 st, const device IsfMetalInputsBufferType& isf_metal_inputs, const device IsfMetalBuiltInsBufferType& isf_metal_builtIns, IsfMetalRuntimeBuiltIns isf_metal_runtimeBuiltIns) {
    if (mode == 0) return sin(dot(st.xy + TIME * speed, float2(12.989800, 78.233002 * 2.000000 * multiplier))) * 40.000000 * 1.000000 * detail; else if (mode == 1) return fract(sin(dot(st.xy + TIME * speed * 0.000100, float2(12.989800, 78.233002 * 2.000000 * multiplier))) * 43758.546875 * 1.000000 * detail);
    
}

float3 invertColor (float3 color, const device IsfMetalInputsBufferType& isf_metal_inputs, const device IsfMetalBuiltInsBufferType& isf_metal_builtIns, IsfMetalRuntimeBuiltIns isf_metal_runtimeBuiltIns) {
    return float3(color * -1.000000 + 1.000000);
}

void main () {
    float2 st = gl_FragCoord.xy / RENDERSIZE.xy;
    st -= float2(pos);
    st.x *= RENDERSIZE.x / RENDERSIZE.y;
    st *= grid;
    float2 ipos = st;
    if (function == 0) ipos = floor(st); else if (function == 1) ipos = fract(st); else if (function == 2) ipos = abs(st); else if (function == 3) ipos = tan(st); else if (function == 4) ipos = atan(st); else if (function == 5) ipos = sin(st); else if (function == 6) ipos = mod(st.xy, st.yx); else if (function == 7) ipos = mod(st.xy, st.xy); else if (function == 8) ipos = clamp(st.xy, float2(-2.000000 + multiplier), float2(2.000000 - multiplier));
    
    
    
    
    
    
    
    
    float2 fpos = fract(st);
    float3 color = float3(random(ipos, isf_metal_inputs, isf_metal_builtIns, isf_metal_runtimeBuiltIns));
    color = smoothstep(0.000000 + contrast + contrastShift, 1.000000 - contrast + contrastShift, color);
    if (invert) color = invertColor(color, isf_metal_inputs, isf_metal_builtIns, isf_metal_runtimeBuiltIns);
    gl_FragColor = float4(color, 1.000000);
}

