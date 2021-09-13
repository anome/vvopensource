
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "vertical",
			"TYPE": "bool",
			"DEFAULT": 1.0
		}
	],
	"PASSES": [
	]
	
}*/

void main()	{
        vec4 lines;
        
        vec2 vUv = isf_FragNormCoord.xy;
        
        if (vertical) {
            lines = texture2D(inputImage, vUv.xx);
        } else {
            lines = texture2D(inputImage, vUv.yy);
        }

        gl_FragColor = vec4(lines);
}
