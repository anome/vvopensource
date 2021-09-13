/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "Automatically Converted"
  ],
  "INPUTS" : [

  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#15770.5"
}
*/


#ifdef GL_ES
precision mediump float;
#endif


#define pi    3.1415926535897932384626433832795 //pi 

void main()
{
	vec2 p   = vv_FragNormCoord * 6.0;
	vec3 col = vec3( 0.0, 0.0, 0.0 );
	float ca = 0.0;
	for( int j = 1; j < 9; j++ )
	{
		p *= 1.2;
		float jj = float( j );
		
		for( int i = 1; i < 9; i++ )
		{
			vec2 newp = p*0.96;
			float ii = float( i );
			newp.x += 0.7 / ( ii + jj ) * sin( ii * p.y + TIME + ( jj * ii ) ) + 1.0;
			newp.y += 0.7 / ( ii + jj ) * cos( ii * p.x + TIME + ( jj * ii ) ) - 1.0;
			p=newp;
			
		
		}
		p   *= 0.98;
		col += vec3( 0.5 * sin( pi * p.x ) + 0.5, 0.5 * sin( pi * p.y ) + 0.5, 0.5 * sin( pi * p.x ) * cos( pi * p.y ) + 0.5 );
		ca  += 0.7;
	}
	col /= ca;
	col = mix(col,1.0-col,pow( 0.5 * sin(length(col)*pi*pi) + 0.5, col.x+col.y+col.z ));
	gl_FragColor = vec4( col * col * col, 1.0 );
}