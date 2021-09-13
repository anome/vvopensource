/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "inputImage1",
      "TYPE" : "image"
    }
  ],
  "CREDIT" : ""
}
*/

/*
void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	//	both of these are also the same
	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
	gl_FragColor = inputPixelColor;
}
*/

vec2 iResolution = RENDERSIZE;
float iTime = TIME;
vec2 iMouse = vec2(100.,100.);

void main() {
    vec2 fragCoord = gl_FragCoord.xy;
    vec4 fragColor = vec4();
    //mainImage(fragColor, fragCoord);
//}

//void mainImage( out vec4 fragColor, in vec2 fragCoord )
//{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = fragCoord/iResolution.xy;

    // Normalized mouse coords
    float mx=iMouse.x/iResolution.x;
    float my=iMouse.y/iResolution.y;
    if ((iMouse.x<=0.0)||(iMouse.y<=0.0)){mx=1.0,my=0.2;};
    
    float effectStrength = 32.0 * my;

    // Time varying pixel color
    vec3 col = 0.5 + 0.5*cos(iTime+uv.xyx+vec3(0,2,4));

    vec4 noiseR = IMG_PIXEL(inputImage1, vec2(uv.x/30., uv.y/30. + iTime/100.));
    vec4 noiseG = IMG_PIXEL(inputImage1, vec2(uv.x/20., uv.y/20. + iTime/90.));
    vec4 noiseB = IMG_PIXEL(inputImage1, vec2(uv.x/40., uv.y/40. + iTime/80.));

    vec4 imageR = IMG_PIXEL(inputImage, uv + (noiseR.xz/10.)*effectStrength);
    vec4 imageG = IMG_PIXEL(inputImage, uv + (noiseG.xz/20.)*effectStrength);
    vec4 imageB = IMG_PIXEL(inputImage, uv + (noiseB.xz/15.)*effectStrength);
    
    vec3 color = vec3(imageR.x, imageG.y, imageB.z);// + noise.xyz;
    
    // Output to screen
    fragColor = vec4(color.xyz,1.0);
    gl_FragColor = fragColor;
}