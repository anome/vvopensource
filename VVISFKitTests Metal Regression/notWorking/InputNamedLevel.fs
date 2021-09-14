/*{
    "CREDIT": "by MTO",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "NAME": "level",
            "TYPE": "float",
            "MIN": 0,
            "MAX": 1.0,
            "DEFAULT": 0.2
        }
    ]
}*/



void main() {
    gl_FragColor = vec4(level, 0.0, level, 1.0);
}

