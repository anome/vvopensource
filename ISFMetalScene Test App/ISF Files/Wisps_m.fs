// SaturdayShader Week 30 : Wisps
// by Joseph Fiola (http://www.joefiola.com)
// 2016-03-12

// Based on Week 29 Saturday Shader + "WAVES" Shadertoy by bonniem
// https://www.shadertoy.com/view/4dsGzH


/*{
 "CREDIT": "Joseph Fiola",
 "INPUTS": [
 {
 "NAME": "lines",
 "TYPE": "float",
 "DEFAULT": 100.0,
 "MIN": 1.0,
 "MAX": 200.0
 },
 {
 "NAME": "linesStartOffset",
 "TYPE": "float",
 "DEFAULT": 0.0,
 "MIN": 0.0,
 "MAX": 1.0
 },
 {
 "NAME": "amp",
 "TYPE": "float",
 "DEFAULT": 0.15,
 "MIN": 0.0,
 "MAX": 1.0
 },
 {
 "NAME": "glow",
 "TYPE": "float",
 "DEFAULT": -12.0,
 "MIN": -40.0,
 "MAX": 0.0
 },
 {
 "NAME": "mod1",
 "TYPE": "float",
 "DEFAULT": 1.0,
 "MIN": 0.0,
 "MAX": 1.0
 },
 {
 "NAME": "mod2",
 "TYPE": "float",
 "DEFAULT": 0.01,
 "MIN": -1.0,
 "MAX": 1.0
 },
 {
 "NAME": "twisted",
 "TYPE": "float",
 "DEFAULT": 0.1,
 "MIN": -0.5,
 "MAX": 0.5
 },
 {
 "NAME": "zoom",
 "TYPE": "float",
 "DEFAULT": 8.0,
 "MIN": 0.0,
 "MAX": 100.0
 },
 {
 "NAME": "rotateCanvas",
 "TYPE": "float",
 "DEFAULT": 0.0,
 "MIN": 0.0,
 "MAX": 1.0
 },
 {
 "NAME": "scroll",
 "TYPE": "float",
 "DEFAULT": 0.3,
 "MIN": 0.0,
 "MAX": 1.0
 },
 {
 "NAME": "pos",
 "TYPE": "point2D",
 "DEFAULT": [0.5,0.5],
 "MIN":[0.0,0.0],
 "MAX":[1.0,1.0]
 }
 ]
 }*/


#define PI 3.14159265359
#define TWO_PI 6.28318530718
mat2 rotate2d (float _angle, const device IsfMetalInputsBufferType& isf_metal_inputs, const device IsfMetalBuiltInsBufferType& isf_metal_builtIns, IsfMetalRuntimeBuiltIns isf_metal_runtimeBuiltIns) {
    return mat2(cos(_angle), -sin(_angle), sin(_angle), cos(_angle));
}

void main () {
    float2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    uv -= float2(pos);
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uv *= zoom;
    uv = rotate2d(rotateCanvas * -TWO_PI, isf_metal_inputs, isf_metal_builtIns, isf_metal_runtimeBuiltIns) * uv;
    float3 wave_color = float3(0.000000);
    float wave_width = 0.010000;
    for (float i = 0.000000; i < 200.000000; i++) {
        uv = rotate2d(twisted * -TWO_PI, isf_metal_inputs, isf_metal_builtIns, isf_metal_runtimeBuiltIns) * uv;
        if (lines <= i) break;
        uv.y += sin(sin(uv.x + i * mod1 + (scroll * TIME * TWO_PI)) * amp + (mod2 * PI));
        if (lines * linesStartOffset - 1.000000 <= i) {
            wave_width = abs(1.000000 / (50.000000 * uv.y * glow));
            wave_color += float3(wave_width, wave_width, wave_width);
        }
    }
    gl_FragColor = float4(wave_color, 1.000000);
}

