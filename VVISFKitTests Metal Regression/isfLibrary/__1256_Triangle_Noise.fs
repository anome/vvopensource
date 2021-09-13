/*{
	"CREDIT": "by isak.burstrom",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"filter",
		"INKA"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}, {
			"NAME": "amount",
			"TYPE": "float"
		}, {
			"NAME": "speed",
			"TYPE": "float"
		}
	]
}*/

//source https://www.shadertoy.com/view/Mts3zM

float aspect = RENDERSIZE.x/RENDERSIZE.y;
float w = 50./sqrt(RENDERSIZE.x*aspect+RENDERSIZE.y);

mat2 mm2(in float a){float c = cos(a), s = sin(a);return mat2(c,-s,s,c);}
float tri(in float x){return abs(fract(x)-.5);}
vec2 tri2(in vec2 p){return vec2(tri(p.x+tri(p.y*2.)),tri(p.y+tri(p.x*2.)));}
mat2 m2 = mat2( 0.970,  0.242, -0.242,  0.970 );

//Animated triangle noise, cheap and pretty decent looking.
float triangleNoise()
{
	vec2 p = gl_FragCoord.xy / RENDERSIZE.xy*2.-1.;
	p = p / (w*w);
    float z=1.5;
    float z2=1.5;
	float rz = 0.;
    vec2 bp = p;
	for (float i=0.; i<=3.; i++ )
	{
        vec2 dg = tri2(bp*2.)*.8;
        dg *= mm2(TIME * speed);
        p += dg/z2;

        bp *= 1.6;
        z2 *= .6;
		z *= 1.8;
		p *= 1.2;
        p*= m2;
        
        rz+= (tri(p.x+tri(p.y)))/z;
	}
	
	return (rz * 0.9 + 0.4) - .66;
}

void main() {
	vec4 color = IMG_THIS_PIXEL(inputImage);

    color += triangleNoise() * amount;
    
	gl_FragColor = color;
    
}
