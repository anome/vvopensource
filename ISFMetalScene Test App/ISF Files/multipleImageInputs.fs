/*{
    "CREDIT": "by Anomes",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
         {
             "NAME": "secondInputImage",
             "TYPE": "image"
         }
    ]
}*/


void main()
{
    vec4 color = IMG_THIS_PIXEL(secondInputImage);
     vec4 color2 = IMG_THIS_PIXEL(inputImage);
    gl_FragColor = vec4(color.r, color2.g, color.b, 1);
}
