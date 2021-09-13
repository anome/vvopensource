/*{
	"CREDIT": "by kazikpogoda",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	]
}*/


float bassHits = syn_BassHits;
float highHits = syn_HighHits;
float vuTimeUncorrected = syn_Time;
float bassTimeUncorrected = syn_BassTime;
float randomizerBeat = syn_RandomOnBeat;
float beatTimeUncorrected = syn_BeatTime;
float onBeat = syn_OnBeat;
float switchOnBeat = syn_ToggleOnBeat;
float highsAccum = syn_HighLevel;
float bassAccum = syn_BassLevel;
float vuFadeToBlack = syn_FadeInOut;

vec3 iResolution = vec3(RENDERSIZE.x, RENDERSIZE.y, 0.0);
float iGlobalTime = TIME*1.0;
int iFrame = int(TIME*60.0);
float time = TIME;
vec2 resolution = RENDERSIZE;
float vuTime = vuTimeUncorrected*6*0.01;
float bassTime = vuTimeUncorrected*6*0.01;
float beatTime = beatTimeUncorrected*6;
// vec2 lightPos = vec2(resolution.x*(0.5+0.5*sin(vuTime*2*PI*0.5)),resolution.y*(0.5+0.5*cos(vuTime*2*PI*0.67)));

// vec4 dataPass(){
//   vec4 oldTimeData = texelFetch(pass0, ivec2(0,0), 0);
//   if (TIME <= 0.1){
//       return vec4(0.0);
//   }
//   float newMidiInputX = x_position;
//   float oldX = oldTimeData.r;
//   float newX = oldX-(oldX-newMidiInputX)*0.1;

//   float newMidiInputY = y_position;
//   float oldY = oldTimeData.g;
//   float newY = oldY-(oldY-newMidiInputY)*0.1;

//   vec4 retData = vec4(newX, newY, abs(newMidiInputX-oldX)+abs(newMidiInputY-oldY), 0.0);
//   return retData;
// }

// ****************** PASS 0 ***********************
// main reaction-diffusion loop

// actually the diffusion is realized as a separated two-pass Gaussian blur kernel and is stored in buffer C

#define pi2_inv 0.159154943091895335768883763372

vec2 complex_mul(vec2 factorA, vec2 factorB){
  return vec2( factorA.x*factorB.x - factorA.y*factorB.y, factorA.x*factorB.y + factorA.y*factorB.x);
}

float sigmoid(float x) {
  return 2./(1. + exp2(-x)) - 1.;
}

float conetip(vec2 uv, vec2 pos, float size, float min)
{
  vec2 aspect = vec2(1.,iResolution.y/iResolution.x);
  return max( min, 1. - length((uv - pos) * aspect / size) );
}

float warpf1lter(vec2 uv, vec2 pos, float size, float ramp)
{
  return 0.5 + sigmoid( conetip(uv, pos, size, -16.) * ramp) * 0.5;
}

vec2 vortex_warp(vec2 uv, vec2 pos, float size, float ramp, vec2 rot)
{
  vec2 aspect = vec2(1.,iResolution.y/iResolution.x);

  vec2 pos_correct = 0.5 + (pos - 0.5);
  vec2 rot_uv = pos_correct + complex_mul((uv - pos_correct)*aspect, rot)/aspect;
  float f1lter = warpf1lter(uv, pos_correct, size, ramp);
  return mix(uv, rot_uv, f1lter);
}

vec2 vortex_pair_warp(vec2 uv, vec2 pos, vec2 vel, out float touched){
  vec2 aspect = vec2(1.,iResolution.y/iResolution.x);
  float ramp = 5.;

  float d = 0.2;

  float l = length(vel);
  vec2 p1 = pos;
  vec2 p2 = pos;

  if(l > 0.){
    vec2 normal = normalize(vel.yx * vec2(-1., 1.))/aspect;
    p1 = pos - normal * d / 2.;
    p2 = pos + normal * d / 2.;
  }

  float w = l / d * 2.;

  // two overlapping rotations that would annihilate when they were not displaced.
  vec2 circle1 = vortex_warp(uv, p1, d, ramp, vec2(cos(w),sin(w)));
  vec2 circle2 = vortex_warp(uv, p2, d, ramp, vec2(cos(-w),sin(-w)));

  touched = abs(pow(distance((circle1+circle2)/2.0, uv),2.0)*1000);
  return (circle1 + circle2) / 2.;
}

void mainImage0(out vec4 fragColor, in vec2 fragCoord)
{
  vec2 uv = fragCoord.xy / iResolution.xy;
  vec2 pixelSize = 1. / iResolution.xy;

  vec2 aspect = vec2(1.,iResolution.y/iResolution.x);
  vec2 flowVec = normalize(vec2(sin(PI*2*iGlobalTime), cos(PI*2*iGlobalTime)));

  vec2 brushPosition = manual_position;
  if (auto_position >= 0.5){
    brushPosition = vec2((sin(syn_Time*2*PI*0.025)),(cos(syn_HighTime*2*PI*0.1)));
  }
  flowVec = brushPosition;


  float decay = 0.95;

  float touchOld = texture(buffA, _uv).g*decay;
  float touchNew;

  float twitcher = (syn_HighHits*auto_impulse+manual_impulse*0.7)*impulse_power;


  if (motion_mode < 0.5){
    uv = vortex_pair_warp(uv, (flowVec+1.0)*0.5, normalize(flowVec)*(twitcher)*0.025, touchNew);
  }

  vec4 blur1 = texture(buffC, uv);

  vec4 noise = vec4(_noise(_uv*1000.0),_noise(_uv*1000.0),_noise(_uv*1000.0),_noise(_uv*1000.0));

  // get the gradients from the blurred image
  vec2 d = pixelSize*4.;
  vec4 dx = (texture(buffC, fract(uv + vec2(1,0)*d)) - texture(buffC, fract(uv - vec2(1,0)*d))) * 0.5;
  vec4 dy = (texture(buffC, fract(uv + vec2(0,1)*d)) - texture(buffC, fract(uv - vec2(0,1)*d))) * 0.5;
  // if (flowing == 1.0){
  //   dy.x += distance(uv*aspect, vec2(0.5,0.0));
  // }

  vec3 logoCol = vec3(0.0);
  if (_exists(syn_UserImage)){
    logoCol = _loadUserImageAsMask().rgb;
  }

  float distFunc = 0.0;
  vec2 flowOut;
  if (motion_mode >= 0.5){
    twitcher = (pow(syn_BassLevel,2.0)*0.75*auto_impulse+manual_impulse)*impulse_power;
    vec2 posCent = _uvc-brushPosition*0.5*vec2(RENDERSIZE.x/RENDERSIZE.y,1.0);
    float mask = smoothstep(twitcher*0.5,twitcher*0.5+0.5,length(posCent));
    distFunc = 1.0-mask;
    flowOut = normalize(posCent)*0.6*distFunc*(-0.25+twitcher)*3.0;
    touchNew += pow(distFunc,10.0)*twitcher*0.2;
    dx.x -= flowOut.x;
    dy.x -= flowOut.y;
  }

  dx.x -= normalize(_uvc).x*0.1*clamp(in_out, -30.0, 10.0);
  dy.x -= normalize(_uvc).y*0.1*clamp(in_out, -30.0, 10.0);

  if (image_block < 0.5){
    distFunc = distFunc-length(logoCol)*10.5;
  }

  // if (invertLogo >= 0.5){
  //   distFunc = 1.0-logoCol;
  // }

  vec2 uv_red = uv + vec2(dx.x, dy.x)*pixelSize*8.0*(1.0+shock_forward*8.0); // add some diffusive expansion

  uv_red = mix(uv_red, uv - vec2(dx.x, dy.x)*pixelSize*8.0, pow(shock_back,2.0)); // add some diffusive expansion


  float new_red = texture(buffA, fract(uv_red)).x + (noise.x*0.5) * 0.0025 - 0.002; // stochastic dec
  new_red -= (texture(buffC, fract(uv_red + (noise.xy-0.5)*pixelSize)).x -
              texture(buffA, fract(uv_red + (noise.xy-0.5)*pixelSize))).x * 0.047  ; // reaction-diffusion

  // float cReg4 = 1.0;
  // if (cReg4 > 0.01){
    new_red -= texture(buffC, _uv).r*0.001*syn_BassPresence;
  // }

  if (distFunc>0.999){
    new_red = new_red+bassAccum*0.01;
  }

  // touchNew = 0.0;
  if(FRAMECOUNT<=10){
    fragColor = noise;
  }
  else{
    fragColor.r = clamp(new_red, 0., 1.);
    fragColor.g = touchOld+touchNew;
  }

  if ((image_block<0.5)&&(length(logoCol)*1.5>1.0)){
    fragColor.r -= 0.009;
  }

  if (image_block>=0.5){
    if (logoCol.r>0.5){
      fragColor.r = 1.0*syn_BassLevel;
    }
  }



  //    fragColor = noise; // need a restart?
}

// *************** PASS 1 *****************
// horizontal Gaussian blur pass

void horizontalBlur(out vec4 fragColor, in vec2 fragCoord) {
  vec2 pixelSize = 1./ iResolution.xy;
  vec2 uv = fragCoord.xy * pixelSize;

  float h = pixelSize.x;
  vec4 sum = vec4(0.0);
  sum += texture(buffA, fract(vec2(uv.x - 4.0*h, uv.y)) ) * 0.05;
  sum += texture(buffA, fract(vec2(uv.x - 3.0*h, uv.y)) ) * 0.09;
  sum += texture(buffA, fract(vec2(uv.x - 2.0*h, uv.y)) ) * 0.12;
  sum += texture(buffA, fract(vec2(uv.x - 1.0*h, uv.y)) ) * 0.15;
  sum += texture(buffA, fract(vec2(uv.x + 0.0*h, uv.y)) ) * 0.16;
  sum += texture(buffA, fract(vec2(uv.x + 1.0*h, uv.y)) ) * 0.15;
  sum += texture(buffA, fract(vec2(uv.x + 2.0*h, uv.y)) ) * 0.12;
  sum += texture(buffA, fract(vec2(uv.x + 3.0*h, uv.y)) ) * 0.09;
  sum += texture(buffA, fract(vec2(uv.x + 4.0*h, uv.y)) ) * 0.05;

  fragColor.xyz = sum.xyz/0.98; // normalize
  fragColor.a = 1.;
}



// *************** PASS 2 *****************
// vertical Gaussian blur pass
void verticalBlur(out vec4 fragColor, in vec2 fragCoord){
  vec2 pixelSize = 1./ iResolution.xy;
  vec2 uv = fragCoord.xy * pixelSize;

  float v = pixelSize.y;
  vec4 sum = vec4(0.0);
  sum += texture(buffB, fract(vec2(uv.x, uv.y - 4.0*v)) ) * 0.05;
  sum += texture(buffB, fract(vec2(uv.x, uv.y - 3.0*v)) ) * 0.09;
  sum += texture(buffB, fract(vec2(uv.x, uv.y - 2.0*v)) ) * 0.12;
  sum += texture(buffB, fract(vec2(uv.x, uv.y - 1.0*v)) ) * 0.15;
  sum += texture(buffB, fract(vec2(uv.x, uv.y + 0.0*v)) ) * 0.16;
  sum += texture(buffB, fract(vec2(uv.x, uv.y + 1.0*v)) ) * 0.15;
  sum += texture(buffB, fract(vec2(uv.x, uv.y + 2.0*v)) ) * 0.12;
  sum += texture(buffB, fract(vec2(uv.x, uv.y + 3.0*v)) ) * 0.09;
  sum += texture(buffB, fract(vec2(uv.x, uv.y + 4.0*v)) ) * 0.05;

  fragColor.xyz = sum.xyz/0.98; // normalize
  fragColor.a = 1.;
}


// *************** PASS 4 *****************
void mainImage4( out vec4 fragColor, in vec2 fragCoord ){
  vec2 uv = fragCoord.xy / iResolution.xy;
  vec2 pixelSize = 1. / iResolution.xy;
  vec2 aspect = vec2(1.,iResolution.y/iResolution.x);

  vec4 noise = vec4(_noise(_uv*1000.0),_noise(_uv*1000.0),_noise(_uv*1000.0),_noise(_uv*1000.0));

  vec2 lightSize=vec2(4.0);
  // lightSize /= pow(highsAccum,5.0)*2.0;

  // get the gradients from the blurred image
  vec2 d = pixelSize*2.0;
  vec4 dx = (texture(buffC, uv + vec2(1,0)*d) - texture(buffC, uv - vec2(1,0)*d))*0.5;
  vec4 dy = (texture(buffC, uv + vec2(0,1)*d) - texture(buffC, uv - vec2(0,1)*d))*0.5;

  // // add the pixel gradients
  d = pixelSize;
  dx += texture(buffA, uv + vec2(1,0)*d) - texture(buffA, uv - vec2(1,0)*d);
  dy += texture(buffA, uv + vec2(0,1)*d) - texture(buffA, uv - vec2(0,1)*d);

  vec2 displacement = vec2(dx.x,dy.x)*lightSize; // using only the red gradient as displacement vector
  // float light = pow(max(1.-distance(0.5+(uv-0.5)*aspect*lightSize + displacement*5.0,vec2(0.5)),0.),2.);
  float light1 = 2.-distance(0.5+(uv-vec2(0.5,1.0))*aspect*lightSize + displacement*5.0,vec2(0.5,1.5));
  light1 = max(light1,0.0);

  float light2 = 2.-distance(0.5+(uv-vec2(0.5,-1.0))*aspect*lightSize + displacement*5.0,vec2(0.5,1.5));
  light2 = max(light2,0.0);

  vec3 baseCol = vec3(0.0);
  vec3 newSpawnCol = vec3(0.0,-1.0,-1.0);
  vec3 topLightCol = vec3(0.5,0.6,0.7)*2.5;
  vec3 botLightCol = vec3(0.5,0.6,0.7)*2.5;
  vec3 motionCol = vec3(-1.1,0.2,0.2)*4.0;

  // float cReg0= 1.0;
  // float cReg1= 0.0;
  // float cReg2= 0.0;
  // float cReg3= 0.0;
  // float cReg4= 0.0;


  // *** Color Regime 1 ***
  baseCol = mix(baseCol, vec3(0.45,0.75,1.0), cReg1);
  newSpawnCol = mix(newSpawnCol, vec3(0.45,0.9,1.0), cReg1);
  topLightCol = mix(topLightCol, vec3(0.25,0.55,1.0), cReg1);
  botLightCol = mix(botLightCol, vec3(0.25,0.55,1.0), cReg1);
  motionCol = mix(motionCol, vec3(0.6,0.6,0.08)*3.0, cReg1);
  // motionCol = mix(motionCol, vec3(0.3,0.4,0.22), cReg1);

  // *** Color Regime 2 ***
  baseCol = mix(baseCol, vec3(1.2,0.7,0.8)*3.0, cReg2);
  newSpawnCol = mix(newSpawnCol, vec3(1.0,0.7,0.8)*2.0, cReg2);
  topLightCol = mix(topLightCol, vec3(1.0,1.0,1.0), cReg2);
  botLightCol = mix(botLightCol, vec3(1.0,1.0,1.0), cReg2);
  motionCol = mix(motionCol, vec3(1.0,0.95,0.00)*3.0, cReg2);

  // *** Color Regime 3 ***
  baseCol = mix(baseCol, vec3(0.5,0.5,0.5), cReg3);
  newSpawnCol = mix(newSpawnCol, vec3(0.8,0.5,0.1), cReg3);
  topLightCol = mix(topLightCol, vec3(1.0,0.0,0.0), cReg3);
  botLightCol = mix(botLightCol, vec3(1.0,0.4,0.1), cReg3);
  motionCol = mix(motionCol, vec3(0.2,0.2,0.3)*2.0, cReg3);

  // *** Color Regime 4 ***
  baseCol = mix(baseCol, vec3(1.0,0.0,1.0), cReg4);
  newSpawnCol = mix(newSpawnCol, vec3(0.2,0.0,0.8), cReg4);
  topLightCol = mix(topLightCol, vec3(0.3,0.7,1.0)*2.0, cReg4);
  botLightCol = mix(botLightCol, vec3(0.9,0.8,0.2)*2.0, cReg4);
  motionCol = mix(motionCol, vec3(1.0,0.77,0.0)*1.0, cReg4);

  baseCol *= 0.5+bassAccum*0.5;

  // recolor the red channel
  vec3 surface = vec3(texture(buffA,uv+vec2(dx.x,dy.x)*pixelSize*10.0).r);
  vec3 edges = vec3(pow(texture(buffA,uv+vec2(dx.x,dy.x)).r,10.0));

  vec3 lightCol = topLightCol*light1 + botLightCol*light2;
  lightCol *= 0.5+highsAccum;

  // and add the light map
  vec3 finalColor = surface*baseCol + lightCol;

  float darkener = (1.-texture(buffA,uv+vec2(dx.x,dy.x)*pixelSize*lightSize.x).x);
  finalColor -= darkener;

  float newSpawn = clamp(texture(buffA, _uv).r,0.0,1.0);
  finalColor = mix(finalColor, edges*newSpawnCol, newSpawn);

  float recentlyTouched = texture(buffA, _uv).g;
  finalColor = mix(finalColor, motionCol*sqrt(edges)*2.0, recentlyTouched*0.4);

  if (color_inv>0.5){
    finalColor = pow(finalColor, vec3(1.000001));
  }

  fragColor = vec4(finalColor, 1.0);
}



vec2 getNormPos() {
  return vec2(-1+2*(gl_FragCoord.x/resolution.x),-1+2*(gl_FragCoord.y/resolution.y));
}
vec2 getNormPosBotLeft() {
  return vec2(gl_FragCoord.xy/resolution.xy);
}

vec4 renderMain(void)
{
  if (PASSINDEX == 0.0){
    //buffA
    vec4 fragColor;
    mainImage0(fragColor, gl_FragCoord.xy);
    return fragColor;
  }
  else if (PASSINDEX == 1.0){
    //buffB
    vec4 fragColor;
    horizontalBlur(fragColor, gl_FragCoord.xy);
    return fragColor;
  }
  else if (PASSINDEX == 2.0){
    //buffC
    vec4 fragColor;
    verticalBlur(fragColor, gl_FragCoord.xy);
    return fragColor;
  }
  else if (PASSINDEX == 3.0){
    //Image
    vec4 fragColor;
    mainImage4(fragColor, gl_FragCoord.xy);
    return fragColor;
  }
}

void main() {
	FragNormCoord.xy);
}



