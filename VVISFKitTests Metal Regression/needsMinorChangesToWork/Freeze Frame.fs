/*{
	"CREDIT": "by VIDVOX",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "freeze",
			"TYPE": "bool",
			"DEFAULT": 0.0
		},
		{
			"NAME": "mix",
			"TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
			"DEFAULT": 0.0
		}
	],
	"PERSISTENT_BUFFERS": [
		"freezePass"
	],
	"PASSES": [
		{
			"TARGET":"freezePass"
		},
		{
			"TARGET":"finalPass"
		}
	]
}*/


void main()
{
    if( PASSINDEX == 0 )
    {
        if(freeze)
        {
            gl_FragColor = IMG_THIS_PIXEL(freezePass);
        }
        else
        {
            gl_FragColor = IMG_THIS_PIXEL(inputImage);
        }
    }
    else if( PASSINDEX == 1 )
    {
        if(freeze)
        {
            gl_FragColor = mix(IMG_THIS_PIXEL(freezePass), IMG_THIS_PIXEL(inputImage), mix);
        }
        else
        {
            gl_FragColor = IMG_THIS_PIXEL(inputImage);
        }
    }
}
