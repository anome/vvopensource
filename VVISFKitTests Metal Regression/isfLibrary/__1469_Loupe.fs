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

	
	float D = distance(p, souris) ;
	float r = calcul_dist(p) ;
	
	vec2 afficher = vec2(0.,0.) ;
	
	if(r <= 0.001){
		color = black ;
	}
	else{
//		afficher = souris + (r/D + max(0., (100./(r + 1.)) - 85.) * random(p) ) * (p-souris) ;
		float bruit = coeff_bruit(r) ;
		afficher = souris + (r/D + bruit * random(p)) * (p-souris) ;
		color = IMG_NORM_PIXEL(inputImage, afficher) ;
	}
	
	
	gl_FragColor = color ;
}