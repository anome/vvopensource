/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    
  ]
}
*/
// PulsarWave1 by mojovideotech

#ifdef GL_ES
precision mediump float;
#endif

varying vec3 v;

void main( void ) {
	vec2 position = vec2((gl_FragCoord.x/RENDERSIZE.x)-0.5,(gl_FragCoord.y/RENDERSIZE.y-0.5)*0.33) ;
	float y = 0.125 * position.x * sin(333.0 * position.x - 66.0 * TIME *0.05);
	y = 3. / (666. * abs(position.y - y));
	
	y += 1.125/length(666.*length(position / vec2(0.01, position.x)));
	y += 1./length(99.*length(position - vec2(mod(dot(tan(-TIME*0.75), position.y),(position.y, cos(TIME*0.333))))));
	
	float saule = 1.0/length(66.*length(position - vec2(0, 0)));
	
	vec4 vsaule = vec4(saule, saule, saule*2.5, 0.67);
	vec4 vstari = vec4(position.y*0.67 - y, y-0.67, y*0.5, 1.0);
		 vstari += vec4( y*1.75, inversesqrt(position.y*TIME), mod(y,TIME), 1.0);

	gl_FragColor = mix(vsaule, vstari, abs(sin(TIME*0.67)));
	
}