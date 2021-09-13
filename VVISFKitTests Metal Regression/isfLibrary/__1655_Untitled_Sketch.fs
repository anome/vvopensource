/*{
 "TITLE": "MetamorphosisOfGray",
 "CREDIT": "by DANtheMAN",
 "DESCRIPTION": "",
 "CATEGORIES": [
 ],
 "INPUTS": [
 {
 "NAME": "zoom",
 "TYPE": "float",
 "MAX" : 10,
 "MIN" : 0.01,
 "DEFAULT":1.0
 },
  {
 "NAME": "scale",
 "TYPE": "float",
 "MAX" : 10000,
 "MIN" : 1,
 "DEFAULT":1.0
 },
 {
 "NAME": "pos",
 "TYPE": "float",
 "MAX" : 100000,
 "MIN" : 0.0001,
 "DEFAULT":1.0
 },
 {
 "NAME": "center",
 "TYPE": "point2D",
 "MIN": [0.0, 0.0],
 "MAX": [1.0, 1.0],
 "DEFAULT": [0.5, 0.5]
 }
 ]
 }*/

#define MAX_ITER 10
#define PI 3.14159265359

void main( void ) {
    
    vec2 p = gl_FragCoord.xy / RENDERSIZE.x -center;
    
    
    
    
    vec2 i = p*zoom;
    
    float c = 0.0;
    
    float d = length(i);
    float r = atan(i.x, i.y);
    for (int n = 0; n < MAX_ITER; n++) {
        i +=  vec2(
                     (center.x)/float((n+2) * MAX_ITER/(n+2)) * d * sin(TIME*r * i.y * (pow(float(n), 2.0 - (cos(TIME / (scale)))) / float(MAX_ITER)/float(n+2))),
                     (center.y)/float((n+1) * MAX_ITER/(n+1)) * d * sin(TIME*r * i.x * (pow(float(n), 2.0 - (cos(TIME / (scale)))) / float(MAX_ITER)/float(n+1)))
                     );
        
        d = length(i);
        r = (atan(i.x, i.y));
    }
    
    c = (1.0-d);
    c = pow(c, 2.2);
    
    gl_FragColor = vec4(c)+vec4(dot(c, d));
}