
/*{
    "CREDIT": "by MTO",
    "CATEGORIES": [
    ],
    "INPUTS": [
    ],
}*/


varying vec2        texOffsets[5];


void main() {
  
    gl_FragColor = vec4(texOffsets[0].x, texOffsets[1].x, 0, 1);
}
