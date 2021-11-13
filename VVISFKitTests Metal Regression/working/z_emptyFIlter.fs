/*{
 "CREDIT": "by zoidberg WOOP WOOP WOOP WOOP WOOP",
 "CATEGORIES": [
 "Blur"
 ],
 "INPUTS": [
 {
 "NAME": "inputImage",
 "TYPE": "image"
 }
 ],
 
 }*/


void main() {
    gl_FragColor = IMG_THIS_PIXEL(inputImage);
}
