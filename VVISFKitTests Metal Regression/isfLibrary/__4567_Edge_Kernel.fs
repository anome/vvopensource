#extension GL_OES_standard_derivatives : enable

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



//precision highp float;
#define tc gl_fragCoord.xy
#define time TIME
//uniform sampler2D texture0;
//uniform sampler2D texture1;
#define res RENDERSIZE
#define pix 1.0/res

#define feedIn texture1
#define camIn texture0
#define feed(x) texture2D(feedIn,uv+pix*x)
#define cam(x)  texture2D(camIn,uv+pix*x)

#define edge1(x) texture2D(texture0,uv+pix*x)
#define minEdge(x,y) x=min(x,edge1(y))
#define maxEdge(x,y) x=max(x,edge1(y))
#define subEdge(x,y) x-=edge1(y)
#define addEdge(x,y) x+=edge1(y)

#define rep(x,y,z) for(float i=x;i<y;i+=z)
//#define repif(w,x,y,z) if(w){for(float i=x;i<y;i+=z)
#define sat(x) clamp(x,0.,1.)
#define ss(a,b,x) smoothstep(a,b,x)

float kernel(sampler2D tex, float k[9]){
   return 0.;
}

float weight = 1.0;//(sin(time)+1.5)*4.;

void main(void){
    mat3 I;
    float cnv[9];
    vec3 sample;

    mat3 G[9]; 
    G[0] = 1.0/(2.0*sqrt(2.0)) * mat3( 1.0, sqrt(2.0), 1.0, 0.0, 0.0, 0.0, -1.0, -sqrt(2.0), -1.0 );
    G[1] = 1.0/(2.0*sqrt(2.0)) * mat3( 1.0, 0.0, -1.0, sqrt(2.0), 0.0, -sqrt(2.0), 1.0, 0.0, -1.0 );
    G[2] = 1.0/(2.0*sqrt(2.0)) * mat3( 0.0, -1.0, sqrt(2.0), 1.0, 0.0, -1.0, -sqrt(2.0), 1.0, 0.0 );
    G[3] = 1.0/(2.0*sqrt(2.0)) * mat3( sqrt(2.0), -1.0, 0.0, -1.0, 0.0, 1.0, 0.0, 1.0, -sqrt(2.0) );
    G[4] = 1.0/2.0 * mat3( 0.0, 1.0, 0.0, -1.0, 0.0, -1.0, 0.0, 1.0, 0.0 );
    G[5] = 1.0/2.0 * mat3( -1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, -1.0 );
    G[6] = 1.0/6.0 * mat3( 1.0, -2.0, 1.0, -2.0, 4.0, -2.0, 1.0, -2.0, 1.0 );
    G[7] = 1.0/6.0 * mat3( -2.0, 1.0, -2.0, 1.0, 4.0, 1.0, -2.0, 1.0, -2.0 );
    G[8] = 1.0/3.0 * mat3( 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 ); 
    
    // fetch the 3x3 neighbourhood and use the RGB vector's length as intensity value 
    for (int i=0; i<3; i++) for (int j=0; j<3; j++) {
        sample = texture2D( texture0,
             vec2(tc)
            + vec2(float(i-1),
             float(j-1))/res).rgb;
        sample += 0.055*texture2D( texture1,
             vec2(tc)
            + vec2(float(i-1),
             float(j-1))/res).rgb
            *(sin(time*0.5)+0.5)*0.75;
        //sample*=0.25;
            
        I[i][j] = length(sample); 
    }

    // calculate the convolution values for all the masks 
    for (int i=0; i<9; i++) {
        float dp3 = dot(G[i][0], I[0]) + dot(G[i][1], I[1]) + dot(G[i][2], I[2]);
        cnv[i] = dp3 * dp3; 
    }
    
    float M = (cnv[0] + cnv[1]) + (cnv[2] + cnv[3]);
    float S = (cnv[4] + cnv[5]) + (cnv[6] + cnv[7]) + (cnv[8] / M); // was + M
    float SOFTNESS = 0.15;
    float EDGESTEP = 0.25;
    float edge = 1.0-smoothstep(
        weight * sqrt(M/S)-SOFTNESS,
        weight * sqrt(M/S)+SOFTNESS, EDGESTEP);
        
    gl_FragColor = vec4(edge);
}





/*
void main(){
   vec2 uv  = gl_FragCoord.st/res;
   vec4 cam = texture2D(texture0,uv);
   vec4 fbo = texture2D(texture1,uv);
   vec4 color = cam+fbo, minc = fbo, maxc = fbo;
   float maxcc = (cam.r+cam.g+cam.b)/3.;
         maxcc += (fbo.r+fbo.g+fbo.b)/3.;//step(cam,vec4(0.5));
   maxcc = ss(0.8, 0.9,maxcc);
   float kern[9];


   rep(-20.,20., 4.){
      minEdge(minc, vec2(i,0));
      minEdge(minc, vec2(0,i));
      minEdge(minc, vec2(i,-i));
      minEdge(minc, vec2(-i));
   }

   //if(maxcc>0.45){
   rep(-20.,20., 1.){
      //float ii = abs(i)/20.; //ii *= ii;
      maxEdge(maxc, vec2(i*4.0, 0));
      maxEdge(maxc, vec2(0,i*2.0));
      maxEdge(maxc, vec2(i,-i));
      maxEdge(maxc, vec2(-i));
   //}
   }

   rep(-20.,20., 2.){
      subEdge(color, vec2(i,0));
      subEdge(color, vec2(0,i));
      addEdge(color, vec2(i));
      addEdge(color, vec2(-i));
   }

   //color += cam;
   //color = fwidth(cam)*50.0-2.;
   //color = max(color,texture2D(texture0, uv+shft));
   //color = vec4(dot(10.0/fbo, fwidth(color)))-0.10;
   gl_FragColor.rgb = vec3(maxcc);//mix(cam, maxc, 0.5);//sat(1.0-color));
}
*/

void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	//	both of these are also the same
	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
	gl_FragColor = inputPixelColor;
}
