/*{
	"CREDIT": "by joris_dejong",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		
		{
			"NAME": "offset",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{ 	
			"NAME": "width",
			"TYPE": "float",
			"DEFAULT": 1.0
		},
		{
			"NAME": "angle",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

#define PI 3.141592654

mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}

void main() {
	vec2 st = isf_FragNormCoord.xy;
	
	st -= vec2(0.5);
    // rotate the space
    st = rotate2d( angle * 2.0 * PI  ) * st;
    // move it back to the original place
    st += vec2(0.5);
    
    //calculate angle adjustment
	float adjust = -(pow(( fract( angle * 4.0 ) - 0.5 ) * 2.0, 2.0)) + 1.0;
	adjust = 1.0 + adjust * .41;

	float mWidth = width * 2.0;
	float mOffset = (offset - 0.5) * -(1.0 + mWidth); 

	float trail = smoothstep( 0.5 - mWidth * 0.5 , 0.5 + mWidth * 0.5, st.x + mOffset * adjust) ;
	trail *= step ( st.x + mOffset * adjust, 0.5 + mWidth * 0.5);

	vec4 color = vec4( vec2(trail), trail, 1.0 );
	gl_FragColor = color;
}