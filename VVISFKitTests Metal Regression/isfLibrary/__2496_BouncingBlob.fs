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
      "MAX" : 1.0,
      "DEFAULT" : 0.5,
      "MIN" : -0.5
    },
    {
      "NAME" : "Radius2",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.5,
      "MIN" : -0.5
    },
    {
      "NAME" : "startColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0.9,
        0.5,
        0.0,
        1
      ]
    },
    {
      "NAME" : "endColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0.7,
        0.2,
        0.3,
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

//to move the circle from bl to tr 
//pct = distance(st,vec2(Radius)) + distance(st,vec2(Radius));
//pct = distance(st,vec2(0.8)) * distance(st,vec2(0.8));
//pct = min(distance(st*2.0,vec2(Radius)),distance(st*2.0,vec2(Radius)));
//pct = max(distance(st,vec2(Radius)),distance(st,vec2(Radius)));
pct = pow(distance(st*1.5,vec2(cos ((sin (Radius*TIME/6.0)) *TIME))),distance(st*1.5,vec2(sin (Radius2*TIME*2.0))));

    //to have an animated pulse:
//pct = (sin (TIME * 2.0))*3.0*distance(st,vec2(0.5));

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
	if  (pct < 0.75) {gl_FragColor = vec4 (vec3 ( startColor/(2.0*pct)),  1.0 )
;}
else if (pct > 0.75)	{
		gl_FragColor = vec4 (vec3 (endColor/ (6.0* pct)), 1.0);

	
// to define the colors with straight edges :
//if  (pct < 0.7) {gl_FragColor = vec4 (vec3 (startColor),  1.0 )
//;}
//else if (pct > 0.7)	{
		//gl_FragColor = vec4 (vec3 (endColor), 1.0);
	}
}


