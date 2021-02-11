/*{
    "CREDIT": "by Anomes",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
         {
             "NAME": "type",
             "TYPE": "long"
         },
    ],
}*/



void main()
{
  if( type == 0 )
    {
        gl_FragColor = IMG_THIS_PIXEL(inputImage);
    }
    else if( type == 1 )
    {
        gl_FragColor = IMG_NORM_THIS_PIXEL(inputImage);
      
    }
    else if( type == 2 )
    {
         vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
          gl_FragColor = IMG_NORM_PIXEL(inputImage, uv);
    }
    else if (type ==3 )
    {
        vec2 uv = gl_FragCoord.xy;
        gl_FragColor = IMG_PIXEL(inputImage, uv);
    }
//    else if (type == 4)
//    {
//        vec2 uv =
//        gl_FragColor = IMG_NORM_PIXEL(inputImage, uv);
//    }
}

