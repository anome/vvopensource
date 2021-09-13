/*{
	"CREDIT": "by You",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "dephasage",
			"TYPE": "float",
			"MIN": -3.14,
			"MAX": 3.14,
			"DEFAULT": 0
		}
	]
}*/

#define pi 3.1415

vec4 noir = vec4(0.,0.,0.,1.);
vec4 bleu = vec4(0.,0.,1.,1.);

float recentre (float alpha){
	if (alpha > pi){
		alpha = alpha - 2. * pi ;
	}
	else{	if(alpha <= -pi){
		alpha = alpha + 2. * pi ;
	}	}
	return alpha ;
}


float angle (float x1, float y1){		// Calcule l'angle entre les points (0.5, 0), (0, 0) et (x1, y1) avec l'origine au centre de l'image
	float alpha = atan(abs(y1/x1)) ;
	if (x1 < 0.){						// Symétrie par rapport à l'axe des ordonnées
		alpha = pi - alpha ;
	}
	if (y1 < 0.){						// Symétrie par rapport à l'axe des abscisses
		alpha = - alpha ;
	}
	alpha = alpha - dephasage ;
	alpha = recentre(alpha) ;
	return alpha ;						// alpha appartient à ]-pi, pi]
}

void main() {
	vec4 color = noir;
	vec2 p =  isf_FragNormCoord.xy;
	
	float x1 = p.x - 0.5 ;
	float y1 = p.y - 0.5 ;

	float alpha = angle(x1,y1);
	alpha = atan((y1/x1)) ;
	
	color = vec4(p.x, /*sin(alpha + dephasage)/3. + 0.7*/ p.y, 0., 1.) ;
	
	gl_FragColor = color;
}