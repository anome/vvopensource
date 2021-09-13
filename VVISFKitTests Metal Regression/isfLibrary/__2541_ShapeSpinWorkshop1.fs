/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Shapes"
  ],
  "DESCRIPTION": "based on glslsandbox.com/e#27866.0",
  "INPUTS": []
}*/


// ShapeSpinWorkshop1 by mojovideotech
// http://glslsandbox.com/e#27866.0
// forked from :
// Supershapes! by xpansive

////////////////////////////////////////////////////////////
//
// ShapeSpinWorkshop1  by mojovideotech
//
// based on : glslsandbox.com/\e#27866.0
//
// fork of : Supershapes! by xpansive
//
////////////////////////////////////////////////////////////



float supershape(vec2 p, float m, float n1, float n2, float n3, float a, float b, float s, float r) {
	float ang = atan(p.y * RENDERSIZE.y, p.x * RENDERSIZE.x) + r;
	float v = pow(pow(abs(cos(m * ang / 4.0) / a), n2) + pow(abs(sin(m * ang / 4.0) / b), n3), -1.0 / n1);
	return 1. - step(v * s * RENDERSIZE.y, length(p * RENDERSIZE)); 
}

void main( void ) {
	vec2 p = (gl_FragCoord.xy / RENDERSIZE) * 2.0 - 1.0;
 	p *= 2.75;
	
	float color ;
	float color1 ;
	float color2 ;
		
	color += supershape(p - vec2(-2, 1), 6.0, 1.0, 7.0, 8.0, 1.0, 1.0, 0.2, (TIME*0.5));
	color1 += supershape(p - vec2(-2, -1), 3.0, 4.5, 10.0, 10.0, 1.0, 1.0, 0.95, (-TIME*0.5));
	color2 += supershape(p - vec2(-1, 1), 7.0, 10.0, 6.0, 6.0, 1.0, 1.0, 0.95, (-TIME*0.5));
	color += supershape(p - vec2(-1, -1), 16.0, 0.5, 0.15, 16.0, 1.1, 1.0, 1.025, (TIME*0.5));
	color1 += supershape(p - vec2(0, 1), 4.0, 12.0, 15.0, 15.0, 1.0, 1.0, 0.85, (TIME*0.5));
	color2 += supershape(p - vec2(0, -1), 19.0, 9.0, 14.0, 11.0, 1.0, 1.0, 0.9, (-TIME*0.5));
	color += supershape(p - vec2(1, 1), 6.0, 60.0, 55.0, 1000.0, 1.0, 1.0, 0.67, (-TIME*0.5));
	color1 += supershape(p - vec2(1, -1), 6.0, 0.53, 1.69, 0.45, 1.0, 1.0, 1.15, (TIME*0.5));
	color1 += supershape(p - vec2(2, 1), 8.0, 0.5, 0.5, 0.3, 1.0, 1.0, 1.5, (TIME*0.5));
	color2 += supershape(p - vec2(2, -1), 6.0, -0.62, 30.0, 0.6, 1.0, 1.0, 1.25, (-TIME*0.5));
	
	gl_FragColor = vec4(color2+0.3,color-0.25,0.5+color1,1.0);
}