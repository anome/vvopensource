
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
        }
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

float plot(vec2 st, float pct){
  return  smoothstep( pct-1.012, pct, st.y) -
          smoothstep( pct, pct+0.204, st.y);
}

void main() {
    vec2 st = gl_FragCoord.xy/RENDERSIZE;

    float y = pow(st.x,-0.336);

    vec3 color = vec3(y);

    float pct = plot(st,y);
    color = (1.344-pct)*color+pct*vec3(cos(TIME)*1.0,cos(TIME),sin(TIME));
    st.x *= 0.108*sin(TIME)+cos(TIME/st.y);
    st.y *= 0.108*sin(TIME)+cos(TIME/st.x);
	color.b *= abs(sin(TIME));
    color.b *= abs(sin(TIME)+-0.760*cos(color.r*TIME*1.994));
    color.b *= abs(cos(TIME)+1.696*sin(color.r*TIME));
    color.r *= abs(sin(TIME)+1.696*cos(color.r*TIME));
    gl_FragColor = vec4(color.r, 0.0, color.b,1.0);
}
