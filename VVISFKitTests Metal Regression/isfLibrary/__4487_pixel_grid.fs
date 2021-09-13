
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "gridLineWidth",
			"TYPE": "float",
			"DEFAULT": 0.01,
			"MIN": 0.0,
			"MAX": 1.0
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

const float GRID_SIZE = 10.;
const float PI = 3.1415;

void main()	{
	vec4 inputPixelColor = IMG_THIS_PIXEL(inputImage);
	
	gl_FragColor = inputPixelColor;
	
	vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    vec3 color = inputPixelColor.xyz;
    
    float squareWave = 1.0 - sign(sin(st.x*10.*PI*2.));
    float vertSquareWave = 1.0 - sign(sin(st.y*10.*PI*2.));
    color -= vec3(squareWave + vertSquareWave);
    
    // for(int i = 0; i < 10; i++) {
    //     // Each result will return 1.0 (white) or 0.0 (black).
    //     // float left = 1. - step(0.1,st.x);   // Similar to ( X greater than 0.1 )
    //     // float bottom = 1. - step(0.1,st.y); // Similar to ( Y greater than 0.1 )
    
    //     // The multiplication of left*bottom will be similar to the logical AND.
    //     // Addition here acts like logical OR.
    //     // color -= vec3( left + bottom );
    //     float percent = 1.0/GRID_SIZE * float(i);
    //     float lineEnd = 1.0 - step(gridLineWidth + percent, st.x);
    //     float lineStart = 1.0 - step(percent, st.x);
        
    //     color -= vec3( lineEnd - lineStart );
    // }

    gl_FragColor = vec4(color,1.0);
	
}
