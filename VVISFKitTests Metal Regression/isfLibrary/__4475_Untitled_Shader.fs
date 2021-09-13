
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
			"NAME": "tint",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.67,
			"MAX": 1.67
		},
		{
			"NAME": "temperature",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.67,
			"MAX": 1.67
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

void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
// 	 // Range ~[-1.67;1.67] works best
    float t1 = temperature;
    float t2 = tint;

    // Get the CIE xy chromaticity of the reference white point.
    // Note: 0.31271 = x value on the D65 white point
    float x = 0.31271 - t1 * (t1 < 0.0 ? 0.1 : 0.05);
    float standardIlluminantY = 2.87 * x - 3.0 * x * x - 0.27509507;
    float y = standardIlluminantY + t2 * 0.05;

    // Calculate the coefficients in the LMS space.
    vec3 w1 = vec3(0.949237, 1.03542, 1.08728); // D65 white point

    // CIExyToLMS
    float Y = 1.0;
    float X = Y * x / y;
    float Z = Y * (1.0 - x - y) / y;
    float L = 0.7328 * X + 0.4296 * Y - 0.1624 * Z;
    float M = -0.7036 * X + 1.6975 * Y + 0.0061 * Z;
    float S = 0.0030 * X + 0.0136 * Y + 0.9834 * Z;
    vec3 w2 = vec3(L, M, S);

    vec3 balance = vec3(w1.x / w2.x, w1.y / w2.y, w1.z / w2.z);

    mat3 LIN_2_LMS_MAT = mat3(
        3.90405e-1, 5.49941e-1, 8.92632e-3,
        7.08416e-2, 9.63172e-1, 1.35775e-3,
        2.31082e-2, 1.28021e-1, 9.36245e-1
    );

    mat3 LMS_2_LIN_MAT = mat3(
        2.85847e+0, -1.62879e+0, -2.48910e-2,
        -2.10182e-1,  1.15820e+0,  3.24281e-4,
        -4.18120e-2, -1.18169e-1,  1.06867e+0
    );

    vec3 lms = LIN_2_LMS_MAT * inputPixelColor.rgb;
    lms *= balance;
    vec3 rgb = LMS_2_LIN_MAT * lms;
    gl_FragColor = vec4(rgb.rgb, inputPixelColor.a);
}


