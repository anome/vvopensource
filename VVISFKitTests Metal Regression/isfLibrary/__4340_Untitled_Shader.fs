
/*{
    "DESCRIPTION": "transition",
    "CREDIT": "mcr",
    "ISFVSN": "2",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "LABEL": "color",
            "NAME": "color",
            "TYPE": "color",
            "DEFAULT": [
                 0.25,
                 0.59,
                 0.9,
                 1.0
             ]
         }
   ]
}*/
void main()
{
    vec2 pt = isf_FragNormCoord;
    vec4 srcPixel = IMG_NORM_PIXEL(inputImage,pt);
    if ( pt.x > fract(TIME))
    {
        gl_FragColor = vec4(srcPixel.r,srcPixel.g,srcPixel.b,opacity) ;
    }
    else
    {
       gl_FragColor = vec4(color.r,color.g,color.b,opacity) ; 
    }
}