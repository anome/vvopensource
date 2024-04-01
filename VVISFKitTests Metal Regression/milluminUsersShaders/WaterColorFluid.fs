/*{
    "CATEGORIES": [
        "Shadertoy"
    ],
    "CREDIT": "",
    "DESCRIPTION": "Water Color Fluid",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "DEFAULT": 1,
            "LABEL": "wetdry",
            "MAX": 1,
            "MIN": 0,
            "NAME": "wetdry",
            "TYPE": "float"
        },
        {
            "LABEL": "Flow",
            "MAX": 1,
            "MIN": 0.25,
            "NAME": "Flow",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "BufferA"
        }
    ]
}
*/


// This bug has been fixed on 22ace35 (29/07/2023)

// Uncomment this line to switch between Metal (default) and GL (fallback)
//int main = 0;

// Uncomment this line to switch between before and after workaround
//int BYPASS_WORKAROUND=0;

void main() {
		
	if (PASSINDEX == 0){

		vec2 blend_uv = gl_FragCoord.xy / RENDERSIZE.xy;
		vec2 uv = vec2(1.0 - blend_uv.x, blend_uv.y);
		vec3 intensity = 1.0 - IMG_NORM_PIXEL(inputImage, uv).rgb;


		float vidSample = dot(vec3(1.0),IMG_NORM_PIXEL(inputImage, uv).rgb);
		float delta = 0.01 * Flow;
		float vidSampleDx = dot(vec3(1.0),IMG_NORM_PIXEL(inputImage, uv + vec2(delta, 0.0)).rgb);
		float vidSampleDy = dot(vec3(1.0),IMG_NORM_PIXEL(inputImage, uv + vec2(0.0, delta)).rgb);
		vec2 flow = delta * vec2 (vidSampleDy - vidSample, vidSample - vidSampleDx);
		intensity = 0.005 * intensity + 0.995 * (1.0 - IMG_NORM_PIXEL(BufferA, blend_uv + vec2(-1.0, 1.0) * flow).rgb);
		gl_FragColor = mix(IMG_NORM_PIXEL(inputImage,uv),(vec4(1.0 - intensity,1.0)),wetdry);
	}

	else if (PASSINDEX == 1)	{
		vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
		gl_FragColor = IMG_NORM_PIXEL(BufferA,mod(uv,1.0));
	}
}
