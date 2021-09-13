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
			"NAME": "typeGrille",
			"TYPE": "long",
			"VALUES": [
				0,
				1
			],
			"LABELS": [
				"sinusoide",
				"image"
			],
			"DEFAULT": 0
		},
		{
			"NAME": "souris",
			"TYPE": "float",
			"MIN" : 0,
			"MAX" : 1,
			"DEFAULT" : 0.5
		},
		{
			"NAME": "centre",
			"TYPE": "point2D",
			"DEFAULT": [0.5,0.5]
		},
		{
			"NAME": "bloquer",
			"TYPE": "bool",
			"DEFAULT": false
		},
		{
			"NAME": "amplitude",
			"TYPE": "float",
			"DEFAULT": 0.01,
			"MIN": 0.0,
			"MAX": 0.2
		},
		{
			"NAME": "periode",
			"TYPE": "float",
			"DEFAULT": 0.28,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "pas",
			"TYPE": "float",
			"DEFAULT": 0.01,
			"MIN": 0.0,
			"MAX": 0.4
		},
		{
			"NAME": "imageGrille",
			"TYPE": "image"
		}
	]
}*/

#define pi 3.14159
vec4 none = vec4(0.);

vec2 versBaseOrigine(vec2 p, float phi){
	vec2 origine = vec2(p.x * cos(phi) + p.y * sin(phi),
						p.y * cos(phi) - p.x * sin(phi)) ;
	return origine ;
}

float largeur(){
	float larg = 0.2 ;
	if (abs(0.5 - souris) < 0.05){
		larg = 0.2 + 0.36/0.05 * (0.05 - abs(0.5 - souris)) ;
	}
	return larg ;
}

bool sinus(vec2 p){
	bool transparent = false ;
	float figure = p.x + amplitude*sin(p.y*2.*pi/periode) ;
	float n = floor((figure / pas) + 0.5) ;
	if((figure >= n*pas - largeur()*pas) && (figure <= n*pas + largeur()*pas)){
		transparent = true ;
	}
	return transparent ;
}


void main() {
	vec2 p = isf_FragNormCoord.xy ;
	float angle = 0.5 - souris ;
	if (bloquer){
		angle = abs(angle) ;
	}
	vec4 color = none ;
	
	if((typeGrille == 0 && sinus(p)) || (typeGrille == 1 && IMG_NORM_PIXEL(imageGrille, p).a >= 0.1)){
		vec2 decalage1 = versBaseOrigine(p - centre, angle * 2.) + centre ;
		vec2 decalage2 = versBaseOrigine(p - centre, angle) + centre ;
		vec2 decalage4 = versBaseOrigine(p - centre, - angle) + centre ;
		vec2 decalage5 = versBaseOrigine(p - centre, - angle * 2.) + centre ;
		
		vec4 image1 = IMG_NORM_PIXEL(inputImage, decalage1) ;
		vec4 image2 = IMG_NORM_PIXEL(inputImage, decalage2) ;
		vec4 image3 = IMG_NORM_PIXEL(inputImage, p) ;
		vec4 image4 = IMG_NORM_PIXEL(inputImage, decalage4) ;
		vec4 image5 = IMG_NORM_PIXEL(inputImage, decalage5) ;
		
		
		color = image5 ;
		if (image4.a >= 0.1){
			color = image4 ;
		}
		if (image3.a >= 0.1){
			color = image3 ;
		}
		if (image2.a >= 0.1){
			color = image2 ;
		}
		if (image1.a >= 0.1){
			color = image1 ;
		}
	}
	
	gl_FragColor = color ;
}