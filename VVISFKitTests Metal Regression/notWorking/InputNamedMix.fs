/*{
    "CREDIT": "by MTO",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "mix",
            "TYPE": "float",
            "MIN": 0,
            "MAX": 1.0,
            "DEFAULT": 0.2
        }
    ]
}*/



void main() {
    gl_FragColor = vec4(mix, 0.0, mix, 1.0);
}

