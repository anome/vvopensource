/*{
	"ISFVSN": "2",
	"INPUTS": [
  {
  "NAME": "inputImage",
  "TYPE": "image"
  },
  {
  "NAME": "repeats",
  "TYPE": "float",
  "MIN": 1.0,
  "MAX": 18.0,
  "DEFAULT": 2.0
  },
  {
  "NAME": "rotate",
  "TYPE": "float",
  "MIN": 0.0,
  "MAX": 1000.0,
  "DEFAULT": 0.0
  },
  {
  "NAME": "cropX",
  "TYPE": "float",
  "MIN": 0.0,
  "MAX": 1.0,
  "DEFAULT": 0.0
  },
  {
  "NAME": "cropY",
  "TYPE": "float",
  "MIN": 0.0,
  "MAX": 1.0,
  "DEFAULT": 0.0
  },
  {
  "NAME": "cropScaleX",
  "TYPE": "float",
  "MIN": -1.0,
  "MAX": 1.0,
  "DEFAULT": 1.0
  },
  {
  "NAME": "cropScaleY",
  "TYPE": "float",
  "MIN": -1.0,
  "MAX": 1.0,
  "DEFAULT": 1.0
  },
  {
  "NAME": "moveXY",
  "TYPE": "point2D",
  "MIN": [-2.0, -2.0],
  "MAX": [2.0, 2.0],
  "DEFAULT": [0.0, 0.0]
  },
  {
  "NAME": "zoom",
  "TYPE": "float",
  "MIN": 0.0001,
  "MAX": 2.0,
  "DEFAULT": 1.0
  }
	]
  }*/

vec2 ZoomAndTranslateUV(vec2 uv, vec2 offset, float zoomFactor)
{//Zoom UV Around the center of current screen position;
	vec2 halfz=vec2(0.5)+offset;
	return (uv+offset-halfz)*zoomFactor+halfz;
}

vec2 rotateUV(vec2 uv, float rotation, vec2 mid)
{
   return vec2(
     cos(rotation) * (uv.x - mid.x) + sin(rotation) * (uv.y - mid.y) + mid.x,
     cos(rotation) * (uv.y - mid.y) - sin(rotation) * (uv.x - mid.x) + mid.y
   );
}

vec4 makeWallpaper(vec2 uv ){
   //Odd variations - Mirroring
   int oddX=2;
   int oddY=2;
   vec2 iXYScale = vec2(cropScaleX, cropScaleY);

   if (mod(uv.y, oddY) >= 1.0){
      uv.y*=-1;
   }
   if (mod(uv.x, oddX) >= 1.0){
      uv.x*=-1;
   }
   //MakingCrop
   float xScaleOffset=(1-iXYScale.x)*0.5;
   float yScaleOffset=(1-iXYScale.y)*0.5;
   uv.x=mix(xScaleOffset+cropX, 1-xScaleOffset+cropX, fract(uv.x));
   uv.y=mix(yScaleOffset+cropY, 1-yScaleOffset+cropY, fract(uv.y));

   return IMG_NORM_PIXEL(inputImage, uv);
}


void main() {
  vec2 uv = isf_FragNormCoord;
  uv=ZoomAndTranslateUV(uv, moveXY, zoom);
  uv*=repeats;
  uv=rotateUV(uv, rotate*3.14, vec2(0.5, 0.5)*repeats);
  gl_FragColor = makeWallpaper(uv);
  
  //gl_FragColor = vec4(frac(uv.x),frac(uv.y),0,1);
  //gl_FragColor=IMG_PIXEL(inputImage, isf_FragNormCoord);
}
