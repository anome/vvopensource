/*{
    "DESCRIPTION": "Your shader description",
    "CREDIT": "by you",
    "CATEGORIES": [
        "Your category"
    ],
      "INPUTS": [
    {
      "NAME": "noise",
      "TYPE": "float",
      "MIN": 0.1,
      "MAX": 1.0,
      "DEFAULT": 0.25
    },
          ]
}*/


void main(void) {
    gl_FragColor = vec4(1,noise,1,1);
}
