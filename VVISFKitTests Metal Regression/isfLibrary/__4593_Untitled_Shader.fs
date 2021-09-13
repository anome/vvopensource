#version 330
/*{
	"ISFVSN": "2",
	"INPUTS": [
  {
  "NAME": "inputImage",
  "TYPE": "image"
  },
  {
  "NAME": "moveX",
  "TYPE": "float",
  "MIN": -2.0,
  "MAX": 2.0,
  "DEFAULT": 0.5
  },
  {
  "NAME": "moveY",
  "TYPE": "float",
  "MIN": -2.0,
  "MAX": 2.0,
  "DEFAULT": 0.5
  },
  {
  "NAME": "zoom",
  "TYPE": "float",
  "MIN": 0.00001,
  "MAX": 0.7,
  "DEFAULT": 0.01
  },
  {
  "NAME": "control1",
  "TYPE": "float",
  "MIN": -2.0,
  "MAX": 2.0,
  "DEFAULT": -0.8
  },
  {
  "NAME": "control2",
  "TYPE": "float",
  "MIN": -2.0,
  "MAX": 2.0,
  "DEFAULT": 0.9
  },
  {
  "NAME": "control3",
  "TYPE": "float",
  "MIN": -2.0,
  "MAX": 2.0,
  "DEFAULT": -0.7
  },
  {
  "NAME": "control4",
  "TYPE": "float",
  "MIN": -2.0,
  "MAX": 2.0,
  "DEFAULT": 0.5
  },
  {
  "NAME": "controlScale",
  "TYPE": "float",
  "MIN": 0.000001,
  "MAX": 1.0,
  "DEFAULT": 0.001
  }
	]
  }*/
  

vec2 ZoomAndTranslateCoord(vec2 fragCoord, vec2 offset, float zoomFactor)
{//Zoom UV Around the center of current screen position;
	vec2 halfz=vec2(0.5)*RENDERSIZE.xy+offset;
	return (fragCoord+offset-halfz)*zoomFactor+halfz;
}

// set samples from 1-4 for quality selection
#define SAMPLES 4

vec2 complexMult(vec2 a, vec2 b) {
	return vec2(a.x*b.x - a.y*b.y, a.x*b.y + a.y*b.x);
}

float testMandelbrot(vec2 coord) {
    // turn this up to 5000 or so if you have a good gpu
    // for better details but less vibrant color in extreme zoom
    const int iterations = 912;
    vec2 center=vec2(0.5,0.5);
	vec2 testPoint = vec2(0,0);
    float reversedZoom=1./zoom;
    int iterationsDivider=iterations/40;
	for (int i = 0; i < iterations; i++){
	    if (i<iterationsDivider){
            float controlGrade=mix(0.,control1, float(i)/float(iterationsDivider));
            testPoint+=(center+vec2(sin(TIME),cos(TIME))*reversedZoom-testPoint)*controlGrade*controlScale;
        }
        else if (i<iterationsDivider*2){
            float controlGrade=mix(control1,control2, float(i-iterationsDivider)/float(iterationsDivider));
            testPoint-=(center+vec2(cos(TIME),sin(TIME))*reversedZoom-testPoint)*controlGrade*controlScale*10.;
        }
        else if (i<iterationsDivider*3){
            float controlGrade=mix(control2,control3, (float(i)-float(iterationsDivider)*2.)/float(iterationsDivider));
            testPoint+=(center+vec2(sin(TIME),cos(TIME))*reversedZoom-testPoint)*controlGrade*controlScale*50.;
        }
        else if (i<iterationsDivider*4){
            float controlGrade=mix(control3,control4, float(i-iterationsDivider*3)/float(iterationsDivider));
            testPoint+=(center+vec2(cos(TIME),sin(TIME))*reversedZoom-testPoint)*controlGrade*controlScale*100.;
        }
        testPoint = complexMult(testPoint,testPoint) + coord;
        
        float ndot = dot(testPoint,testPoint);
		if (ndot > 45678.0) {
            float sl = float(i) - log2(log2(ndot))+4.0;
			return sl/float(iterations);
		}
	}
	return 0.0;
}

vec4 mapColor(float mcol) {
    return vec4(0.5 + 0.5*cos(2.7+mcol*30.0 + vec3(0.0,.6,1.0)),1.0);
}


void main() {
    vec2 fragCoord = gl_FragCoord; // (gl_FragCoord | isf_FragNormCoord) (500x350 | 1.0x0.6)
    vec2 mouse = vec2(moveX,moveY);
	fragCoord=ZoomAndTranslateCoord(fragCoord, mouse, zoom);
    const vec2 zoomP = vec2(-.7457117,.186142);
    const float zoomTime = 100.0;
    vec2 iResolution=RENDERSIZE.xy;

    float tTime = 9.0 + abs(mod(TIME+zoomTime,zoomTime*2.0)-zoomTime);
    //tTime=1; //Stop Time (Temp)
    vec2 aspect = vec2(1,iResolution.y/iResolution.x);
    
    float offsetsD = .35*zoom;
    vec4 outs = vec4(0.0);
    vec2 offsets[4] = vec2[](
        vec2(-offsetsD,-offsetsD),
        vec2(offsetsD,offsetsD),
        vec2(-offsetsD,offsetsD),
        vec2(offsetsD,-offsetsD)
    );
    for(int i = 0; i < SAMPLES; i++) {        
        vec2 fragment = (fragCoord+offsets[i])/iResolution;    
        vec2 fragCoord = aspect * (zoomP + tTime * (fragment - mouse));
        outs += mapColor(testMandelbrot(fragCoord));
    }
	gl_FragColor = outs/float(SAMPLES);
}

