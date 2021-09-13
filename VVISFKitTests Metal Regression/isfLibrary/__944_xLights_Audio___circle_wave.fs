/*
{
  "ISFVSN": "2",
  "CREDIT": "from https://www.shadertoy.com/view/Xsf3WB",
  "DESCRIPTION": "xLights AudioFFT",
  "INPUTS" : [
	{
	    "NAME": "inputImage",
	    "TYPE": "image"
	}
  ]
}
*/

// "audio" and "audioFFT" input types will not work on the website but
// can work within certain apps... replace the input type above with "audioFFT"


const float tau = 6.28318530717958647692;

void main()
{
	vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy*.5)/RENDERSIZE.x;

	uv = vec2(abs(atan(uv.x,uv.y)/(.5*tau)),length(uv));

	// adjust frequency to look pretty	
	uv.x *= 1.0/2.0;
	
	float seperation = 0.06*(1.0-0.5);

	vec3 wave = vec3(0.0);
	const int n = 60;
	for ( int i=0; i < n; i++ )
	{
		float sound = texture2D( inputImage, vec2(uv.x,.75) ).x;
		
		// choose colour from spectrum
		float a = .9*float(i)*tau/float(n)-.6;
		vec3 phase = smoothstep(-1.0,.5,vec3(cos(a),cos(a-tau/3.0),cos(a-tau*2.0/3.0)));
		
		wave += phase*smoothstep(4.0/640.0, 0.0, abs(uv.y - sound*.3));
		uv.x += seperation/float(n);
	}
	wave *= 3.0/float(n);
		
	vec3 col = vec3(0);
	col.z  += texture2D( inputImage, vec2(.000,.25) ).x;
	col.zy += texture2D( inputImage, vec2(.125,.25) ).xx*vec2(1.5,.5);
	col.zy += texture2D( inputImage, vec2(.250,.25) ).xx;
	col.zy += texture2D( inputImage, vec2(.375,.25) ).xx*vec2(.5,1.5);
	col.y  += texture2D( inputImage, vec2(.500,.25) ).x;
	col.yx += texture2D( inputImage, vec2(.625,.25) ).xx*vec2(1.5,.5);
	col.yx += texture2D( inputImage, vec2(.750,.25) ).xx;
	col.yx += texture2D( inputImage, vec2(.875,.25) ).xx*vec2(.5,1.5);
	col.x  += texture2D( inputImage, vec2(1.00,.25) ).x;
	col /= vec3(4.0,7.0,4.0);
	
	// vignetting
	col *= smoothstep( 1.2, 0.0, uv.y );
	
	gl_FragColor = vec4(wave+col,1);
}