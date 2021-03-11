
/*{
  "DESCRIPTION": "Checking texture2D works",
  "CREDIT": "by Anomes",

  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    }
  ],
}*/

void someSampling(sampler2D sampler, vec2 coord) {
    return;
}
void main()
{
    someSampling(inputImage, vec2(0,0));
    gl_FragColor = vec4(1,1,1,1);
}

