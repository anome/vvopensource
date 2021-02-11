/*
{
  "CATEGORIES" : [
    "Glitch"
  ],
  "DESCRIPTION" : "Keeps an accumulation of difference since a key frame",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "updateKeyFrame",
      "TYPE" : "bool",
      "DEFAULT" : 0,
      "LABEL" : "Update Key Frame"
    },
    {
      "NAME" : "adaptRate",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0,
      "LABEL" : "Adapt Rate",
      "MIN" : 0
    },
    {
      "NAME" : "numColors",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 1,
      "LABEL" : "Color Quality",
      "MIN" : 0
    },
    {
      "NAME" : "buffQuality",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 1.0,
      "LABEL" : "Buffer Quality",
      "MIN" : 0
    }
  ],
  "PASSES" : [
    {
      "TARGET" : "keyFrame",
      "PERSISTENT" : true
    },
    {
      "TARGET" : "diffFrame",
      "PERSISTENT" : true,
      "WIDTH" : "max(8.0,floor($WIDTH*$buffQuality))",
      "HEIGHT" : "max(8.0,floor($HEIGHT*$buffQuality))"
    },
    {
      "TARGET" : "lastFrame",
      "PERSISTENT" : true
    },
    {

    }
  ],
  "CREDIT" : "by zoidberg"
}
*/
void main()
{
    float ho = gl_FragCoord.y/100.;
    gl_FragColor = vec4(ho,ho,ho, 1);
}
