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


float distance (vec2 center, vec2 pt)
{
   return 0.7;
}

void main() {
    float color = distance(vec2(1), vec2(2));
    gl_FragColor = vec4(color, 0.0, color, 1.0);
}

