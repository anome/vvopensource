/*{
	"CREDIT": "by You",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "temps",
			"TYPE": "float",
			"MIN" : 0,
			"MAX" : 1,
			"DEFAULT" : 0
	    },
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "duree",
			"TYPE": "float",
			"MIN": 0,
			"MAX": 0.2,
			"DEFAULT": 0.13
		},
		{
			"NAME": "vitesse_relative",
			"TYPE": "float",
			"MIN": 0,
			"MAX": 10,
			"DEFAULT": 2.6
		},
		{
			"NAME": "dephasage",
			"TYPE": "float",
			"MIN": -3.14,
			"MAX": 3.14,
			"DEFAULT": 0
		},
		{
			"NAME": "borderRadius",
			"TYPE": "float",
			"MIN": 0,
			"MAX": 1,
			"DEFAULT": 0.5
		},
		{
			"VALUES" : [
				0,
				1
			],
			"NAME" : "bords",
			"TYPE" : "long",
			"DEFAULT" : 0,
			"LABELS" : [
				"Bords droits",
				"Bords ronds"
			]
		}
	]
}*/

#define pi 3.1415

float t = temps * duree ;
vec2 centre = vec2(0.,0.) ;
vec4 noir = vec4(0., 0., 0., 1.);
float border_radius = temps * borderRadius ;

float alpha (float temps){
	float a = 1.;
	if (temps > 0.5){
		a = 1. - 2. * (temps - 0.5);
	}
	return a ;
}

float random (vec2 st) {
	return fract(sin(dot(st.xy,
	vec2(12.9898,78.233)))*43758.5453123);
}

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
	return alpha ;						// alpha appartient à ]-pi, pi]
}

float distb (float alpha){				// Calcule la distance séparant le centre de l'imge au bord pour l'angle alpha en entrée
	float distance_au_bord = 0. ;
	alpha = abs(alpha) ;
	if (alpha > 3. * pi/4.){
		alpha = alpha - pi ;
	}
	else{	if (alpha > pi /4.){
		alpha = alpha - pi/2. ;
	}}
	distance_au_bord = 0.5 / cos(alpha) ;
	return distance_au_bord ;
}

float vitesse (float alpha){
	float distance_au_bord = distb(alpha) ;
	alpha = alpha + dephasage ;
	float vitesse_pixel = (4. * vitesse_relative * ( 1. + 0.05*sin(4.*alpha) - 0.05 * cos(3. * alpha)));
	if (bords == 0){
		vitesse_pixel = vitesse_pixel * distance_au_bord ;
	}
	else{
		vitesse_pixel = vitesse_pixel * 0.5 ;
	}
	return vitesse_pixel ;
}

vec2 translation (float x1, float y1){	// Calcule la translation entre le pixel d'entrée
	float alpha = angle (x1,y1) ;
	vec2 translate = vec2(cos(alpha),sin(abs(alpha))) ;
	
	if (x1 < 0.){
		translate.x = - cos(pi-alpha) ;
	}
	if (y1 < 0.){
		translate.y = - translate.y ;
	}
	
	// Mise à l'échelle pour intégrer le déplacement
	translate.x = translate.x * t * vitesse(alpha) ;
	translate.y = translate.y * t * vitesse(alpha) ;
	
	return(translate) ;
}

void main() {
	vec2 p =  isf_FragNormCoord.xy;
	vec4 color = noir;
	
	// Changement de repère, l'origine est au centre de l'image
	float x1 = p.x - 0.5 ;
	float y1 = p.y - 0.5 ;
	
	// Coordonnée du pixel à afficher
	vec2 new = p + translation(x1,y1) ;
	
	if (new.x > 0. && new.x < 1. && new.y > 0. && new.y < 1.){		// Si le pixel est dans l'image
		color = vec4(vec3(IMG_NORM_PIXEL(inputImage, new)), alpha(temps)) ;
	}
	
	// Ajouts bords ronds
	if (bords == 1){
		if (new.x < border_radius){
			if (new.y < border_radius && distance(new, vec2(border_radius, border_radius)) > border_radius){
				color = noir ;
			}
			else{ if (new.y > (1. - border_radius) && distance(new, vec2(border_radius, 1. - border_radius)) > border_radius){
				color = noir ;
			}}
		}
		if (new.x > 1. - border_radius){
			if (new.y < border_radius && distance(new, vec2(1. - border_radius, border_radius)) > border_radius){
				color = noir ;
			}
			else{ if (new.y > (1. - border_radius) && distance(new, vec2(1. - border_radius, 1. - border_radius)) > border_radius){
				color = noir ;
			}}
		}
	}
	
	gl_FragColor = color ;
}