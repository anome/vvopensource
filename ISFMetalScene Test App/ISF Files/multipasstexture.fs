/*{
    "CREDIT": "by Anomes",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
    ],
    "PASSES": [
           {
               "TARGET": "pass0Render",
               "FLOAT": true,
               "DESCRIPTION": 0
           },
           {
               "TARGET": "pass1Render",
               "FLOAT": true,
               "DESCRIPTION": 1
           },
    {
        "TARGET": "pass2Render",
        "FLOAT": true,
        "DESCRIPTION": 2
    }
       ]
}*/



void main()
{
  if( PASSINDEX == 0 )
    {
        gl_FragColor = IMG_THIS_PIXEL(inputImage);
    }
    else if( PASSINDEX == 1 )
    {
        gl_FragColor = IMG_THIS_PIXEL(pass0Render);
      
    }
    else if( PASSINDEX == 2 )
    {
        gl_FragColor = IMG_THIS_PIXEL(pass1Render);
    }
}

