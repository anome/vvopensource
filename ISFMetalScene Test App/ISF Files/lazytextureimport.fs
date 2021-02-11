

/*{
    "DESCRIPTION": "Amatorka FX",
    "CREDIT": "by IMIMOT (ported from https://github.com/BradLarson/GPUImage)",
    "CATEGORIES": [
        "Film"
    ],
    "INPUTS": [
        {
            "NAME": "type",
            "TYPE": "long",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 10.0
                
        }
    ],
    "IMPORTED": {
        "hola": {
            "PATH": "gandalf.jpg"
        }
    }
}*/

void main()
{
  if( type == 0 )
    {
        gl_FragColor = IMG_THIS_PIXEL(hola);
    }
    else if( type == 1 )
    {
        gl_FragColor = IMG_NORM_THIS_PIXEL(hola);
      
    }
    else if( type == 2 )
    {
         vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
          gl_FragColor = IMG_NORM_PIXEL(hola, uv);
    }
    else if (type ==3 )
    {
        vec2 uv = gl_FragCoord.xy;
        gl_FragColor = IMG_PIXEL(hola, uv);
    }
//    else if (type == 4)
//    {
//        vec2 uv =
//        gl_FragColor = IMG_NORM_PIXEL(hola, uv);
//    }
}
