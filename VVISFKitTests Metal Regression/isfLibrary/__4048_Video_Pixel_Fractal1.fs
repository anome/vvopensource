/*{
	"CREDIT": "by kawaiiidesu",
	"DESCRIPTION": "Video Pixel Fractal",
	"CATEGORIES": [
		"fx Box"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	
	]
}*/


vec3 iResolution = vec3(RENDERSIZE, 1.);
float iTime = TIME;

//////////////////////////////////////////////////////////////////////////////////
// Video Pixel Fractal - Copyright 2017 Frank Force
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
//////////////////////////////////////////////////////////////////////////////////

const float zoomSpeed			= 0.3;	// how fast to zoom (negative to zoom out)
const float zoomScale			= 0.001;	// how much to multiply overall zoom (closer to zero zooms in)
const int recursionCount		= 2;	// how deep to recurse
const float recursionFadeDepth	= 0.0;	// how deep to fade out
const int glyphSize				= 64;	// width & height of glyph in pixels
const float glyphMargin			= 0.0;	// how much to center the glyph in each pixel
const float brightness			= 0.6;

//////////////////////////////////////////////////////////////////////////////////
// Precached values and math

const float glyphSizeF = float(glyphSize) + 2.0*glyphMargin;
const float glyphSizeLog = log(glyphSizeF);
const int powTableCount = 10;
const float gsfi = 1.0 / glyphSizeF;
const float powTable[powTableCount] = float[]( 1.0, gsfi, pow(gsfi,2.0), pow(gsfi,3.0), pow(gsfi,4.0), pow(gsfi,5.0), pow(gsfi,6.0), pow(gsfi,7.0), pow(gsfi,8.0), pow(gsfi,9.0));
const float e = 2.718281828459;
const float pi = 3.14159265359;

float RandFloat(int i) { return (fract(sin(float(i)) * 43758.5453)); }
int RandInt(int i) { return int(100000.0*RandFloat(i)); }

vec3 HsvToRgb(vec3 c) 
{
    float s = c.y * c.z;
    float s_n = c.z - s * .5;
    return vec3(s_n) + vec3(s) * cos(2.0 * pi * (c.x + vec3(1.0, 0.6666, .3333)));
}

//////////////////////////////////////////////////////////////////////////////////
// Color and image manipulation

float GetRecursionFade(int r, float timePercent)
{
    if (r > recursionCount)
        return timePercent;
    
    // fade in and out recusion
    float rt = max(float(r) - timePercent - recursionFadeDepth, 0.0);
    float rc = float(recursionCount) - recursionFadeDepth;
    return rt / rc;
}

vec3 InitPixelColor() { return vec3(0); }
vec3 CombinePixelColor(vec3 color, float timePercent, int i, int r, vec2 pos, ivec2 glyphPos, ivec2 glyphPosLast)
{
    /*vec3 myColor = vec3
    (
    	mix(-0.2, 0.2, RandFloat(i + r + 419*(glyphPosLast.x + glyphPosLast.y))),
    	mix(0.0, 1.0, RandFloat(i + r + 929*(glyphPosLast.x + glyphPosLast.y))),
        mix(0.0, 1.0, RandFloat(i + r + 316*(glyphPosLast.x + glyphPosLast.y)))
    );

    // combine with my color
    float f = GetRecursionFade(r, timePercent);
    color.x += myColor.x*f;;
        color.y = max(color.y, myColor.y*f);
    color.z = max(color.z, myColor.z*pow(f,1.5));
    return color;*/
    vec3 myColor = vec3(1.0, 1.0, 1.0);
    //float f = GetRecursionFade(r, timePercent);
    //color = mix(color, myColor, f);
    return myColor;
}

vec3 FinishPixel(vec3 color, vec2 uv)
{
    // color wander
    //color.x += (0.05*uv.y + 0.05*uv.x + 0.05*iTime);
    
    // convert to rgb
    //color = HsvToRgb(color);
    return color;
}

vec2 InitUV(vec2 uv)
{
	// wave
	//uv.x += 0.01*sin(10.0*uv.y + 0.17*iTime);
	//uv.y += 0.01*sin(10.0*uv.x + 0.13*iTime);
	//uv.x += 0.1*sin(2.0*uv.y + 1.0*iTime);
	//uv.y += 0.1*sin(2.0*uv.x + 0.8*iTime);
    return uv;
}

//////////////////////////////////////////////////////////////////////////////////
// Fractal functions

vec3 GetGlyphPixel(ivec2 pos)
{
    vec2 uv = vec2(pos) + 0.5f;
    uv /= float(glyphSize);
    
    vec4 c = texture(inputImage,uv);
    //return (c.x + c.y + c.z) / 3.0f;
    return vec3(c);
    
}

ivec2 focusList[max(powTableCount, recursionCount) + 2];
ivec2 GetFocusPos(int i) { return focusList[i+2]; }

ivec2 CalculateFocusPos(int iterations)
{
    //return ivec2(RandInt(iterations) % glyphSize, RandInt(iterations) % glyphSize);
    //return ivec2(glyphSize / 2);
    return ivec2(glyphSize/4 + RandInt(iterations) % (glyphSize/2), glyphSize/4 +RandInt(iterations) % (glyphSize/2));
}
      
// get color of pos, where pos is 0-1 point in the glyph
vec3 GetPixelFractal(vec2 pos, int iterations, float timePercent)
{
	ivec2 glyphPosLast = GetFocusPos(-2);
	ivec2 glyphPos =     GetFocusPos(-1);
    
	bool isFocus = true;
    ivec2 focusPos = glyphPos;
    
	vec3 color = InitPixelColor();
	for (int r = 0; r <= recursionCount + 1; ++r)
	{
        //color = CombinePixelColor(color, timePercent, iterations, r, pos, glyphPos, glyphPosLast);
        
        //if (r == 1 && glyphPos == GetFocusPos(r-1))
	    //    color.z = 1.0; // debug - show focus
           
        // update pos
        pos -= vec2(glyphMargin*gsfi);
        pos *= glyphSizeF;

        // get glyph and pos within that glyph
        glyphPosLast = glyphPos;
        glyphPos = ivec2(pos);

        // check pixel
        vec3 glyphValue = GetGlyphPixel(glyphPos);
        //color.z *= glyphValue;
        //glyphValue = pow(glyphValue, 2.0) + 0.2;
        //glyphValue = pow(glyphValue, vec3(2.0)) + vec3(0.2);
        glyphValue *= brightness;
       
        vec3 myColor = vec3(0.0, 0.0, glyphValue);
        float f = GetRecursionFade(r, timePercent);
        //color = mix(color, myColor, f);
        color += glyphValue*f;//
        
        if (r > recursionCount)
			return color;
        
		if (pos.x < 0.0 || pos.y < 0.0)
			return color;
        
        // next glyph
		pos -= vec2(floor(pos));
        if (isFocus)
        {
        	isFocus = isFocus && (glyphPosLast == focusPos);
            focusPos = isFocus? GetFocusPos(r) : ivec2(-100);
        }
	}
}
 
//////////////////////////////////////////////////////////////////////////////////
	
void main1()
{
	// use square aspect ratio
	vec2 uv = isf_FragNormCoord.xy;
	uv = fragCoord / iResolution.y;
	uv -= vec2(0.5*iResolution.x / iResolution.y, 0.5);
    uv = InitUV(uv);
	
	// get time 
	float timePercent = iTime*zoomSpeed;
	int iterations = int(floor(timePercent));
	timePercent -= float(iterations);
	
	// update zoom, apply pow to make rate constant
	float zoom = pow(e, -glyphSizeLog*timePercent);
	zoom *= zoomScale;
    
    // cache focus positions
    for(int i = 0; i  < powTableCount + 2; ++i)
      focusList[i] = CalculateFocusPos(iterations+i-2);
    
	// get offset
	vec2 offset = vec2(0);
	for (int i = 0; i < powTableCount; ++i)
		offset += ((vec2(GetFocusPos(i)) + vec2(glyphMargin)) * gsfi) * powTable[i];
    
	// apply zoom & offset
    vec2 uvFractal = uv * zoom + offset;
	
	// check pixel recursion depth
	vec3 pixelFractalColor = GetPixelFractal(uvFractal, iterations, timePercent);
    pixelFractalColor = FinishPixel(pixelFractalColor, uv);
    
	// apply final color
	//gl_FragColor = vec4(pixelFractalColor, 1.0);
	//gl_FragColor = vec4(1.0, 1.0,1.0,1.0);
	
}

void main() {
	gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
}