/*{
	"DESCRIPTION": "Rendora",
	"CREDIT": "chimanaco",
	"CATEGORIES": [ "Generator"],
	"INPUTS": [
        {
         "NAME": "rotationSpeed",
         "TYPE": "float",
         "DEFAULT": 0.2,
         "MIN": 0.0,
         "MAX": 2.0
        },
        {
        "NAME": "wrapSize",
        "TYPE": "float",
        "DEFAULT": 0.9,
        "MIN": 0.0,
        "MAX": 5.0
        },
        {
         "NAME": "repetition",
         "TYPE": "float",
         "DEFAULT": 6.0,
         "MIN": 0.0,
         "MAX": 30.0
        },
        {
           "NAME": "repDistance",
           "TYPE": "float",
           "DEFAULT": 2.0,
           "MIN": 0.1,
           "MAX": 10.0
        },
        {
           "NAME": "wrapSpeed",
           "TYPE": "float",
           "DEFAULT": 0.25,
           "MIN": 0.0,
           "MAX": 1.0
        },
        {
         "NAME": "mirrorDistance",
         "TYPE": "float",
         "DEFAULT": 0.5,
         "MIN": 0.0,
         "MAX": 1.0
        },
        {
        "NAME": "xWrap",
        "TYPE": "bool",
        "DEFAULT": 0.0
        },
        {
        "NAME": "yWrap",
        "TYPE": "bool",
        "DEFAULT": 0.0
        },
        {
        "NAME": "xMirror",
        "TYPE": "bool",
        "DEFAULT": 1.0
        },
        {
        "NAME": "yMirror",
        "TYPE": "bool",
        "DEFAULT": 1.0
        },
        {
        "NAME": "bodyColor",
        "TYPE": "color",
        "DEFAULT": [
            0.11764,
            0.72549,
            0.93725,
            1.0
        ]
        },
        {
         "NAME": "noseColor",
         "TYPE": "color",
         "DEFAULT": [
             0.91372,
             0.0,
             0.12156,
             1.0
         ]
        },
        {
         "NAME": "lineColor",
         "TYPE": "color",
         "DEFAULT": [
             0.0,
             0.0,
             0.0,
             1.0
         ]
        },
        {
         "NAME": "faceColor",
         "TYPE": "color",
         "DEFAULT": [
             1.0,
             1.0,
             1.0,
             1.0
         ]
        },
        {
         "NAME": "eyeColor",
         "TYPE": "color",
         "DEFAULT": [
             1.0,
          1.0,
          1.0,
          1.0
         ]
        },
        {
        "NAME": "bodyW",
        "TYPE": "float",
        "DEFAULT": 0.95,
        "MIN": 0.0,
        "MAX": 3.0
        },
        {
        "NAME": "bodyH",
        "TYPE": "float",
        "DEFAULT": 0.85,
        "MIN": 0.0,
        "MAX": 3.0
        },
        {
          "NAME": "faceW",
          "TYPE": "float",
          "DEFAULT": 0.75,
          "MIN": 0.0,
          "MAX": 3.0
        },
        {
            "NAME": "faceH",
            "TYPE": "float",
            "DEFAULT": 0.65,
            "MIN": 0.0,
            "MAX": 3.0
        },
        {
               "NAME": "eyesW",
               "TYPE": "float",
               "DEFAULT": 0.30,
               "MIN": 0.0,
               "MAX": 3.0
        },
        {
               "NAME": "eyesH",
               "TYPE": "float",
               "DEFAULT": 0.30,
               "MIN": 0.0,
               "MAX": 3.0
        },
        {
               "NAME": "eyeBallX",
               "TYPE": "float",
               "DEFAULT": 0.15,
               "MIN": 0.0,
               "MAX": 3.0
        },
        {
               "NAME": "eyeBallY",
               "TYPE": "float",
               "DEFAULT": 0.15,
               "MIN": 0.0,
               "MAX": 3.0
        },
        {
               "NAME": "eyeBallW",
               "TYPE": "float",
               "DEFAULT": 0.1,
               "MIN": 0.0,
               "MAX": 3.0
        },
        {
               "NAME": "eyeBallH",
               "TYPE": "float",
               "DEFAULT": 0.1,
               "MIN": 0.0,
               "MAX": 3.0
        },
        {
               "NAME": "noseW",
               "TYPE": "float",
               "DEFAULT": 0.15,
               "MIN": 0.0,
               "MAX": 3.0
        },
        {
               "NAME": "noseH",
               "TYPE": "float",
               "DEFAULT": 0.15,
               "MIN": 0.0,
               "MAX": 3.0
        },
        {
               "NAME": "eyeBallSpeed",
               "TYPE": "float",
               "DEFAULT": 10.0,
               "MIN": 0.0,
               "MAX": 30.0
        }
	]
}*/

const float PI = 3.1415926;
vec4 destColor = vec4(0.0, 0.0, 0.0, 1.0);
const vec2 center = vec2(0., 0.);

void ellipse(vec2 p, vec2 offset, vec2 prop, float size, vec4 color, inout vec4 i){
  vec2 q = (p - offset) / prop;
  if(length(q) < size){i = color;}
}

void circle(vec2 p, vec2 offset, float size, vec4 color, inout vec4 i){
  float l = length(p - offset);
  if(l < size){i = color;}
}

void rect(vec2 p, vec2 offset, vec2 size, vec4 color, inout vec4 i){
  vec2 q = (p - offset) / size;
  if(abs(q.x) < size.x && abs(q.y ) < size.y){i = color;}
}

void circleLine(vec2 p, vec2 offset, float iSize, float oSize, vec4 color, inout vec4 i){
  vec2 q = p - offset;
  float l = length(q);
  if(l > iSize && l < oSize){i = color;}
}

void ellipseLine(vec2 p, vec2 offset, vec2 prop, float iSize, float oSize, vec4 color, inout vec4 i){
  vec2 q = (p - offset) / prop;
  float l = length(q);
  if(l > iSize && l < oSize){i = color;}
}

void arcLine(vec2 p, vec2 offset, float iSize, float oSize, float rad, float height, vec4 color, inout vec4 i){
  float s = sin(rad);
  float c = cos(rad);
  vec2 q = (p - offset) * mat2(c, -s, s, c);
  float l = length(q);
  if(l > iSize && l < oSize && q.y > height){i = color;}
}

float wrap(float x) {
  return abs(mod(x, 2.)-1.);
}

vec2 trans(vec2 p)
{
  float theta = atan(p.y, p.x);
  float r = length(p + TIME * 0.2);
  return vec2(theta, r);
}

void main()
{
	vec2 p = (gl_FragCoord.xy * 2.0 - RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);

    // Repetetion

      float size = wrapSize;

      if(xWrap) {
        p.x = mod(p.x, size);
        p.x = abs(p.x - size/2.);
        p.x = wrap(p.x + TIME * wrapSpeed);
      }

      if(yWrap) {
        p.y = mod(p.y, size);
        p.y = abs(p.y - size/2.);
        p.y = wrap(p.y + TIME * wrapSpeed);
      }

      if(xMirror) {
        p.x = abs(p.x - sin(TIME) * mirrorDistance);
      }

      if(yMirror) {
        p.y = abs(p.y - cos(TIME) * mirrorDistance);
      }


      // rotation
      if(rotationSpeed > 0.0) {
          float s = sin(TIME * rotationSpeed);
          float c = cos(TIME * rotationSpeed);
          mat2 m = mat2(c, s, -s, c); // 行列に回転用の値をセット
          p *= m;
      }

      if(repetition > 0.0) {
          p = trans(p);
          p = mod(p * repetition, repDistance)- repDistance / 2.0;
      }

      // scale
     //mat2 scale = mat2(cos(TIME) * 0.5 + 0.5, 0, 0, cos(TIME) * 0.5 + 0.5); // 行列に回転用の値をセット
     //p *= scale;

  	// body
    ellipse(p, vec2(.0, .0), vec2(bodyW, bodyH), 1.0, bodyColor, destColor);

    // face
    ellipse(p, vec2(.0, -.20), vec2(faceW, faceH), 1.0, faceColor, destColor);

    // eyes
    ellipse(p, vec2(.32, .35), vec2(eyesW, eyesH), 1.0, eyeColor, destColor);
    ellipse(p, vec2(-.32, .35), vec2(eyesW, eyesH), 1.0, eyeColor, destColor);

    // eyes line
    ellipseLine(p, vec2(0.32, 0.35),vec2(eyesW, eyesH), 1.0, 1.1, lineColor, destColor);
    ellipseLine(p, vec2(-0.32, 0.35),vec2(eyesW, eyesH), 1.0, 1.1, lineColor, destColor);

    // noseR
    ellipse(p, vec2(.0, .0), vec2(noseW, noseH), 1.0, noseColor, destColor);
    ellipseLine(p, vec2(0.0, 0.0),vec2(noseW, noseH), 1.0, 1.2, lineColor, destColor);

    // nose
    rect(p, vec2(.0, -0.45), vec2(0.1, 0.55), lineColor, destColor);

    // eyeBall
    ellipse(p, vec2(.32 + sin(TIME * eyeBallSpeed + p.x) * eyeBallX, .35 + cos(TIME * eyeBallSpeed) * eyeBallY), vec2(eyeBallW, eyeBallH), 1.0, lineColor, destColor);
    ellipse(p, vec2(-.32 + sin(TIME * -eyeBallSpeed + p.x) * eyeBallX, .35 + cos(TIME * eyeBallSpeed) * eyeBallY), vec2(eyeBallW, eyeBallH), 1.0, lineColor, destColor);

    // mouth
    arcLine(p, vec2(-.0, -.2), .55, .58, PI * 1.0, 0.3, lineColor, destColor);

    // bear L
    rect(p, vec2(-.3, -0.23), vec2(0.48, 0.12), lineColor, destColor);
    rect(p, vec2(-.3, -0.33), vec2(0.48, 0.12), lineColor, destColor);
    rect(p, vec2(-.3, -0.43), vec2(0.48, 0.12), lineColor, destColor);

    // bear R
    rect(p, vec2(.3, -0.23), vec2(0.48, 0.12), lineColor, destColor);
    rect(p, vec2(.3, -0.33), vec2(0.48, 0.12), lineColor, destColor);
    rect(p, vec2(.3, -0.43), vec2(0.48, 0.12), lineColor, destColor);

	gl_FragColor = vec4(destColor);
}
