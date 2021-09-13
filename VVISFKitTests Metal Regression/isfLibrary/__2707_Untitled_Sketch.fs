/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "VSN" : "0.1",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "image01",
      "TYPE" : "image",
      "LABEL" : "image01"
    },
    {
      "NAME" : "image02",
      "TYPE" : "image",
      "LABEL" : "image02"
    },
    {
      "NAME" : "splitAngle",
      "TYPE" : "float",
      "MAX" : 3,
      "DEFAULT" : 0.5,
      "LABEL" : "splitAngle",
      "MIN" : 0
    },
    {
      "NAME" : "imageTrack",
      "TYPE" : "float",
      "DEFAULT" : 100,
      "LABEL" : "imageTrack"
    }
  ],
  "DESCRIPTION" : "Twin Slit Scan",
  "CREDIT" : "Nathan Adams"
}
*/


float pi = 3.141592;
float halfPi = pi/2.0;

float distToLine(vec2 pt1, vec2 pt2, vec2 testPt)
{
  vec2 lineDir = pt2 - pt1;
  vec2 perpDir = vec2(lineDir.y, -lineDir.x);
  vec2 dirToPt1 = pt1 - testPt;
  return dot(normalize(perpDir), dirToPt1);
}

void main()	{
	
	//get distance from split line	
	vec2 sp1 = vec2(((tan(halfPi*splitAngle) * -0.5) + 0.5),0);
	vec2 sp2 = vec2(((tan(halfPi*splitAngle) * 0.5) + 0.5),1);	
	//as working with normalised posits, these should be reasonaly normalled to 0.5 - so *2 to approximate 1
	float sd = distToLine(sp1,sp2,isf_FragNormCoord) * 2.0;		//split distance - used to determine which half we are in
	float ad = abs(sd);											//absolute distance - used for calculating stuff
	
	//get distance along the line
	vec2 xp1 = vec2(((tan(halfPi*splitAngle*-1.0) * -0.5) + 0.5),0);
	vec2 xp2 = vec2(((tan(halfPi*splitAngle*-1.0) * 0.5) + 0.5),1);	
	//as working with normalised posits, these should be reasonaly normalled to 0.5 - so *2 to approximate 1
	float sx = distToLine(xp1,xp2,isf_FragNormCoord) * 2.0;		//split distance - used to determine which half we are in
	float ax = abs(sx); 
	
	
	if (sd < 0.0) {
		gl_FragColor = vec4(1.0-ad,0.0,0.0,1.0-ax);
		vec4 test = IMG_NORM_PIXEL(image01,isf_FragNormCoord.xy);
		gl_FragColor = test;
	} else {
		gl_FragColor = IMG_NORM_PIXEL(image02,isf_FragNormCoord.yx);
		//gl_FragColor = vec4(0.0,1.0,0.0,1.0-ad);
		
	}
//		gl_FragColor = IMG_PIXEL(image01,gl_FragCoord.xy);

}
		