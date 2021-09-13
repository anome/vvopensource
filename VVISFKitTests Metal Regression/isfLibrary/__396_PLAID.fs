/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
			"NAME": "PLAID",
			"LABEL": "position",
			"TYPE": "float",
			"DEFAULT": 12.0,
			"MIN": 0.01,
			"MAX": 50.0
		}
	]
}*/

void main( void ) {

	vec2 position = ( gl_FragCoord.xy / (RENDERSIZE.xy +PLAID) * 5.0);

	float color = position.x;
float size = 21.0 + atan(TIME) * cos(PLAID) ;
float size2 = 12.0 + sin(TIME) * PLAID;
	
	if( distance(  mod(gl_FragCoord.xy * position.xy /12.0 * RENDERSIZE,size) - vec2(size/3.0,size2/3.0),vec2(0.0,0.1)) < 5.0)
		gl_FragColor = vec4( vec3( size2 * 0.25, PLAID/48.0, log2( size + TIME / 50.0 ) * size2 ), 0.8 );
	else 
		gl_FragColor = vec4(size/52.0, cos( TIME / size), sin( TIME ),1.0);
		
		//gl_FragColor = vec4(0.5,0.5,0.5,1);
}
