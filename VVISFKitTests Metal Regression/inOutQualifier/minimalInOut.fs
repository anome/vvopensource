/*{
    "DESCRIPTION": "Basic shader using in keyword",
    "CREDIT": "MTO",
}*/

    
void turnGreen(inout vec4 color)
{
    color = vec4(0,100,0,1);
}

void main(void) {
//    vec3 a = vec3(1,1,1);
    turnGreen(gl_FragColor);
}
