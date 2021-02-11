
/*{
    "CREDIT": "by MTO",
    "ISFVSN": "2",
    "CATEGORIES": [
        "Blur"
    ],
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
    ],
    "PASSES": [
        {
            "TARGET": "pass0Render",
            "WIDTH": "floor($WIDTH/2.0)",
            "HEIGHT": "floor($HEIGHT/2.0)",
            "DESCRIPTION": "Pass 0"
        },
        {
            "TARGET": "pass1Render",
            "WIDTH": "floor($WIDTH/4.0)",
            "HEIGHT": "floor($HEIGHT/4.0)",
            "DESCRIPTION": "Pass 1"
        },
    ]
}*/





void main() {
    if(PASSINDEX==0) {
         gl_FragColor = IMG_THIS_PIXEL(inputImage);
    } else if(PASSINDEX==1) {
         gl_FragColor = IMG_THIS_PIXEL(pass0Render);
        
    }
}
