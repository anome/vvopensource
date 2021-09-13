
/*{
    "CREDIT": "by MTO",
}*/


void main() {

    float minusOneToOne = 2.0*(isf_FragNormCoord.y-0.5); // Generation: OK
    float somePow = pow(minusOneToOne,3.0);
    gl_FragColor = vec4(abs(somePow), somePow, somePow, 1.0);

}
