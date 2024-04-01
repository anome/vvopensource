/*{
	"CREDIT": "by Anomes",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	]
}*/


void main()
{
	vec4 color = IMG_THIS_PIXEL(inputImage);
	float gray = dot(color.rgb, vec3(0.299, 0.587, 0.114));
	gl_FragColor = vec4(gray, gray, gray, color.a);
}
