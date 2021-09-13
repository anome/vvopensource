/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "FractilianFoundation",
  "CATEGORIES": [
    "generator",
    "fractal",
    "2d",
    "iteration"
  ],
  "INPUTS": []
}*/

///////////////////////////////////////////
// FractilianFoundation  by mojovideotech
// orgin of the Fractilian series of ISFs
//
// based on :
//
// Fractals: MRS
// by Nikos Papadopoulos, 4rknova / 2015
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
///////////////////////////////////////////

#ifdef GL_ES
precision mediump float;
#endif

void main( void )
{
    vec2 uv = .987 * gl_FragCoord.xy / RENDERSIZE.y;
    float t = TIME*.01125, k = cos(t), l = sin(t);        
    float s = .456;
    for(int i=0; i<96; ++i) 
    {
        uv  = abs(uv) - s;  
        uv *= mat2(k,-l,l,k);
        s  *= 0.963;         
    }
    float x = .5 + .5*cos(6.28318*(337.*length(uv)));
    gl_FragColor = vec4(0.0,0.0,0.0,1.0);
    gl_FragColor += vec4(vec3(length(uv)*37.0,0.755*x, 0.955/x),0.95);
}