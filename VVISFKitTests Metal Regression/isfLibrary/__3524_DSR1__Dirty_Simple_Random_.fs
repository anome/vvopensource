/*
{
  "CATEGORIES" : [
    "generator", "glitch", "pattern"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "speed",
      "TYPE" : "float",
      "MAX" : 0.10000000000000001,
      "DEFAULT" : 0.001,
      "LABEL" : "speed",
      "MIN" : 0
    },
    {
      "NAME" : "variance",
      "TYPE" : "float",
      "MAX" : 0.1,
      "DEFAULT" : 0.01,
      "MIN" : 0.0000001
    },{
    	"NAME" : "horizontalMirror",
    	"TYPE" : "bool",
    	"LABEL" : "horizontal mirror",
    	"DEFAULT" : "true"	
    },{
    	"NAME" : "verticalMirror",
    	"TYPE" : "bool",
    	"LABEL" : "vertical mirror",
    	"DEFAULT" : "true"	
    },{
    	"NAME" : "grey",
    	"TYPE" : "float",
    	"LABEL" : "grey amounnt",
    	"DEFAULT" : "0.0",
    	"MIN" : 0.0,
    	"MAX" : 0.499
    }

  ],
  "CREDIT" : "Subtiv"
}
*/

float random (vec2 st) {
    return fract(sin(dot(st.xy,
                         vec2(12.9898,78.233)))*
        43758.5453123);
}

float sinn(float val){
  return .5+.5*sin(val);
}

void main()	{
	
  vec2 pos;
  float time = TIME*.01;

  pos = isf_FragNormCoord.xy;  

  if (isf_FragNormCoord.x > .5 && horizontalMirror){
    pos = vec2(1.0 - pos.x, pos.y);
  }

  if (isf_FragNormCoord.y > .5 && verticalMirror){
    pos = vec2(pos.x, 1.0 - pos.y );
  } 
  
  float variancet = pow(variance, 3.0);

  float c = random(pos*variancet + sinn(time*speed));
  c += random(pos*variancet + sinn(1.23 + time*speed));
  c /= 2.0;


  vec4 col = vec4(c, c, c, 1.0);
  //col = step(0.09, col);
  col = smoothstep(grey, 1.0 - grey, col);

  gl_FragColor = col;
}
