/*{
  "CREDIT": "by msfeldstein",
  "DESCRIPTION": "",
  "CATEGORIES": [
	"generator"
  ],
  "INPUTS": [
    {
      "NAME": "timeMult",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 10
    }
  ]
}*/


void main() {
	float y =step(1.0, mod(-TIME * timeMult - gl_FragCoord.y / RENDERSIZE.y, 1.10));
	float c = step(1.0, y);
	gl_FragColor = vec4(y);
}
