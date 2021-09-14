/*{
    "CREDIT": "by MTO",
}*/

// GL is less precise, making black stripes on some values

void main() {
    float well = isf_FragNormCoord.x / isf_FragNormCoord.x;
    float thisShoulsBeOne = floor(well);
    gl_FragColor = vec4(thisShoulsBeOne, thisShoulsBeOne, 0.0, 1.0);
}
