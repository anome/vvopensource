/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "Radius",
      "TYPE" : "float",
      "MAX" : 2.5,
      "DEFAULT" : 0.5,
      "MIN" : 0
    },
    {
      "NAME" : "startColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0,
        0.5,
        1,
        1
      ]
    },
    {
      "NAME" : "endColor",
      "TYPE" : "color",
      "DEFAULT" : [
        1,
        1,
        1,
        1
      ]
    }
  ],
  "CREDIT" : ""
}
*/

#ifdef GL_ES
precision mediump float;
#endif

void main(){
	vec2 st = gl_FragCoord.xy/RENDERSIZE;
    float pct = 1.0;


    // a. The DISTANCE from the pixel to the center
    //pct = Radius*3.0*distance(st,vec2(0.5));
    //to have an animated pulse:
pct = (sin (TIME * 0.2))*3.0*distance(st,vec2(0.5));

    // b. The LENGTH of the vector
    //    from the pixel to the center
  //vec2 toCenter = vec2(0.5)-st;
  //pct = length(toCenter);

    // c. The SQUARE ROOT of the vector
    //    from the pixel to the center
 //vec2 tC = vec2(0.5)-st;
 //pct = sqrt(tC.x*tC.x+tC.y*tC.y);
 
   

	//gl_FragColor = vec4 (colorA,  1.0 );
	
// to define the colors with blurred edges and shadow : 
	if  (pct < 0.7) {gl_FragColor = vec4 (vec3 ( 1.1,0.6, (2.0*pct)),  1.0 )
;}
else if (pct > 0.7)	{
		gl_FragColor = vec4 (vec3 ((0.5,0.4,0.0)/ (2.0* pct)), 1.0);

	
// to define the colors with straight edges :
//if  (pct < 0.7) {gl_FragColor = vec4 (vec3 (startColor),  1.0 )
//;}
//else if (pct > 0.7)	{
		//gl_FragColor = vec4 (vec3 (endColor), 1.0);
	}
}


