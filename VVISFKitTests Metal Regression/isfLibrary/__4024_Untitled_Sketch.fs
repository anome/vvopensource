/*
{
  "CATEGORIES" : [
    "Generator"
  ],
  "DESCRIPTION" : "Visualizes an audio waveform image",
  "INPUTS" : [
    {
      "NAME" : "waveImage",
      "TYPE" : "audio"
    },
    {
      "NAME" : "waveSize",
      "TYPE" : "float",
      "MAX" : 0.5,
      "DEFAULT" : 0.05,
      "MIN" : 0
    }
  ],
  "CREDIT" : "by VIDVOX"
}
*/

void main() {
	//	just grab the first audio channel here
	float		channel = 0.0;
	
	//	get the location of this pixel
	vec2		loc = isf_FragNormCoord;
	
	//	though not needed here, note the IMG_SIZE function can be used to get the dimensions of the audio image
	//vec2		audioImgSize = IMG_SIZE(waveImage);
	
	vec2		waveLoc = vec2(loc.x,channel);
	vec4		wave = IMG_NORM_PIXEL(waveImage, waveLoc);
	vec4		waveAdd = (1.0 - smoothstep(0.0, waveSize, abs(wave - loc.y)));
	gl_FragColor = waveAdd;
}