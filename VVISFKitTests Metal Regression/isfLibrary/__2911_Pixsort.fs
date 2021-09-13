/*
{
  "IMPORTED" : [
    {
      "NAME" : "iChannel0",
      "PATH" : "fb918796edc3d2221218db0811e240e72e340350008338b0c07a52bd353666a6.jpg"
    }
  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/MstyDl by Wunkolo.  GLSL based vector field pixel sorting\n\nCrude attempt at implementing PixSort( https://wunkolo.itch.io/pixsort ) as a GLSL shader.",
  "INPUTS" : [

  ]
}
*/


// A basic port of Adobe After Effects plugin PixSort: https://wunkolo.itch.io/pixsort
// Created by Wunkolo/2018
// Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International Public License

// Unless you got a beefy as hell GPU
// dont make MAXSPAN any higher than it needs to.
// Each pixel is potentially O( MAXSPAN ^ 2 )
// Overall this is very nasty and has no business being a glsl shader
#define MAXSPAN 27
#define MINSPAN 16

vec4 Span[MAXSPAN];

// Samples a single texel
vec4 Texel( ivec2 Coord )
{
    ivec2 CurSize = ivec2( IMG_SIZE(iChannel0).xy );
    vec2 CurCoord = vec2( 0.0 );
    CurCoord.x = (mod(float(Coord.x),float(CurSize.x)));
    CurCoord.y = (mod(float(Coord.y),float(CurSize.y)));
    return IMG_PIXEL( iChannel0, CurCoord );    
}

// Converts any color into a single ordinal float
// This is what it sorts by
float Bias( vec4 Color )
{
    return dot( Color.rgb, vec3( 0.3, 0.59, 0.11 ) );
}

// Mask for sorting
// Areas of "false" are where sortings occur
// Areas of "true" are where they dont
bool Threshold( vec4 Color )
{
    // True when "Bright"
    // False when "Dark"
    return dot( Color.rgb, vec3( 0.3, 0.59, 0.11 ) ) > 0.5;
}

// Vector field which gives a direction vector given a spacial coordinate
// Determines the "Flow" of the pixel sorting spans
vec2 Flow( vec2 Coord )
{
    Coord /= IMG_SIZE(iChannel0).xy;
    Coord -= vec2(0.5);
    return normalize(vec2(
        sin( Coord.x/Coord.y + TIME),
        Coord.y
    ));
}

// Gets the index of the current span following flow until we hit a "wall" pixel
int GetSpanIndex( ivec2 Coord )
{
    int CurIndex = 0;
    vec2 CurVector = vec2(Coord);
    for( ; CurIndex < MAXSPAN; ++CurIndex )
    {
        vec4 CurTexel = Texel(ivec2(CurVector));
        // Integrate backwards until we hit an "edge"
        if( Threshold(CurTexel) == true )
        {
            // Hit a border
            // This pixel doesnt count
            CurIndex--;
            break;
        }
        Span[CurIndex] = CurTexel;
        // get previous pixel
        CurVector += -Flow(CurVector);
    }
    return CurIndex;
}

// Gets the upper index of the current span following flow until we hit a "wall" pixel
// This tells us how large the current span is
int GetUpperSpanIndex( ivec2 Coord, int CurIndex )
{
    vec2 CurVector = vec2(Coord);
    for( ; CurIndex < MAXSPAN; ++CurIndex )
    {
        vec4 CurTexel = Texel(ivec2(CurVector));
        // Integrate forward until we hit an "edge"
        if( Threshold(CurTexel) == true )
        {
            // Hit a border
            // This pixel doesnt count
            CurIndex--;
            break;
        }
        Span[CurIndex] = CurTexel;
        // get next pixel
        CurVector += Flow(CurVector);
    }
    return CurIndex;
}

// Fallback/debug
vec4 Fallback( ivec2 Coord )
{
    vec4 Final = Texel(Coord);
    if( Coord.x < int(iMouse.x) )
    {
        vec2 Direction = Flow(vec2(Coord)).xy * 0.5 + 0.5;
        Final = Threshold(Final) ? vec4(Direction,1.0,1.0): vec4(Direction,0.0,1.0);
    }
    return Final;
}

// Bubblesort
// This is going to be disgustingly slow
// like O(n^2)-per-sorted-pixel slow
void SortSpan( int CurSize )
{
    for( int i = 0; i < MAXSPAN; ++i )
    {
        if( i >= CurSize )
        {
            break;
        }
        for( int j = 0; j < MAXSPAN; ++j )
        {
        	if( j >= CurSize )
        	{
	            break;
    	    }
            if( Bias(Span[j]) < Bias(Span[i]) )
            {
                vec4 Temp = Span[j];
                Span[j] = Span[i];
                Span[i] = Temp;
            }
        }
    }
}

void main() {



    ivec2 ThisCoord = ivec2(gl_FragCoord.xy);
    vec4 CurColor = Texel(ThisCoord);
    
    // Find out what index we're currently at
    int CurIndex = GetSpanIndex(ThisCoord);
    // No interval here
    if( CurIndex == MAXSPAN )
    {
        // return faded pixel
        gl_FragColor = Fallback(ThisCoord);
        return;
    }
    
    // Get the rest of the "upper" pixels
    int UpperIndex = GetUpperSpanIndex(ThisCoord, CurIndex);
    // Interval too short
    if( UpperIndex < MINSPAN )
    {
        // return faded pixel
        gl_FragColor = Fallback(ThisCoord);
        return;
    }
    
    // Select the "CurIndex"-th sorted pixel within
    // an array of "UpperIndex+1" pixels
    SortSpan(UpperIndex+1);
    vec4 FinalColor = Span[CurIndex];
    
    // Output to screen
    gl_FragColor = vec4(
        //vec3( float(CurIndex) / float(UpperIndex) ),
        FinalColor.rgb,
        1.0
    );
}
