/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2.0",
	"VSN": "2.0",
	"CATEGORIES": [
		""
	],
	"INPUTS": [
		
		{
			"NAME": "N",
			"TYPE": "float",
			"DEFAULT": 12,
			"MIN": 0,
			"MAX": 22
		},
		{
			"NAME": "magnify",
			"TYPE": "float",
			"DEFAULT": 12,
			"MIN": 0,
			"MAX": 1000
		},
			{
			"NAME": "seed",
			"TYPE": "float",
			"DEFAULT": 12,
			"MIN": 0,
			"MAX": 100
		},
		
		{
			"NAME": "UTIME",
			"TYPE": "float",
			"DEFAULT": 0.01,
			"MIN": 0,
			"MAX": 2
		},
			{
			"NAME": "shift",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 20
		},
			{
			"NAME": "test",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 1
		}
		,
			{
			"NAME": "harmony",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 5
		}
		,
			{
			"NAME": "mharmony",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 5
		}
		,
			{
			"NAME": "magpow",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 5
		}
	]
}*/




void main() {
//	vec2 v = isf_FragNormCoord
	
	
    float MTIME =UTIME*TIME;
	float x = gl_FragCoord.x/pow(magnify,magpow);
	float y = gl_FragCoord.y/pow(magnify,magpow);
	
//	int n = mod(1.0,N)    
	
	float t = MTIME * 0.4;
	float r;
	for ( int i = 0; i < 20; i++ ){
		float d = 3.14159265*test /float(20) * float(i) * seed;
		r = length(vec2(x,y)) + shift;
		float xx = x;
		x = x + cos(y +cos(r) + d) + cos(t);
		y = y - sin(xx+cos(r) + d) + sin(t);
	}

	gl_FragColor = vec4( cos(r*sin(MTIME*mharmony*harmony)), cos(r*cos(MTIME*mharmony+mharmony*harmony)), cos(r*sin(MTIME*mharmony+mharmony+mharmony*harmony)), 1.0 );


}