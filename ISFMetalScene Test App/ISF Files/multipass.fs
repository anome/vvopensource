/*{
    "CREDIT": "Anomes",
    "DESCRIPTION": "https://www.shadertoy.com/view/MsjyW3",
    "CATEGORIES": [
        "generator"
    ],
    "INPUTS": [],
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


/* Original shader at https://www.shadertoy.com/view/MsjyW3 */







// ----------------
// COMPOSITE PASSES
// ----------------

void main()
{
    if( PASSINDEX == 0 )
    {
        gl_FragColor = vec4(isf_FragNormCoord.y,0,0,1);
    }
    else if( PASSINDEX == 1 )
    {
        gl_FragColor = IMG_THIS_PIXEL(pass0Render);
//        return;
        if(gl_FragCoord.x > 405) {
            gl_FragColor = vec4(isf_FragNormCoord.y,isf_FragNormCoord.y,0,0.5);
        }
//        else {
//            gl_FragColor = vec4(0,1,0,1); // this writes over pass0
//        }
      
    }
    else if( PASSINDEX == 2 )
    {
        gl_FragColor = IMG_THIS_PIXEL(pass1Render);
        if(gl_FragCoord.x < 305) {
            gl_FragColor = vec4(0,isf_FragNormCoord.y,isf_FragNormCoord.y,0.5);
        }
//        else if(305 < gl_FragCoord.x && gl_FragCoord.x < 505) {
//              gl_FragColor = IMG_THIS_PIXEL(pass0Render);
//        }
//       else {
//             gl_FragColor = vec4(1,1,1,1); // this writes over pass0
//       }
    }
    else {
        gl_FragColor = vec4(0,0,isf_FragNormCoord.y,1);
    }
}





