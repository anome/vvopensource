
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
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
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

uniform vec2 u_resolution;
uniform vec2 u_mouse;

// util
// noise
vec2 random2(vec2 st){
    st = vec2( dot(st,vec2(127.1,311.7)),
              dot(st,vec2(269.5,183.3)) );
    return -1.0 + 2.0*fract(sin(st)*43758.5453123);
}

// Gradient Noise by Inigo Quilez - iq/2013
// https://www.shadertoy.com/view/XdXGW8
float noise(vec2 st) {
    vec2 i = floor(st);
    vec2 f = fract(st);

    vec2 u = f*f*(3.0-2.0*f);

    return mix( mix( dot( random2(i + vec2(0.0,0.0) ), f - vec2(0.0,0.0) ),
                     dot( random2(i + vec2(1.0,0.0) ), f - vec2(1.0,0.0) ), u.x),
                mix( dot( random2(i + vec2(0.0,1.0) ), f - vec2(0.0,1.0) ),
                     dot( random2(i + vec2(1.0,1.0) ), f - vec2(1.0,1.0) ), u.x), u.y);
}

vec4 noise1(vec2 uv){
    vec3 color = vec3(0.0);

    vec2 pos = vec2(uv*40.0);

    color = vec3( noise(pos)*.5+.5 );

    return vec4(color,1.0);
    
}



//http://roy.red/slitscan-.html
vec3 hash3( vec2 p )
{
    vec3 q = vec3( dot(p,vec2(127.1,311.7)), 
				   dot(p,vec2(269.5,183.3)), 
				   dot(p,vec2(419.2,371.9)) );
	return fract(sin(q)*43758.5453);
}

float iqnoise( in vec2 x, float u, float v )
{
    vec2 p = floor(x);
    vec2 f = fract(x);
		
	float k = 1.0+63.0*pow(1.0-v,6.0);
	
	float va = 0.0;
	float wt = 0.0;
    for( int j=-2; j<=2; j++ )
    for( int i=-2; i<=2; i++ )
    {
        vec2 g = vec2( float(i),float(j) );
		vec3 o = hash3( p + g )*vec3(u,u,1.0);
		vec2 r = g - f + o.xy;
		float d = dot(r,r);
		float ww = pow( 1.0-smoothstep(0.0,1.414,sqrt(d)), k );
		va += o.z*ww;
		wt += ww;
    }
	
    return va/wt;
}

vec3 colorSlit(vec2 pt) {
    pt = 1.2*pt;
    float rInv = 1./length(pt);
    pt = pt * rInv - vec2(rInv+2.*mod(TIME,6000.),0.0);
    vec3 color = vec3(0.659,0.772,1.000);
    return color*vec3(iqnoise(5.*pt,1.,1.)+0.240*rInv);
}

vec3 colorSlit2(vec2 pt) {
    pt = pt / length(pt);
    return vec3(iqnoise(5.*pt,1.,1.));
}

vec3 colorSlit3(vec2 pt) {
    float rInv = 1./length(pt);
    pt = pt * rInv - vec2(rInv,0.0);
    return vec3(iqnoise(5.*pt,1.,1.));
}

vec3 colorSlit4(vec2 pt) {
    pt = 1.2*pt;
    float rInv = 1./length(pt);
    pt = pt * rInv - vec2(rInv+2.*mod(TIME,6000.),0.0);
    vec3 color = vec3(0.659,0.772,1.000);
    return color*vec3(iqnoise(5.*pt,1.,1.)+0.240*rInv);
}

float dist(vec2 pt) {
    //return min(abs(pt.x+pt.y),abs(pt.x-pt.y))+0.001;
    //return abs(pt.x+pt.y);
    //return abs(pt.x)+abs(pt.y);
    //return max(abs(pt.x),abs(pt.y));
    return abs(pt.x);
}
vec3 colorSlit5(vec2 pt, float param) {
    float rInv = 1./dist(pt);
    // Uncomment below to show contours
    // return vec3(fract(rInv));
    //pt = pt * rInv - vec2(rInv-mod(u_time*0.1,499.392),0.0);
    //return vec3(iqnoise(param*pt,1.,1.)+1.280*rInv)*1.0;
    pt = pt * rInv - vec2(rInv+mod(TIME,5998.824),0.0);
    return vec3(iqnoise(param*pt,1.,1.)+-0.166*rInv);
}

vec4 finalSlit(){
    vec2 st = gl_FragCoord.xy/u_resolution.xy - vec2(0.5);

    vec3 colorY = vec3(0.);
    vec3 colorB = vec3(0.);

   // color = vec3(st.x,st.y,abs(sin(u_time)));

    colorY = colorSlit5(st,5.070)*vec3(0.960,0.924,0.007);
    colorB = colorSlit5(st,3.912)*vec3(0.000,0.011,0.960);

    return vec4(mix(colorY,colorB,0.),1.0);
}
vec4 auraSlit(vec2 uv){

    vec3 colorY = vec3(0.);
    vec3 colorB = vec3(0.);

   // color = vec3(st.x,st.y,abs(sin(u_time)));

    colorY = colorSlit5(uv,5.398)*vec3(0.960,0.924,0.007);
    colorB = colorSlit5(uv,4.968)*vec3(0.000,0.011,0.960);

    return vec4(mix(colorY,colorB,0.5),1.0);
}



//https://www.shadertoy.com/view/XljBzz
//higher is longer bleeds on x
#define bleed 1.456
//higher to regain a bit of detail
#define sharpness 6.0
//higher is less bleeding
#define exp_k 3.0 
float bleedcurve (float x)
{
    return (sin(x*3.1415) + exp(-exp_k*2.*x))/3.;
    //could also just retun the exp
}

vec4 bleeding()
{
	vec2 st = gl_FragCoord.xy / u_resolution.xy - vec2(0.5);

    const float iterations = 5.;
    vec4 aura = auraSlit(st);
    float one_x = 1./u_resolution.x;
    float img = auraSlit(st).y * sharpness; //channel
    float norm = sharpness;    
    
    for (float i = 0.; i<iterations; i+=1.)
    {
		img += auraSlit( st + vec2(i*one_x*bleed, 0.)).y //channel
               * bleedcurve (i/iterations)
			   + auraSlit( st + vec2(-i*one_x*bleed, 0.)).y //channel
               * bleedcurve (i/iterations);
        
        norm += bleedcurve (i/iterations) * 2. 
                + (noise1( st+sin(TIME*9.1)).x - 1. /*not -.5 because we use pow below*/) * 0.05; //adds grain
        
        
    }
    
    img /= norm;
	img = pow(img, 1.1);
    
    return mix(vec4(mix(img, auraSlit(st).y /*channel*/, floor(st.x))) , auraSlit(st),0.5);


}
vec4 bleeding1( vec2 st)
{
//	vec2 st = gl_FragCoord.xy / u_resolution.xy - vec2(0.5);

    const float iterations = 5.;
    vec4 aura = auraSlit(st);
    float one_x = 1./u_resolution.x;
    float img = auraSlit(st).y * sharpness; //channel
    float norm = sharpness;    
    
    for (float i = 0.; i<iterations; i+=1.)
    {
		img += auraSlit( st + vec2(i*one_x*bleed, 0.)).y //channel
               * bleedcurve (i/iterations)
			   + auraSlit( st + vec2(-i*one_x*bleed, 0.)).y //channel
               * bleedcurve (i/iterations);
        
        norm += bleedcurve (i/iterations) * 2. 
                + (noise1( st+sin(TIME*9.1)).x - 1. /*not -.5 because we use pow below*/) * 0.05; //adds grain
        
        
    }
    
    img /= norm;
	img = pow(img, 1.1);
    
    return mix(vec4(mix(img, auraSlit(st).y /*channel*/, floor(st.x))) , auraSlit(st),0.5);


}



// glitch
float sat( float t ) {
	return clamp( t, -0.5, .5 );
}

vec2 sat( vec2 t ) {
	return clamp( t,-0.5, .5 );
}

//remaps inteval [a;b] to [0;1]
float remap  ( float t, float a, float b ) {
	return sat( (t - a) / (b - a) );
}

//note: /\ t=[0;0.5;1], y=[0;1;0]
float linterp( float t ) {
	return sat( 1.0 - abs( 2.0*t - 1.0 ) );
}

vec3 spectrum_offset( float t ) {
    float t0 = 3.0 * t - 1.5;
	return clamp( vec3( -t0, 1.0-abs(t0), t0), 0.0, 1.0);
    /*
	vec3 ret;
	float lo = step(t,0.5);
	float hi = 1.0-lo;
	float w = linterp( remap( t, 1.0/6.0, 5.0/6.0 ) );
	float neg_w = 1.0-w;
	ret = vec3(lo,1.0,hi) * vec3(neg_w, w, neg_w);
	return pow( ret, vec3(1.0/2.2) );
*/
}

//note: [0;1]
float rand( vec2 n ) {
  return fract(sin(dot(n.xy, vec2(12.9898, 78.233)))* 43758.5453);
}

//note: [-1;1]
float srand( vec2 n ) {
	return rand(n) * 2.0 - 1.0;
}

float mytrunc( float x, float num_levels )
{
	return floor(x*num_levels) / num_levels;
}
vec2 mytrunc( vec2 x, float num_levels )
{
	return floor(x*num_levels) / num_levels;
}

vec4 glitch1(){
    float aspect = u_resolution.x / u_resolution.y;
	vec2 uv = gl_FragCoord.xy / u_resolution.xy - vec2(0.5);
	vec2 st = gl_FragCoord.xy / u_resolution.xy - vec2(0.5);

    //uv.y = uv.y *-1.;
	
	float time = mod(TIME, 2.512); // + modelmat[0].x + modelmat[0].z;

	float GLITCH = 1.0 ;
	
    //float rdist = length( (uv - vec2(0.5,0.5))*vec2(aspect, 1.0) )/1.4;
    //GLITCH *= rdist;
    
	float gnm = sat( GLITCH );
	float rnd0 = rand( mytrunc( vec2(time, time), 6.0 ) );
	float r0 = sat((1.0-gnm)*0.7 + rnd0);
	float rnd1 = rand( vec2(mytrunc( uv.x, 10.0*r0 ), time) ); //horz
	//float r1 = 1.0f - sat( (1.0f-gnm)*0.5f + rnd1 );
	float r1 = 0.5 - 0.5 * gnm + rnd1;
	r1 = 1.0 - max( 0.0, ((r1<1.0) ? r1 : 0.9999999) ); //note: weird ass bug on old drivers
	float rnd2 = rand( vec2(mytrunc( uv.y, 40.0*r1 ), time) ); //vert
	float r2 = sat( rnd2 );

	float rnd3 = rand( vec2(mytrunc( uv.y, 10.0*r0 ), time) );
	float r3 = (1.0-sat(rnd3+0.8)) - 0.1;

	float pxrnd = rand( uv + time*1.1 );

	float ofs = 0.05 * r2 * GLITCH * ( rnd0 > 0.5 ? 1.0 : -1.0 );
	ofs += 0.5 * pxrnd * ofs;

	uv.y += 0.1 * r3 * GLITCH;

    const int NUM_SAMPLES = 3;
    const float RCP_NUM_SAMPLES_F = 1.0 / float(NUM_SAMPLES);
    
	vec4 sum = vec4(0.0);
	vec3 wsum = vec3(0.0);
	for( int i=0; i<NUM_SAMPLES; ++i )
	{
		float t = float(i) * RCP_NUM_SAMPLES_F;
		uv.x = sat( uv.x + ofs * t );
		vec4 samplecol =  bleeding1(uv);
		vec3 s = spectrum_offset( t );
		samplecol.rgb = samplecol.rgb * s;
		sum += samplecol;
		wsum += s;
	}
	sum.rgb /= wsum;
	sum.a *= RCP_NUM_SAMPLES_F;

    //fragColor = vec4( sum.bbb, 1.0 ); return;
    vec4 color = vec4(0);
	color.a = sum.a;
	color.rgb = sum.rgb;
    
    return color;
}




// glitch 2
//2D (returns 0 - 1)
float random2d(vec2 n) { 
    return fract(sin(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

float randomRange (in vec2 seed, in float min, in float max) {
		return min + random2d(seed) * (max - min);
}

// return 1 if v inside 1d range
float insideRange(float v, float bottom, float top) {
   return step(bottom, v) - step(top, v);
}

//inputs
const float AMT = 0.2; //0 - 1 glitch amount
float SPEED = 0.6; //0 - 1 speed
   
vec4 glitch2()
{
    
    float time = floor(TIME * SPEED * 60.0);    
    
    vec2 uv = gl_FragCoord.xy / u_resolution.xy;
	vec2 st = gl_FragCoord.xy / u_resolution.xy - vec2(0.5);
    
    
    //copy orig
    vec3 outCol = auraSlit(uv).rgb;
    
    //randomly offset slices horizontally
    float maxOffset = AMT/2.0;
    for (float i = 0.0; i < 2.0 * AMT; i += 1.0) {
        float sliceY = random2d(vec2(time , 2345.0 + float(i)));
        float sliceH = random2d(vec2(time , 9035.0 + float(i))) * 0.25;
        float hOffset = randomRange(vec2(time , 9625.0 + float(i)), -maxOffset, maxOffset);
        vec2 uvOff = uv;
        uvOff.x += hOffset;
        if (insideRange(uv.y, sliceY, fract(sliceY+sliceH)) == 1.0 ){
        	outCol = auraSlit(uvOff).rgb;
        }
    }
    
    //do slight offset on one entire channel
    float maxColOffset = AMT/2.;
    float rnd = random2d(vec2(time , 9545.0));
    vec2 colOffset = vec2(randomRange(vec2(time , 9545.0),-maxColOffset,maxColOffset), 
                       randomRange(vec2(time , 7205.0),-maxColOffset,maxColOffset));
    if (rnd < 0.00001){
        outCol.r = auraSlit( uv + colOffset*0.1).r;
        
    }else if (rnd < 0.01){
        outCol.g = auraSlit( uv ).g;
        
    } else{
        outCol.b = auraSlit( uv).b;  
    }
       
	return vec4(outCol,1.0);
}

highp float rand3(vec2 co)
{
    highp float a = 12.9898;
    highp float b = 78.233;
    highp float c = 43758.5453;
    highp float dt= dot(co.xy ,vec2(a,b));
    highp float sn= mod(dt,3.14);
    return fract(sin(sn) * c);
}

vec4 glitch3_windfilter(){
    vec2 uv = gl_FragCoord.xy / u_resolution.xy ;// - vec2(0.5) ;
	// Flip Y Axis
	uv.y = -uv.y;
	
	highp float magnitude = 0.5;
	
	
	// Set up offset
	vec2 offsetRedUV = uv;
	offsetRedUV.x = uv.x + rand(vec2(TIME*0.03,uv.y*0.42)) * 0.001;
	offsetRedUV.x += sin(rand(vec2(TIME*0.006, uv.y))*0.5)*magnitude;
	
	vec2 offsetGreenUV = uv;
	offsetGreenUV.x = uv.x + rand(vec2(TIME*0.004,uv.y*0.002)) * 0.004;
	offsetGreenUV.x += sin(TIME*0.0001)*magnitude;
	
	vec2 offsetBlueUV = uv;
	offsetBlueUV.x = uv.y;
	offsetBlueUV.x += rand(vec2(cos(TIME*0.01),sin(uv.y)));
	
	// Load Texture
	float r = auraSlit(offsetRedUV).r;
	float g = auraSlit( offsetGreenUV).g;
	float b = auraSlit( uv).b;
//	vec3 colorY = colorSlit5(offsetRedUV,5.398)*vec3(0.960,0.924,0.007);

    //return vec4(colorY,1.0);
	return mix( vec4(r,g,b,1),auraSlit( uv),0.5);;
}



// grain
// https://www.shadertoy.com/view/4t2fRz
#define SHOW_NOISE 0
#define SRGB 0
// 0: Addition, 1: Screen, 2: Overlay, 3: Soft Light, 4: Lighten-Only
#define BLEND_MODE 3
#define SPEED 2.0
#define INTENSITY 0.075
// What gray level noise should tend to.
#define MEAN 0.0
// Controls the contrast/variance of noise.
#define VARIANCE 0.5

vec3 channel_mix(vec3 a, vec3 b, vec3 w) {
    return vec3(mix(a.r, b.r, w.r), mix(a.g, b.g, w.g), mix(a.b, b.b, w.b));
}

float gaussian(float z, float u, float o) {
    return (1.0 / (o * sqrt(2.0 * 3.1415))) * exp(-(((z - u) * (z - u)) / (2.0 * (o * o))));
}

vec3 madd(vec3 a, vec3 b, float w) {
    return a + a * b * w;
}

vec3 screen(vec3 a, vec3 b, float w) {
    return mix(a, vec3(1.0) - (vec3(1.0) - a) * (vec3(1.0) - b), w);
}

vec3 overlay(vec3 a, vec3 b, float w) {
    return mix(a, channel_mix(
        2.0 * a * b,
        vec3(1.0) - 2.0 * (vec3(1.0) - a) * (vec3(1.0) - b),
        step(vec3(0.5), a)
    ), w);
}

vec3 soft_light(vec3 a, vec3 b, float w) {
    return mix(a, pow(a, pow(vec3(2.0), 2.0 * (vec3(0.5) - b))), w);
}

vec4 grain1() {
    vec2 uv = gl_FragCoord.xy / u_resolution.xy ;// - vec2(0.5) ;
    vec4 color =glitch3_windfilter();
    #if SRGB
    color = pow(color, vec4(2.2));
    #endif
    
    float t = TIME * float(SPEED);
    float seed = dot(uv, vec2(12.9898, 78.233));
    float noise = fract(sin(seed) * 43758.5453 + t);
    noise = gaussian(noise, float(MEAN), float(VARIANCE) * float(VARIANCE));
    
    #if SHOW_NOISE
    color = vec4(noise);
    #else    
    // Ignore these mouse stuff if you're porting this
    // and just use an arbitrary intensity value.
    float w = float(INTENSITY);
    if (0.5 > 0.0) {
        w = 0.5 * uv.y;
        w *= step(uv.x, 0.5);
    }
	
    vec3 grain = vec3(noise) * (1.0 - color.rgb);
    
    #if BLEND_MODE == 0
    color.rgb += grain * w;
    #elif BLEND_MODE == 1
    color.rgb = screen(color.rgb, grain, w);
    #elif BLEND_MODE == 2
    color.rgb = overlay(color.rgb, grain, w);
    #elif BLEND_MODE == 3
    color.rgb = soft_light(color.rgb, grain, w);
    #elif BLEND_MODE == 4
    color.rgb = max(color.rgb, grain * w);
    #endif
        
    #if SRGB
    color = pow(color, vec4(1.0 / 2.2));
    #endif
    #endif
    return color;
}
















void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	//	both of these are also the same
	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
	//gl_FragColor = inputPixelColor;
	gl_FragColor = glitch3_windfilter();
}
