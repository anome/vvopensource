/*{
	"CREDIT": "by PALUCK",
	"CATEGORIES" : [ "Factal de The Art of Code"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
  	{
     	"NAME" :		"Occurence",
     	"TYPE" : 		"long",
    	"DEFAULT" :		3,
     	"VALUES" : [0,1,2,3,4,5,6],
      	"LABELS" : [1,2,3,4,5,6,7]
	},
  	{
     	"NAME" :		"Extension",
     	"TYPE" : 		"float",
    	"DEFAULT" :		1.0,
     	"MIN" : 		-2.0,
      	"MAX" :			5.0
	},
  	{
     	"NAME" :		"Flou",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0.0,
     	"MIN" : 		0.0,
      	"MAX" :			4.0
	},
  	{
     	"NAME" :		"Force",
     	"TYPE" : 		"float",
    	"DEFAULT" :		1.0,
     	"MIN" : 		0.0,
      	"MAX" :			10.0
	},
	{
     	"NAME" :		"Variant1",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0.6,
     	"MIN" : 		-2.0,
      	"MAX" :			2.0
	},
  	{
     	"NAME" :		"Variant2",
     	"TYPE" : 		"float",
    	"DEFAULT" :		0.6,
     	"MIN" : 		-2.0,
      	"MAX" :			2.0
	},
	{
     	"NAME" :		"Angle1",
     	"TYPE" : 		"float",
    	"DEFAULT" :		1.0,
     	"MIN" : 		0.0,
      	"MAX" :			1.0
	},
  	{
     	"NAME" :		"Angle2",
     	"TYPE" : 		"float",
    	"DEFAULT" :		1.0,
     	"MIN" : 		0.0,
      	"MAX" :			1.0
	},	{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	]

}
*/

uniform vec2 resolution;
uniform vec2 mouse;
uniform float time;

vec2 N(float angle) {
    return vec2(sin(angle*Angle1), cos(angle*Angle2));
}

void main() 
{
    vec2 uv = ( gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    
    uv *= 1.5;
    vec3 col = vec3(0.0);
    
    uv.x = abs(uv.x);
    uv.y += tan((5.0/6.0)*3.1415)*0.5;
    vec2 n = N((5.0/6.0)*3.1415);
    float d =dot(uv -vec2(0.5, 0.0), n);
    uv -= n*max(0.0,d)*2.0;
    
//   col += smoothstep(0.01,0.0, abs(d));
    
    n = N((Variant1+Variant2*0.1+Variant1*0.01)*3.1415);
    float scale = Force;
    uv.x += 0.5;
  int oc = 0;
  for (int i=0; i<10; i++) {  
    oc += 1;
    uv *= 3.0;
    scale *= 3.0;
    uv.x -= 1.5;
    
    uv.x = abs(uv.x);
    uv.x -= 0.5;
    uv -= n*min(0.0, dot(uv,n))*2.0;
    if(oc > Occurence) {break;};
  }
  
    
    d = length(uv - vec2(clamp(uv.x, -1.0, 1.0), 0));
    col += smoothstep(1.0/RENDERSIZE.y,Flou, d/scale);
    uv /= scale;

//    col += IMG_THIS_PIXEL(inputImage).rgb;
//    vec2 uvt  = vec2(uv.x*100.0*sin(TIME*0.3),uv.y*40.0*tan(TIME*0.2) );
    vec2 uvt  = vec2(uv.x,uv.y)*2.0*Extension;
//    vec2 uvt  = vec2(uv.x,uv.y)*Nb;
    col += IMG_NORM_PIXEL(inputImage,uvt).rgb;

    gl_FragColor = vec4(col, 1.0);
    
}