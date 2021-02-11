/*{
  "CREDIT": "iq (https://www.shadertoy.com/view/Ml2GWy)",
  "INPUTS": [
    {
      "NAME" : "scale",
      "TYPE" : "long",
      "DEFAULT" : 6,
      "MIN" : 1,
      "MAX" : 15,
      "LABEL" : "Scale",
    },
    {
      "NAME" : "colorInput",
      "TYPE" : "color",
      "DEFAULT" : [
        0.0,
        0.0,
        0.0,
        1
      ],
      "LABEL" : "Color"
    },
  ]
}
*/


// Created by inigo quilez - iq/2015 - www.iquilezles.org
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.


void main()
{
    vec2 pos = isf_FragNormCoord.xy*600. + TIME;
    pos.x *= RENDERSIZE.x/RENDERSIZE.y;

    vec3 col = vec3(0.0);
    for( int i=0; i<scale; i++ ) 
    {
        vec2 a = floor(pos);
        vec2 b = fract(pos);
        
        vec4 w = fract((sin(a.x*7.0+31.0*a.y + 0.01*TIME)+vec4(0.035,0.01,0.0,0.7))*13.545317); // randoms
                
        col += w.xyz *                                   // color
               smoothstep(0.45,0.55,w.w) *               // intensity
               sqrt( 16.0*b.x*b.y*(1.0-b.x)*(1.0-b.y) ); // pattern
        
        pos /= 2.0; // lacunarity
        col /= 2.0; // attenuate high frequencies
    }
    
    col = pow( 2.5*col, vec3(1.0,1.0,0.7)-colorInput.rgb );    // contrast and color shape
    
    gl_FragColor = vec4( col, 1.0 );
}