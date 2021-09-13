/*
{
  "CATEGORIES" : [
    "Generator"
  ],
  "DESCRIPTION" : "Simple Radial Rainbow",
  "ISFVSN" : "1",
  "INPUTS" : [
    {
      "NAME" : "center",
      "TYPE" : "point2D",
      "DEFAULT" : [
        411.8477,
        178.1289
      ],
      "MIN" : [
        0,
        0
      ],
      "MAX" : [
        1280,
       	1024
      ]
    },
    {
      "NAME" : "colorCycle",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 0,
      "MIN" : -10
    },
    {
      "NAME" : "autoCycle",
      "TYPE" : "bool",
      "DEFAULT" : true,
      "LABEL" : "Auto Cycle Colors"
    },
    {
      "NAME" : "distanceScale",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : -0.005681817,
      "MIN" : -1
    },
    {
      "NAME" : "modulus",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 0,
      "MIN" : -10
    }
  ],
  "CREDIT" : "Eliot Lash"
}
*/

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main()	{
	float dist = distance(center, gl_FragCoord.xy ) * distanceScale;
	
	float hue = dist + colorCycle;

   	hue += (TIME / 2.0) * float(autoCycle);
   	
   	hue = mod(hue, modulus);
	
	gl_FragColor = vec4(hsv2rgb(vec3(hue, 1.0, 1.0)), 1.0);
}
