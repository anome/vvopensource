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

void main()
{
    gl_FragColor = texture2D(inputImage, isf_FragNormCoord);
}

