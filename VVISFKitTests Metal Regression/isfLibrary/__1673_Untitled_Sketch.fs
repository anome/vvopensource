/*{
	"CREDIT": "by thedantheman",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
  {
 "NAME": "startColor",
 "TYPE": "float",
 "MAX" : 1.0,
 "MIN" : -1.0,
 "DEFAULT":0.0
 },
   {
 "NAME": "pos",
 "TYPE": "float",
 "MAX" : 1.0,
 "MIN" : -1.0,
 "DEFAULT":0.0
 },
   {
 "NAME": "offset",
 "TYPE": "float",
 "MAX" : 1.0,
 "MIN" : -1.0,
 "DEFAULT":0.0
 },
    {
 "NAME": "thickness",
 "TYPE": "float",
 "MAX" : 0.10,
 "MIN" : 0.0,
 "DEFAULT":0.0
 }

	]
}*/

float plot(vec2 st, float pct){
  return  smoothstep( pct*pct, pct, st.y) - 
          smoothstep( pct, pct*pct, st.y);
}

void main(void){
	float t = pos;
	vec2 r = RENDERSIZE;
	vec2 p = (gl_FragCoord.xy)/RENDERSIZE.x - vec2(0.5, 0.25);
	vec3 destColor = vec3(startColor);
	for (float i = 0.0; i<5.0;i++){
		float j = i + 1.0;
		vec2 q =  p + vec2(cos(t * j), sin(t * j)) * offset;
		float l = thickness / abs(length(q)-0.2*i*abs(tan(t)));
		destColor +=  vec3(abs(tan(t))*l,abs(sin(t))*l,abs(cos(t))*l);
	}
	gl_FragColor = vec4(destColor, 1.0);
}