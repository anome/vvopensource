/*
{
  "CATEGORIES" : [
    "Effects"
  ],
  "DESCRIPTION" : "SeamlessX",
  "ISFVSN" : "2",
  "VSN" : ".01",
  "INPUTS" : [
    {
      "NAME" : "u_input",
      "TYPE" : "image"
    },
    {
      "NAME" : "u_seamX",
      "TYPE" : "float",
      "MAX" : 0.5,
      "DEFAULT" : 0.1,
      "MIN" : 0
    }
  ],
  "CREDIT" : "Roger Sodre"
}
*/

void main()
{
	vec2 st = isf_FragNormCoord;
	vec2 seam = vec2( u_seamX, 0.0 );
	vec2 size = 1.0 - seam;
	
	vec2 st1 = st * size;
	vec4 color1 = IMG_NORM_PIXEL(u_input, st1);
	
	vec2 st2 = st * size;
	vec2 a = vec2(0);
	
	if ( st.x < seam.x )
	{
		st2.x = (1.0 - seam.x + st.x);
		a.x = (st.x / seam.x);
	}
	
	vec4 color = color1;
	if ( a.x > 0.0 || a.y > 0.0 )
	{
		vec4 color2 = IMG_NORM_PIXEL(u_input, st2);
		color = color1 * a.x + color2 * (1.0-a.x);
	}

	if ( st.y < seam.y )
	{
		st2.y = (1.0 - seam.y + st.y);
		a.y = (st.y / seam.y);
	}

	gl_FragColor = color;
}
