/*
	{
	"DESCRIPTION": "Import Notes: Shader Toy",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "By: Old Salt",
	"VSN": "1.0",
	"INPUTS":
		[
			{
			"NAME": "inputImage",
			"TYPE": "image"
			}
		]
	}
*/

/*

Shader Toy                          ISF
---------------------------------   ---------------------------
vec2      fragCoord                 vec2   gl_FragCoord
vec4      fragColor                 vec4   gl_FragColor
vec3      iResolution               vec2   RENDERSIZE (iResolution.xy only)
float     iTime                     float  TIME
float     iTimeDelta                float  TIMEDELTA
int       iFrame                    int    FRAMEINDEX
float     iChannelTime[4]                  note #1
vec3      iChannelResolution[4].xy  vec2   IMG_SIZE(image imageName)     
vec4      iMouse                           note #2
samplerXX iChannel0..3              vec4   IMG_PIXEL(image imageName, vec2 pixelCoord)
vec4      iDate                     vec4   DATE
float     iSampleRate                      note #1

notes:
 1.   No known equivalent
 2.   Often simulated using a point2D for iMouse.xy and an event for iMouse.z or iMouse.w
 3.   iChannel0..3 equivalents can be defined using the IMPORTED key in the JSON dict.
 4.   Sound, and an image for filter shaders are accessed using an image named "inputImage"
      defined in the JSON dict.
 5.   


vec3      iResolution;           // viewport resolution (in pixels)
float     iTime;                 // shader playback time (in seconds)
float     iTimeDelta;            // render time (in seconds)
int       iFrame;                // shader playback frame
float     iChannelTime[4];       // channel playback time (in seconds)
vec3      iChannelResolution[4]; // channel resolution (in pixels)
vec4      iMouse;                // mouse pixel coords. xy: current (if MLB down), zw: click
samplerXX iChannel0..3;          // input channel. XX = 2D/Cube
vec4      iDate;                 // (year, month, day, time in seconds)
float     iSampleRate;           // sound sample rate (i.e., 44100)

*/
void main()
	{
	gl_FragColor = IMG_THIS_PIXEL(inputImage);
	}