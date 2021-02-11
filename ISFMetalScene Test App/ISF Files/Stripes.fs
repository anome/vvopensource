/*{
 "CREDIT": "Anomes",
 "INPUTS": [
 {
 "NAME": "width",
 "TYPE": "float",
 "MIN": 0.01,
 "MAX": 2.0,
 "DEFAULT": 0.2
 },
 {
 "NAME": "color1",
 "TYPE": "color",
 "DEFAULT": [
 1.0,
 1.0,
 1.0,
 1.0
 ]
 },
 {
 "NAME": "color2",
 "TYPE": "color",
 "DEFAULT": [
 0.0,
 0.0,
 0.0,
 1.0
 ]
 }
 ]
 }*/




void main()
{    
    float offset = mod(TIME/4.0, width);
    float x = isf_FragNormCoord.x;
    if(  mod( (x-offset)/width , 1.0 )  <  0.5  )
    {
        gl_FragColor = color1;
    }
    else
    {
        gl_FragColor = color2;
    }
}
