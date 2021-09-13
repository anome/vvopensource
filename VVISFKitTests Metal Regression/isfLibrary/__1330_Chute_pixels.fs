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
			"NAME": "vitesse_chute",
			"TYPE": "float",
			"MIN": 0,
			"MAX": 20,
			"DEFAULT": 17
		},
		{
			"NAME": "decoupage",
			"TYPE": "float",
			"MIN": 2,
			"MAX": 12,
			"DEFAULT": 12
		},
		{
			"VALUES" : [
				0,
				1,
				2,
				3
			],
			"NAME" : "Depart",
			"TYPE" : "long",
			"DEFAULT" : 0,
			"LABELS" : [
				"Aleatoire",
				"Partant du bas",
				"Vague",
				"Sinusoidal"
			]
		}
	]
}*/

// vitesse_chute est l'accélération des cases. Celles-ci chutent à accélération constante
// decoupage donne le nombre de cases horizontalement et verticalement
// Depart propose différents départs possibles pour les cases : testez-les !

// Une case est un ensemble de pixels ayant le même comportement : départ au même moment et même vitesse.
// Il y a decoupage*decoupage cases.
// On repère une case par un pixel quelconque qu'elle contient.
// Il suffit ensuite d'utiliser la fonction bornesup pour déterminer son ordonnée la plus haute et donc connaître sa position exacte.

#define pi 3.1415
vec4 noir = vec4(0., 0., 0., 1.);

float n = floor(decoupage) ;
float pas = 1./n ;
float t = temps ;

float random (vec2 st) {
	return fract(sin(dot(st.xy,
	vec2(12.9898,78.233)))*43758.5453123);
}

float bornesup (vec2 p){			// Détermine l'ordonnée de la borne haute de la case contenant le pixel d'entrée
	return ((floor(p.y* n + 1.))/n);
}

float starttime (vec2 p){			// Détermine le temps de départ de la case contenant le pixel d'entrée
	float xinf = floor(p.x * n)/n ;
	float yinf = floor(p.y * n)/n ;
	float t0 = random(vec2(xinf,yinf)) / 2. ;
	if (Depart == 1){
		t0 = (3. * yinf + random(vec2(xinf,yinf))) / 5. ;
	}
	else{
		if (Depart == 2){ t0 = (yinf + random(vec2(xinf,yinf)) + 3. * sin(xinf)) / 6. ;}
		else {
		if (Depart == 3){ t0 = yinf * ( 0.3 * random(vec2(xinf,yinf)) + 0.3*sin(4.*xinf*pi) - 0.3 * cos(3. * xinf*pi)) ; }
		}
	}
	if (yinf >= 1.){ t0 = 20000000.;}
	return t0 * vitesse_chute / 17. ;
}

vec2 positions_max (vec2 case){		// Détermine les ordonnées inférieure et supérieure de la case d'entrée en fonction du temps
	float t0 = starttime(case) ;
	float sup = bornesup(case) ;
	float inf = sup - pas ;
	float pos_sup = sup ;					// Avant le début de la chute
	float pos_inf  = inf ;
	if (t > t0){							// Après le début de la chute
		float translation =  (t - t0) * (t - t0) * 0.5 * vitesse_chute ;
		pos_sup = sup - translation ;
		pos_inf = inf - translation ;
	}
	return vec2(pos_inf, pos_sup)  ;
}

vec2 courant (vec2 p){				// Détermine la case devant être affichée sur le pixel en entrée
	vec2 increment = vec2(0.,0.) ;
	vec2 case = p + increment ;				// Increment nul donc il s'agit de la case contenant le pixel d'entrée
	vec2 position = positions_max(case) ;	
	
	if (p.y < position.x || p.y > position.y){		// Si la case ne recouvre pas le pixel d'entrée, on considère la case suivante (celle du dessus)
		increment.y = increment.y + pas ;
		case = p + increment ;
		position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
			position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
			position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
			position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
			position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
			position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
			position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
			position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
			position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
			position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
			position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
			position = positions_max(case) ;
		if (p.y < position.x || p.y > position.y){
			increment.y = increment.y + pas ;
			case = p + increment ;
		}
		}
		}
		}
		}
		}
		}
		}
		}
		}
		}
		}
	}
	
	return case ;							// La fonction retourne la case recouvrant le pixel d'entrée
}

void main() {
	vec2 p = vec2(isf_FragNormCoord.xy) ;	// Coordonnées normalisées du pixel traité
	vec4 color = noir ;						// Initialisation de la couleur de ce pixel à afficher

	float debut = starttime(p);				// Début de la chute de la case
	vec2 c = courant(p) ;					// Case courante recouvrant le pixel
	
	if (t < debut){							// Situation initiale
		color = IMG_NORM_PIXEL(inputImage, p) ;		// On affiche l'image originale
	}
	else{
		float t0 = starttime(c) ;			// Temps de départ de la case recouvrant le pixel
		// Coordonnée du pixel de l'image à afficher : coordonnées initiales décalées de la distance parcourue
		vec2 translate = p + vec2(0., (t - t0) * (t - t0) * 0.5 * vitesse_chute) ;
		if (translate.y <= 1.){				// Si le pixel est dans l'image
			color = IMG_NORM_PIXEL(inputImage, translate) ;			// On affiche le pixel de l'image que l'on vient de déterminer
		}
	}
	

	gl_FragColor = color ;
}
