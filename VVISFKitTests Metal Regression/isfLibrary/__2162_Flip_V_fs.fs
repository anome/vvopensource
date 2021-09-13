/*{
  "CREDIT": "by VIDVOX",
  "CATEGORIES": [
    "Geometry Adjustment"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    }
  ]
}*/

void main() {
	v_texcoord = vec2(a_texcoord.s, 1.0 - a_texcoord.t);

}