/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "generator"
  ],
  "INPUTS" : [
  ],
  "DESCRIPTION" : "based on http://glslsandbox.com/e#37192.0"
}
*/

////////////////////////////////////////////////////////////
// MatrixRain  by mojovideotech
//
// based on :  
// glslsandbox.com/e#37192.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


void main( void ) 
{	gl_FragColor = vec4( 0.0, -mod( gl_FragCoord.y + TIME, cos( gl_FragCoord.x*0.75 )+.002 ), 0.0, 1.0 );
}