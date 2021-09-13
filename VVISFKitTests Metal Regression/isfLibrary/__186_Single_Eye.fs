/*{
	"DESCRIPTION": "Eye",
	"CREDIT": "Jon D.",
	"CATEGORIES": [
		"Eyes"
	],
	"INPUTS": [
    	{	"LABEL":		"Use Color Pallet 1",
    		"NAME":			"Pupilcolor",
    		"TYPE":			"color",
    		"DEFAULT":		[0.0,0.0,0.0,1.0]
    	},
    	{	"LABEL":		"Use Color Pallet 2",
    		"NAME":			"Iriscolor",
    		"TYPE":			"color",
    		"DEFAULT":		[0.0,0.0,1.0,1.0]
    	},
    	{	"LABEL":		"Use Color Pallet 3",
    		"NAME":			"Eyeballcolor",
    		"TYPE":			"color",
    		"DEFAULT":		[1.0,1.0,1.0,1.0]
    	},
    	{	"LABEL":		"Use Color Pallet 4",
    		"NAME":			"Eyelidcolor",
    		"TYPE":			"color",
    		"DEFAULT":		[1.0,1.0,0.0,1.0]
    	},
    	{	"LABEL":		"Use Color Pallet 5",
    		"NAME":			"Backcolor",
    		"TYPE":			"color",
    		"DEFAULT":		[0.2,0.2,0.2,1.0]
    	},
    	{
    		"LABEL":		"Pupil Radius",
    		"NAME" : 		"r_pupil",
    		"TYPE" : 		"float",
    		"DEFAULT" : 	0.10,
    		"MIN" : 		0.0,
    		"MAX" : 		0.5
    	},
    	{
    		"LABEL":		"Iris Radius",
    		"NAME" : 		"r_iris",
    		"TYPE" : 		"float",
    		"DEFAULT" : 	0.25,
    		"MIN" : 		0.0,
    		"MAX" : 		0.5
    	},
    	{
    		"LABEL":		"Inner Eye Lid Width",
    		"NAME" : 		"width_ei",
    		"TYPE" : 		"float",
    		"DEFAULT" : 	0.80,
    		"MIN" : 		0.0,
    		"MAX" : 		1.0
    	},
    	{
    		"LABEL":		"Inner Eye Lid Height",
    		"NAME" : 		"height_ei",
    		"TYPE" : 		"float",
    		"DEFAULT" : 	0.3,
    		"MIN" : 		0.0,
    		"MAX" : 		1.0
    	},
    	{
    		"LABEL":		"Outer Eye Lid Width",
    		"NAME" : 		"width_eo",
    		"TYPE" : 		"float",
    		"DEFAULT" : 	0.80,
    		"MIN" : 		0.0,
    		"MAX" : 		1.0
    	},
    	{
    		"LABEL":		"Outer Eye Lid Height",
    		"NAME" : 		"height_eo",
    		"TYPE" : 		"float",
    		"DEFAULT" : 	0.4,
    		"MIN" : 		0.0,
    		"MAX" : 		1.0
    	},
        {
    		"LABEL": 		"Pupil Center Location",
    		"NAME": 		"PupilCenter",
    		"TYPE": 		"point2D",
    		"DEFAULT": 		[0.5,0.5]
        },
        {
    		"LABEL": 		"Eye Center Location",
    		"NAME": 		"EyeCenter",
    		"TYPE": 		"point2D",
    		"DEFAULT": 		[0.5,0.5]
        },
       	{
       		"LABEL":		"Use Cat Eye Pupil",
    		"NAME" : 		"cateye",
         	"TYPE" : 		"bool",
         	"DEFAULT" : 	0.0
       	},
    	{
    		"LABEL":		"Cat Eye Width",
    		"NAME" : 		"cateyewidth",
    		"TYPE" : 		"float",
    		"DEFAULT" : 	0.02,
    		"MIN" : 		0.0,
    		"MAX" : 		1.0
    	},
    	{
    		"LABEL":		"Cat Eye Height",
    		"NAME" : 		"cateyeheight",
    		"TYPE" : 		"float",
    		"DEFAULT" : 	0.40,
    		"MIN" : 		0.0,
    		"MAX" : 		1.0
    	}
]
}*/

float xpos(vec2 pos, vec2 ratio, float width, vec2 center) {
	return (pos.x - (ratio.x * (1.0 - width)) / 2.0 - (center.x - 0.5) * ratio.x);
}

float yval(float xpos, vec2 ratio, float height, float width) {
	return (((-4.0 * height / pow((ratio.x * width),2.0))*pow(xpos,2.0) + (4.0 * height / (ratio.x * width)) * (xpos)) + 0.5);
}

float ypos(vec2 pos, vec2 ratio, float height, vec2 center) {
	return (pos.y - (ratio.y * (1.0 - height)) / 2.0 - (center.y - 0.5)*ratio.y);
}

float xval(float ypos, vec2 ratio, float height, float width) {
	return (((-4.0 * width / pow((ratio.y * height),2.0))*pow(ypos,2.0)+(4.0 * width/(ratio.y * height)) * (ypos)) +0.5);
}


void main()
{    
    vec4 color = vec4(0.0);
	vec2 ratio = vec2(1.0);
	vec2 pos = ( gl_FragCoord.xy / RENDERSIZE.xy );
	if (RENDERSIZE.y>RENDERSIZE.x){
		ratio.y = RENDERSIZE.y/RENDERSIZE.x;
		}
		else
		{
		ratio.x = RENDERSIZE.x/RENDERSIZE.y;
	}
    vec2 normpos = pos;
	normpos.y = pos.y * ratio.y;
	normpos.x = pos.x * ratio.x;
	vec2 modPupilCenter = ratio * PupilCenter;
	float radius = distance(normpos,modPupilCenter);

	float x_inner = xpos(normpos,ratio,width_ei,EyeCenter);
	float y_inner = yval(x_inner,ratio,height_ei,width_ei);
	float x_outer = xpos(normpos,ratio,width_eo,EyeCenter);
	float y_outer = yval(x_outer,ratio,height_eo,width_eo);
	float ycateye = ypos(normpos,ratio,cateyeheight,PupilCenter);
	float xcateye = xval(ycateye,ratio,cateyeheight,cateyewidth);
	
// Set the color of the eyeball	
	if (pos.y < y_inner) {color = Eyeballcolor;}

// Set the color of the Iris
	if(radius <= r_iris) {color = Iriscolor;}	

//Set the color of the pupil
    if (cateye) {
		if ((pos.x-(PupilCenter.x-0.5)) < xcateye && (1.0-pos.x+(PupilCenter.x-0.5)) < xcateye) {color = Pupilcolor;};
		}
		else
		{if(radius <= r_pupil) {color = Pupilcolor;};
	}	

// Set the color of the eyelid
    if ((pos.y) < y_outer && (pos.y) > y_inner || (1.0-pos.y) < y_outer && (1.0-pos.y) > y_inner ) {color = Eyelidcolor;}

//Set the color of the backgound - anything behind the outer eyelid line
	if ((pos.y) >= y_outer || ((1.0-pos.y)) >= y_outer) {color = Backcolor;}	
   gl_FragColor = vec4(color);
}