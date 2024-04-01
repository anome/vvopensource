/*{
	"CREDIT": "Anomes",
	"DESCRIPTION": "create a border around alpha edges",
	"CATEGORIES": [
		"Stylize"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "color",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				0.0,
				1.0
			]
		},
		{
			"NAME": "blurSize",
			"TYPE": "float",
			"DEFAULT": 3.0,
			"MIN": 0.01,
			"MAX": 20.0
		}
	],
	"PASSES": [
		{
			"TARGET": "horizontalBlur"
		},
		{
			"TARGET": "verticalBlur"
		},
		{
			"TARGET": "border"
		}
	]
	
}*/






vec4 blurGaussianPass(bool horizontal)
{
    float r1 = 2.0*blurSize*blurSize;
    float r2 = 3.1415926*r1;
    float rs = ceil(blurSize * 2.57);
    vec2 position = vv_FragNormCoord*RENDERSIZE;
    vec4 sum = vec4(0.);
    float count = 0.;
    for(float i=-floor(rs); i<=rs; i++)
    {
        float weight = exp(-i*i/r1) / r2;
        vec2 offsetPosition = position;
        if( horizontal )
        {
            offsetPosition.x += i;
        }
        else
        {
            offsetPosition.y += i;
        }
        if( 0. <= offsetPosition.x && offsetPosition.x < RENDERSIZE.x && 0. <= offsetPosition.y && offsetPosition.y < RENDERSIZE.y )
        {
            if( horizontal )
            {
                sum += IMG_NORM_PIXEL(inputImage, offsetPosition/RENDERSIZE)*weight;
            }
            else
            {
                sum += IMG_NORM_PIXEL(horizontalBlur, offsetPosition/RENDERSIZE)*weight;
            }
            count += weight;
        }
    }
    return sum /= count;
}


#define SHADOW 0.0
#define HIGHLIGHT 0.04
#define GAMMA 0.3

void main()
{
    if( PASSINDEX == 0 )
    {
        gl_FragColor = blurGaussianPass(true);
    }
    else if( PASSINDEX == 1 )
    {
        gl_FragColor = blurGaussianPass(false);
    }
    else if( PASSINDEX == 2 )
    {
        vec4 blurColor = IMG_NORM_PIXEL(verticalBlur, vv_FragNormCoord);
        vec4 originalColor = IMG_NORM_PIXEL(inputImage, vv_FragNormCoord);
        blurColor.rgb = color.rgb;
        float shadow = 0.0;
        float highlight = 0.04;
        float gamma = 0.3;
        blurColor.a = pow(    min( max(blurColor.a-SHADOW, 0.) / (HIGHLIGHT-SHADOW) , 1. )    ,    1./GAMMA    )    *    color.a;
        gl_FragColor = mix(blurColor, originalColor, originalColor.a);
    }

}
