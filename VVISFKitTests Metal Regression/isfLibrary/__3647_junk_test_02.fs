/*{
	"CREDIT": "by microcyb",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		}
	]
}*/
float decay=0.97815;
float exposure=0.92;
float density=0.966;
float weight=0.04767;
vec2 lightPositionOnScreen = vec2(0.5,0.45);
varying vec4 gl_TexCoord[];
const int NUM_SAMPLES = 100 ;

void main(void)
{	
	vec2 deltaTextCoord = vec2( gl_TexCoord[0].st - lightPositionOnScreen.xy );
	vec2 textCoo = gl_TexCoord[0].st;
	deltaTextCoord *= 1.0 /  float(NUM_SAMPLES) * density;
	float illuminationDecay = 1.0;

    gl_FragColor = vec4(0.0);
	for(int i=0; i < NUM_SAMPLES ; i++)
    {
             textCoo -= deltaTextCoord;
             vec4 sample = IMG_PIXEL(inputImage, textCoo );
		
             sample *= illuminationDecay * (weight + cos(u_Elapsed*100000.0)/500.0);
         //    sample *= illuminationDecay * weight;// + cos(u_Elapsed*1000.0)/500.0);


             gl_FragColor += sample;

             illuminationDecay *= decay;
     }
     gl_FragColor *= exposure;
    //gl_FragColor = vec4(0.0);
}
