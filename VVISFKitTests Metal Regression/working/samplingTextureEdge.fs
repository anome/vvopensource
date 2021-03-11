/*{
  "DESCRIPTION": "Texture sampling edge cases",
  "CREDIT": "by Anomes",

  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    }
  ],
 "PASSES": [
     {
         "TARGET": "pass0Render",
         "PERSISTENT": true,
         "DESCRIPTION": 0
     },
     {
         "TARGET": "pass1Render",
         "DESCRIPTION": 1
     },
 ]
}*/

void main()
{
    if( PASSINDEX == 0 )
    {
        gl_FragColor = vec4(1.0,0.0,0.0,1.0);
    }
    else if( PASSINDEX == 1 )
    {
        // Verify 0,0 border is not smoothed out
        if(gl_FragCoord.y < 200.) {
            gl_FragColor = IMG_PIXEL(pass0Render, vec2(0.,0.));
            
        } else {
            gl_FragColor = IMG_PIXEL(pass0Render, vec2(1.,1.));
        }
    }
}
