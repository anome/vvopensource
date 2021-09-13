/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	{
			"NAME": "BackColor",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.4,
				0.58,
				1.0
			]
		},
		{
			"NAME": "CloudColor",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.4,
				0.58,
				1.0
			]
		},
		{
			"NAME": "CloudSize",
			"TYPE": "float",
			"DEFAULT": 3.5,
			"MIN": 0.25,
			"MAX": 20.0
		},
		{
			"NAME": "CloudDetail",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.3
		},
		{
			"NAME": "NumLayers",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.01,
			"MAX": 1.0
		},
		{
			"NAME": "LayerSpread",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": 0.25,
			"MAX": 20.0
		},
		{
			"NAME": "Blurriness",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 0.0,
			"MAX": 20.0
		},
		{
			"NAME": "Zoom",
			"TYPE": "float",
			"DEFAULT": 18.0,
			"MIN": 0.0,
			"MAX": 20.0
		},
		{
			"NAME": "OffsetY",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.0,
			"MAX": 10.0
		}
	]
}*/

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

// ----------------------------------------------------------------------------------------
//	"Toon Cloud" by Antoine Clappier - March 2015
//
//	Licensed under:
//  A Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.
//	http://creativecommons.org/licenses/by-nc-sa/4.0/
// ----------------------------------------------------------------------------------------

#define TAU 6.28318530718


// const vec3 BackColor	= vec3(0.0, 0.4, 0.58);
// const vec3 CloudColor = vec3(0.18,0.70,0.87);


float Func(float pX)
{
	return 0.6*(0.5*sin(0.1*pX) + 0.5*sin(0.553*pX) + 0.7*sin(1.2*pX));
}


float FuncR(float pX)
{
	return CloudDetail + 0.25*(1.0 + sin(mod(40.0*pX, TAU)));
}


float Layer(vec2 pQ, float pT)
{
	vec2 Qt = CloudSize*pQ;
	pT *= 1.5;
	Qt.x += pT;

	float Xi = floor(Qt.x);
	float Xf = Qt.x - Xi -0.5;

	vec2 C;
	float Yi;
	float D = 1.0 - step(Qt.y,  Func(Qt.x));

	// Disk:
	Yi = Func(Xi + 0.5);
	C = vec2(Xf, Qt.y - Yi ); 
	D =  min(D, length(C) - FuncR(Xi+ pT/80.0));

	// Previous disk:
	Yi = Func(Xi+1.0 + 0.5);
	C = vec2(Xf-1.0, Qt.y - Yi ); 
	D =  min(D, length(C) - FuncR(Xi+1.0+ pT/80.0));

	// Next Disk:
	Yi = Func(Xi-1.0 + 0.5);
	C = vec2(Xf+1.0, Qt.y - Yi ); 
	D =  min(D, length(C) - FuncR(Xi-1.0+ pT/80.0));

	return min(1.0, D);
}



void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	// Setup:
	vec2 UV = (20.-Zoom)*(fragCoord.xy - vec2(iResolution.x, iResolution.y/OffsetY)) / min(iResolution.x, iResolution.y);	
	
	// Render:
	vec3 Color= vec3(BackColor);

	for(float J=0.0; J<=1.0; J+=0.2)
	{
		// Cloud Layer: 
		float Lt =  iGlobalTime*(0.5  + 2.0*J)*(1.0 + 0.1*sin(226.0*J)) + 17.0*J;
		vec2 Lp = vec2(0.0, 0.3+LayerSpread*( J - 0.5));
		float L = Layer(UV + Lp, Lt);

		// Blur and color:
		float Blur = Blurriness*(0.5*abs(2.0 - 5.0*J))/(11.0 - 5.0*J);

		float V = mix( 0.0, 1.0, 1.0 - smoothstep( 0.0, 0.01 +0.2*Blur, L ) );
		vec3 Lc=  mix( vec3(CloudColor), vec3(1.0), J);

		Color =mix(Color, Lc,  V);
	}

	fragColor = vec4(Color, 1.0);
}




void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}