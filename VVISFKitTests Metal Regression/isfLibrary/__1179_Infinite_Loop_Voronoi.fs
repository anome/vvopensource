/*{
	"CREDIT": "by psylion",
	"DESCRIPTION": "Imported from ShaderToy ID:Xl2XWw See credit in Full Description",
	"CATEGORIES": [
		"Stylize"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	]
}*/


vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

/*
	Bumped Infinite Zoom Texture
	----------------------------

	The infinite zoom effect has been keeping me amused for years.

	This one is based on something I wrote some time ago, but was inspired by Fabrice Neyret's 
	"Infinite Fall" shader. I don't know if it "technically" qualifies as "infinite zoom," but
	it at least gives that feel.

	I put together a couple of overly optimized versions a couple of days ago, just for fun, and 
	Fabrice's comments were pretty helpful. I also liked the way he did the layer rotation in his 
	"Infinite Fall" version, so I'm using that. The rest is stock standard infinite zoom stuff 
	that has been around for years.

	Most people like to use noise for this effect, so I figured I'd do something different
	and use a texture. I've also bump mapped it, added specular highlights, etc. It was 
	tempting to add a heap of other things, but I wanted to keep the example relatively simple.

	By the way, most of the code is basic bump mapping and lighting. The infinite zoom code 
	takes up just a small portion.
	

	Fabrice Neyret's versions:

	infinite fall - short
	https://www.shadertoy.com/view/ltjXWW

    infinite fall - FabriceNeyret2
    https://www.shadertoy.com/view/4sl3RX

	Other examples:

    Fractal Noise - mu6k
    https://www.shadertoy.com/view/Msf3Wr

    Infinite Sierpinski - gleurop
    https://www.shadertoy.com/view/MdfGR8

    Infinite Zoom - fizzer
    https://www.shadertoy.com/view/MlXGW7


*/

// Grey scale.
float getGrey(vec3 p){ return p.x*0.299 + p.y*0.587 + p.z*0.114; }

void mainImage( out vec4 fragColor, in vec2 fragCoord ){

    // Screen coordinates.
	vec2 uv = (fragCoord - iResolution.xy*.5)/iResolution.y;

    // Variable setup, plus rotation.
	float t = iGlobalTime, s, a, b, e;
    
    
    // Rotation the canvas back and forth. I've kept it slow, so as not to 
    // detract too much from the infinite zoom feel.
    float th = sin(iGlobalTime*0.05)*sin(iGlobalTime*0.06)*4.;
    float cs = cos(th), si = sin(th);
    uv *= mat2(cs, -si, si, cs);
    

    // Setup: I find 2D bump mapping more intuitive to pretend I'm raytracing, then lighting a bump mapped plane 
    // situated at the origin. Others may disagree. :)  
    vec3 sp = vec3(uv, 0); // Surface posion. Hit point, if you prefer. Essentially, a screen at the origin.
    vec3 ro = vec3(0, 0, -1); // Camera position, ray origin, etc.
    vec3 rd = normalize(sp-ro); // Unit direction vector. From the origin to the screen plane.
    vec3 lp = vec3(cos(iGlobalTime)*0.375, sin(iGlobalTime)*0.1, -1.); // Light position - Back from the screen.
    //vec3 lp = vec3(0., 0., -1.); // Light position - Back from the screen.
 
    
    // The number of layers. More gives you a more continous blend, but is obviously slower.
    // If you change the layer number, you'll proably have to tweak the "gFreq" value.
    const float L = 8.;
     // Global layer frequency, or global zoom, if you prefer.
    const float gFreq = 0.125*0.25;
    float sum = 0.; // Amplitude sum, of sorts.
    
    
    // Setting up the layer rotation matrix, used to rotate each layer.
    // Not completely necessary, but it helps mix things up. It's standard practice, but 
    // this one is based on Fabrice's example.
    th = 3.14159265*0.7071/L;
    cs = cos(th), si = sin(th);
    mat2 M = mat2(cs, -si, si, cs);
    
    
    // The overall scene color. Initiated to zero.
    vec3 col = vec3(0);
    
    
    
    
    // Setting up the bump mapping variables and initiating them to zero.
    // f - Function value
    // fx - Change in "f" in in the X-direction.
    // fy - Change in "f" in in the Y-direction.
    float f=0., fx=0., fy=0.;
    vec2 eps = vec2(2./iResolution.y, 0.);
    
    // I've had to off-center this just a little to avoid an annoying white speck right
    // in the middle of the canvas. If anyone knows how to get rid of it, I'm all ears. :)
    vec2 offs = vec2(0.0);
    
    
    vec3 zCol = vec3(0);
    
    
    // Infinite Zoom.
    //
    // The first three lines are a little difficult to explain without describing what infinite 
    // zooming is in the first place. A lot of it is analogous to fBm. Sum a bunch of increasing
    // frequencies with decreasing amplitudes.
    //
    // Anyway, the effect is nothing more than a series of layers being expanded from an initial
    // size to a final size, then being snapped back to its original size to repeat the process 
    // again. However, each layer is doing it at diffent incremental stages in time, which tricks
    // the brain into believing the process is continuous. If you wish to spoil the illusion, 
    // simply reduce the layer count. If you wish to completely ruin the effect, set it to one.
	
    // Infinite zoom loop.
	for (float i = 0.; i<L; i++){
	
        // Fractional time component. Obviously, incremented by "1./L" and ranging from
        // zero to one, whist on a repeat cycle.
		s = fract((i - t*2.)/L);
        
        // Using the fractional time component to determine the layer frequency. It increases
        // with time, then snaps back to one in a cyclic fashion.
        // Note that exp2(t) is just another way to write pow(2., s). The latter is more
        // intuitive, but slower... We'll, I assume it is.
        e = exp2(s*L)*gFreq; // Range (approx): [ 1, pow(2., L)*gFreq ]
        
        // Layer ampltude component. Inversely propotional to the frequency, which makes sense.
        // Because the layers are finite, you need to smoothly interpolate between them, 
        // and the "cos" setup below is just one of many ways to do it.
        a = (1.-cos(s*6.283))/e;  // Smooth transition.
        //a = (1. - abs(s-.5)*2.); // Alternative linear fade. Not as smooth, or accurate.
        //a *= a*(3.-2.*a)/e; // Smoothing the linear fade above, if so desired.
        
        
        // Accumulating each layer.
        
        // I had to have a bit of a think as to how to bump map this. Normally, you'd write a function
        // then call it three times, but that'd be too expensive, so it's all being done simultaneously.
        //
        // Either way, it's still pretty simple. In addition to accumulating the pixel value, accumulate 
        // sample values just to the left of it and above. The X-gradient and Y-gradient can then be 
        // determined outside the loop.
		//
        vec3 tex = texture2D(inputImage, M*sp.xy*e + offs).xyz;
        
        f += getGrey(tex) * a; // Sample value multiplied by the amplitude.
        fx += getGrey(texture2D(inputImage, M*(sp.xy-eps.xy)*e + offs).xyz) * a; // Same for the nearby sample in the X-direction.
        fy += getGrey(texture2D(inputImage, M*(sp.xy-eps.yx)*e + offs).xyz) * a; // Same for the nearby sample in the Y-direction.
         
        zCol += tex * a;
        
        
        // Sum each amplitude. Used to normalize the results once the loop is complete.
        sum += a;
        
        // Rotating each successive layer is pretty standard, but this is the way Fabrice does
        // it.
        M *= M;

	}
    
    // I doubt it'd happen, but just in case sum is zero.
    sum = max(sum, 0.001);
    
    
    zCol /= sum;
    
    // Normalizing the three Voronoi samples.
    f /= sum;
    fx /= sum;
    fy /= sum;
   
 
    // Common bump mapping stuff.
    float bumpFactor = 0.05;
    // Using the above to determine the dx and dy function gradients.
    fx = (fx-f)/eps.x; // Change in X
    fy = (fy-f)/eps.x; // Change in Y.
    // Using the gradient vector, "vec3(fx, fy, 0)," to perturb the XY plane normal ",vec3(0, 0, -1)."
    vec3 n = normalize( vec3(0, 0, -1) + vec3(fx, fy, 0)*bumpFactor );           
   
    
    
    
	// Determine the light direction vector, calculate its distance, then normalize it.
	vec3 ld = lp - sp;
	float lDist = max(length(ld), 0.001);
	ld /= lDist;
    
    

    // Light attenuation.    
    float atten = min(1./(lDist*0.75 + lDist*lDist*0.15), 1.);
	//float atten = min(1./(dist*dist*2.), 1.);
	

	// Diffuse value.
	float diff = max(dot(n, ld), 0.);  
    // Enhancing the diffuse value a bit. Made up.
    diff = pow(diff, 2.)*0.66 + pow(diff, 4.)*0.34; 
    // Specular highlighting.
    float spec = pow(max(dot( reflect(-ld, n), -rd), 0.), 16.); 


	// Using the infinite Voronoi value to produce a purplish color.
	vec3 objCol = zCol;//vec3(f, f*f*sqrt(f)*0.4, f*0.6);
    // Blood... ruby red.
    //vec3 objCol = vec3(f, pow(f, 6.), pow(f, 3.)*0.5);
    

    // Using the values above to produce the final color.   
    col = (objCol * (diff + 0.5) + vec3(0.8, 0.95, 1.)*spec*0.65) * atten;


    // Done. 
	fragColor = vec4(min(col, 1.), 1.);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}