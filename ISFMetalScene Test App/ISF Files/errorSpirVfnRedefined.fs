/*{
 "CREDITS": "mto-anomes"
 }
*/


// SpirV forbids redefinition (works in GL, not in Metal)
int someName = 0;

int someName() {
    return 0;
}

void main() {
    gl_FragColor = vec4(1,0,0,0);
}


