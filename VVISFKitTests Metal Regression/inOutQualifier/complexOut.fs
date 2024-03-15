/*{
    "DESCRIPTION": "Basic shader using in keyword",
    "CREDIT": "MTO",
}*/

    
void turnGreen(out vec4 color)
{
    color = vec4(0,100,0,1);
}

void turnRed( out vec4 fragColor, in vec2 fragCoord )
{

    fragColor = vec4(1,0.2,0.2,1.0);
}

void main(void) {
//    vec3 a = vec3(1,1,1);
    turnRed(gl_FragColor, vec2(1,1));
}
