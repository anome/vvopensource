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
            "TARGET": "halfSizeBaseRender",
            "WIDTH": "floor($WIDTH/2.0)",
            "HEIGHT": "floor($HEIGHT/2.0)",
            "DESCRIPTION": "Pass 0"
        },
        {
            "TARGET": "quarterSizeBaseRender",
            "WIDTH": "floor($WIDTH/4.0)",
            "HEIGHT": "floor($HEIGHT/4.0)",
            "DESCRIPTION": "Pass 1"
        },
        {
            "TARGET": "eighthSizeBaseRender",
            "WIDTH": "floor($WIDTH/8.0)",
            "HEIGHT": "floor($HEIGHT/8.0)",
            "DESCRIPTION": "Pass 2"
        },
        {
            "TARGET": "quarterGaussA",
            "WIDTH": "floor($WIDTH/4.0)",
            "HEIGHT": "floor($HEIGHT/4.0)",
            "DESCRIPTION": "Pass 3"
        },
    ]
}*/





void main() {
    gl_FragColor = vec4(isf_FragNormCoord.x, isf_FragNormCoord.y, 0, 1);
}
