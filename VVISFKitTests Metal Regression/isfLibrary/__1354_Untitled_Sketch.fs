/*{
	"CREDIT": "by elisemoris",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "nbCases",
			"TYPE": "float",
			"DEFAULT": 12,
			"MIN": 0,
			"MAX": 100
		},
		{
			"NAME": "force",
			"TYPE": "float",
			"DEFAULT": 0.01,
			"MIN": 0.0,
			"MAX": 0.1
		},
		{
			"NAME": "presenceBruit",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "souris",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		}
	]
}*/

vec4 black = vec4(0.,0.,0.,1.) ;
float random (vec2 st) {
	return fract(sin(dot(st.xy,
	vec2(12.9898,78.233)))*43758.5453123);
}

float n = floor(nbCases) ;
float pas = 1./n ;

vec2 milieuCase (vec2 p){			// Détermine l'ordonnée de la borne haute de la case contenant le pixel d'entrée
	vec2 milieu = vec2((floor(p.x* n))/n,(floor(p.y* n))/n) + vec2(pas/2., pas/2.) ;
	return milieu;
}

float calcul_dist(vec2 p){
	float D = distance(p, souris) ;
	float delta = pow(D,2.) - 4. * force ;
	float r = (D + sqrt(abs(delta)))/2. ;
	return r ;
}

float coeff_bruit (float r){
	float bruit = 0. ;
	if(presenceBruit){
		bruit = 0.01/(1. + pow(r, 1.) - 0.1) ;
	}
	return bruit ;
}

void main() {
	vec2 p = isf_FragNormCoord.xy ;
	vec4 color = black ;
	
	vec2 milieu = milieuCase(p) ;
	
	float D = distance(milieu, souris) ;
	float r = calcul_dist(milieu) ;
	
	vec2 afficher = vec2(0.,0.) ;
	
	if(r <= 0.001){
		color = black ;
	}
	else{
//		afficher = souris + (r/D + max(0., (100./(r + 1.)) - 85.) * random(p) ) * (p-souris) ;
		float bruit = coeff_bruit(r) ;
		if (abs(p.x-milieu.x) <= 0.5 && abs(p.y-milieu.y) <= 0.5){
			afficher = souris + (r/D + bruit * random(p)) * (milieu-souris) + p - milieu ;
			color = IMG_NORM_PIXEL(inputImage, afficher) ;
		}
	}
	
	
	gl_FragColor = color ;
}