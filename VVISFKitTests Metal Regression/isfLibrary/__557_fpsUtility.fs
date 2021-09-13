/*{
	"CREDIT": "by mojovideotech",
  	"CATEGORIES" : [
  		"utility",
    	"fps"
  	],
  	"DESCRIPTION" : "based on https://www.shadertoy.com/view/lsKGWV by iq.",
  	"INPUTS" : [
  	],
  "ISFVSN" : "2"
}
*/

// NOTE : does not render correctlly on mobile browsers (?)

////////////////////////////////////////////////////////////
// fpsUtility  by mojovideotech
// seconds elapsed / frames rendered / time delta / avg fps 
//
// based on :
//
// shadertoy.com/view/lsKGWV
// by inigo quilez - iq/2016
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#ifdef GL_ES
precision mediump float;
#endif

// Digit drawing function by P_Malin (shadertoy.com/view/4sf3RN)
float SampleDigit(const in float n, const in vec2 vUV)
{		
	if(vUV.x  < 0.0) return 0.0;
	if(vUV.y  < 0.0) return 0.0;
	if(vUV.x >= 1.0) return 0.0;
	if(vUV.y >= 1.0) return 0.0;
	float data = 0.0;
	     if(n < 0.5) data = 7.0 + 5.0*16.0 + 5.0*256.0 + 5.0*4096.0 + 7.0*65536.0;
	else if(n < 1.5) data = 2.0 + 2.0*16.0 + 2.0*256.0 + 2.0*4096.0 + 2.0*65536.0;
	else if(n < 2.5) data = 7.0 + 1.0*16.0 + 7.0*256.0 + 4.0*4096.0 + 7.0*65536.0;
	else if(n < 3.5) data = 7.0 + 4.0*16.0 + 7.0*256.0 + 4.0*4096.0 + 7.0*65536.0;
	else if(n < 4.5) data = 4.0 + 7.0*16.0 + 5.0*256.0 + 1.0*4096.0 + 1.0*65536.0;
	else if(n < 5.5) data = 7.0 + 4.0*16.0 + 7.0*256.0 + 1.0*4096.0 + 7.0*65536.0;
	else if(n < 6.5) data = 7.0 + 5.0*16.0 + 7.0*256.0 + 1.0*4096.0 + 7.0*65536.0;
	else if(n < 7.5) data = 4.0 + 4.0*16.0 + 4.0*256.0 + 4.0*4096.0 + 7.0*65536.0;
	else if(n < 8.5) data = 7.0 + 5.0*16.0 + 7.0*256.0 + 5.0*4096.0 + 7.0*65536.0;
	else if(n < 9.5) data = 7.0 + 4.0*16.0 + 7.0*256.0 + 5.0*4096.0 + 7.0*65536.0;
	vec2 vPixel = floor(vUV * vec2(4.0, 5.0));
	float fIndex = vPixel.x + (vPixel.y * 4.0);
	
	return mod(floor(data / pow(2.0, fIndex)), 2.0);
}

float PrintInt(const in vec2 uv, const in float value)
{
	float res = 0.0;
	float maxDigits = 1.0+ceil(log2(value)/log2(10.0));
	float digitID = floor(uv.x);
	if( digitID>0.0 && digitID<maxDigits ) {
        float digitVa = mod( floor( value/pow(10.0,maxDigits-1.0-digitID) ), 10.0 );
        res = SampleDigit( digitVa, vec2(fract(uv.x), uv.y) );
	}

	return res;	
}

void main() 
{
    vec3 col;
    float px = 1.0 / RENDERSIZE.y;
    vec2 uv = gl_FragCoord.xy*px;
    float T = floor(TIME),F = float(FRAMEINDEX);
    col.rgb += PrintInt((uv-vec2(0.1,0.5))*10.0,T);
    col.gb += PrintInt((uv-vec2(0.1,0.3))*10.0,F);
    col.g += PrintInt((uv-vec2(0.1,0.1))*10.0, floor(1.0/TIMEDELTA + 0.5));
    col.r += PrintInt((uv-vec2(0.5,0.1))*10.0,F/T);

    gl_FragColor = vec4(col,1.0);
}
