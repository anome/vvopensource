/*{
  "CREDIT": "by Jon D.",
  "CATEGORIES" : [
    "None"
  ],
  "INPUTS" : [
	{	"LABEL":		"Use Color Pallet 1",
		"NAME":			"Beamcolor",
		"TYPE":			"color",
		"DEFAULT":		[1.0,1.0,1.0,1.0]
	},
	{	"LABEL":		"Use Color Pallet 2",
		"NAME":			"Backcolor",
		"TYPE":			"color",
		"DEFAULT":		[0.0,0.0,0.0,0.0]
	},	
    {	"LABEL": 		"Position",
    	"NAME": 		"pos",
    	"TYPE": 		"point2D",
    	"DEFAULT": 		[0.5,0.5]
    },	
	{
		"LABEL":		"Start Angle - 0 is top, 90 is right",
		"NAME" : 		"startang",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.0,
		"MIN" : 		0.0,
		"MAX" : 		360.0
	},
	{
		"LABEL":		"End Angle",
		"NAME" : 		"endang",
		"TYPE" : 		"float",
		"DEFAULT" : 	90.0,
		"MIN" : 		0.0,
		"MAX" : 		360.0
	},
	{
   		"LABEL":		"Invert Direction Movement",
		"NAME" : 		"reverse",
     	"TYPE" : 		"bool",
     	"DEFAULT" : 	0.0
   	},
	{
		"LABEL":		"Time for 1 Sweep",
		"NAME" : 		"sweeptime",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		0.1,
		"MAX" : 		60.0
	},
	{
		"LABEL":		"Center Beam Angle",
		"NAME" : 		"solidbeamangdeg",
		"TYPE" : 		"float",
		"DEFAULT" : 	10.0,
		"MIN" : 		0.0,
		"MAX" : 		360.0
	},
	{
		"LABEL":		"Fade Angle",
		"NAME" : 		"fadebeamangdeg",
		"TYPE" : 		"float",
		"DEFAULT" : 	30.0,
		"MIN" : 		0.0,
		"MAX" : 		180.0
	}
  ],
  "ISFVSN" : 2.0
}
*/

const float PI = 3.14159265359;
const float PI2 = PI*2.0;

//0 deg is up, 90 is to right

void main() {
	vec2 position = ( gl_FragCoord.xy / RENDERSIZE.xy ) - vec2 (pos.x,pos.y);
	position.y *= RENDERSIZE.y/RENDERSIZE.x;
	float ang = atan(position.x, position.y);
	float speed = 1.0/sweeptime; 
	float solid = solidbeamangdeg*PI/(2.0*180.0);
	float fade = fadebeamangdeg*PI/(180.0);
	float endangmod = 0.0;
	if (reverse){
		endangmod = endang-360.0;
		} 
		else {
		endangmod = (endang);
	}
	float sweepdeg = startang-endangmod;
	float sweep = sweepdeg*PI/(2.0*180.0);
	float offset = ((startang+endangmod)/2.0)*PI/180.0;
	float ray = sweep*cos(TIME*PI*speed)+offset;
	ray = mod(ray, PI2);
		if (ray < ang - PI) {ray += PI2;}
		if (ray > ang + PI) {ray -= PI2;}
	vec4 color = Backcolor;
	if (abs(ang - ray)<solid) {color = Beamcolor;}
	if (abs(ang - ray)>=solid && abs(ang - ray)<=solid+fade) {
	    color = ((Backcolor-Beamcolor)/fade)*(abs(ang - ray)-solid)+Beamcolor;
	    
	}
	if (abs(ang - ray)>solid+fade) {color = Backcolor;}
	gl_FragColor.rgb = vec3(color);
	gl_FragColor.a = 1.0;
}
