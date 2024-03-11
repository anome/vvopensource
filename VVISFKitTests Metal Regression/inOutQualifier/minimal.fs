/*{
	"DESCRIPTION": "Basic shader using in keyword",
	"CREDIT": "MTO",
}*/

	
vec4 turnGreen(vec3 _)
{
    return vec4(0,100,0,1);
}

void main(void) {
    vec3 a = vec3(1,1,1);
    gl_FragColor = vec4(1,1,1,1);
}
