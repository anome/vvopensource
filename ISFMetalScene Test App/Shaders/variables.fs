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
        gl_FragColor = vec4( gl_FragCoord.x / RENDERSIZE.x, gl_FragCoord.y / RENDERSIZE.y, 0, 1);
    }
    else if( type == 1 )
    {
         gl_FragColor = vec4( vv_FragNormCoord.x,  vv_FragNormCoord.y, 0, 1);
    }
    else if( type == 2 )
    {
        gl_FragColor = vec4( isf_FragNormCoord.x,  isf_FragNormCoord.y, 0, 1);
    }
    else if (type ==3 )
    {
        gl_FragColor = vec4(isf_fragCoord.x / RENDERSIZE.x, isf_fragCoord.y / RENDERSIZE.y, 0, 1);
    }
}

