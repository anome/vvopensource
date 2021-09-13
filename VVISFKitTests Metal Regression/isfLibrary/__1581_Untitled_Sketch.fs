/*{
 "DESCRIPTION": "RGB GLitchMod",
 "CREDIT": "by dantheman",
 "CATEGORIES": [
 "stylized Effect"
 ],
 "INPUTS": [
 {
 "NAME": "index",
 "TYPE": "float",
 "MIN":1.0,
 "MAX":10.0
 },
 {
 "NAME": "one",
 "TYPE": "image"
 },
 {
 "NAME": "two",
 "TYPE": "image"
 },
 {
 "NAME": "three",
 "TYPE": "image"
 },
 {
 "NAME": "four",
 "TYPE": "image"
 },
 {
 "NAME": "five",
 "TYPE": "image"
 },
 {
 "NAME": "six",
 "TYPE": "image"
 },
 {
 "NAME": "seven",
 "TYPE": "image"
 },
 {
 "NAME": "eight",
 "TYPE": "image"
 },
 {
 "NAME": "nine",
 "TYPE": "image"
 },
 {
 "NAME": "ten",
 "TYPE": "image"
 },
 {
 "NAME": "eleven",
 "TYPE": "image"
 },
 {
 "NAME": "twelve",
 "TYPE": "image"
 },
 {
 "NAME": "thirteen",
 "TYPE": "image"
 },
 {
 "NAME": "fourteen",
 "TYPE": "image"
 },
 {
 "NAME": "fifteen",
 "TYPE": "image"
 }
 ]
 
 }*/



void main()
{
    vec2 uv = vv_FragNormCoord.xy;
    //	if this is the first pass, i'm going to read the position from the "lastPosition" image, and write a new position based on this and the hold variables
    
    vec4	srcPixel1 = IMG_NORM_PIXEL(one,uv);
    vec4	srcPixel2 = IMG_NORM_PIXEL(two,uv);
    vec4	srcPixel3 = IMG_NORM_PIXEL(three,uv);
    vec4	srcPixel4 = IMG_NORM_PIXEL(four,uv);
    vec4	srcPixel5 = IMG_NORM_PIXEL(five,uv);
    vec4	srcPixel6 = IMG_NORM_PIXEL(six,uv);
    vec4	srcPixel7 = IMG_NORM_PIXEL(seven,uv);
    vec4	srcPixel8 = IMG_NORM_PIXEL(eight,uv);
    vec4	srcPixel9 = IMG_NORM_PIXEL(nine,uv);
    vec4	srcPixel0 = IMG_NORM_PIXEL(ten,uv);
    vec4	srcPixel11 = IMG_NORM_PIXEL(eleven,uv);
    vec4	srcPixel12 = IMG_NORM_PIXEL(twelve,uv);
    vec4	srcPixel13 = IMG_NORM_PIXEL(thirteen,uv);
    vec4	srcPixel14 = IMG_NORM_PIXEL(fourteen,uv);
    vec4	srcPixel15 = IMG_NORM_PIXEL(fifteen,uv);
    
    //vec4   modPix = mod(srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5+srcPixel6+srcPixel7+srcPixel8+srcPixel8+srcPixel9+srcPixel0, vec4(1.0));
    
    vec4   avgPix = (srcPixel1);
        if(index< = 1.0){
            avgPix = (srcPixel1);
        }else if(index <= 2.0){
            avgPix = (srcPixel1+srcPixel2)/index;
        }else if(index <= 3.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3)/index;
        }else if(index = 4.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4)/index;
        }else if(index == 5.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5)/index;
        }else if(index == 6.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5+srcPixel6)/index;
        }else if(index == 7.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5+srcPixel6+srcPixel7)/index;
        }else if(index == 8.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5+srcPixel6+srcPixel7+srcPixel8)/index;
        }else if(index == 9.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5+srcPixel6+srcPixel7+srcPixel8+srcPixel9)/index;
        }else if(index == 10.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5+srcPixel6+srcPixel7+srcPixel8+srcPixel8+srcPixel9+srcPixel0)/index;
        }else if(index == 11.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5+srcPixel6+srcPixel7+srcPixel8+srcPixel8+srcPixel9+srcPixel0+srcPixel11)/index;
        }else if(index == 12.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5+srcPixel6+srcPixel7+srcPixel8+srcPixel8+srcPixel9+srcPixel0+srcPixel11+srcPixel12)/index;
        }else if(index == 13.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5+srcPixel6+srcPixel7+srcPixel8+srcPixel8+srcPixel9+srcPixel0+srcPixel11+srcPixel12+srcPixel13)/index;
        }else if(index == 14.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5+srcPixel6+srcPixel7+srcPixel8+srcPixel8+srcPixel9+srcPixel0+srcPixel11+srcPixel12+srcPixel13+srcPixel14)/index;
        }else if(index == 15.0){
            avgPix = (srcPixel1+srcPixel2+srcPixel3+srcPixel4+srcPixel5+srcPixel6+srcPixel7+srcPixel8+srcPixel8+srcPixel9+srcPixel0+srcPixel11+srcPixel12+srcPixel13+srcPixel14+srcPixel15)/index;
        }
    gl_FragColor = vec4(vec3(avgPix), 1.0);
}
