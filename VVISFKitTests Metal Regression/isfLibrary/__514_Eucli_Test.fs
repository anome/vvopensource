/*{
    "CATEGORIES": [
        "XXX"
    ],
    "CREDIT": "",
    "INPUTS": [
        {
            "DEFAULT": 0.8,
            "MAX": 1,
            "MIN": 0,
            "NAME": "size",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0,
            "LABEL": "angle_in",
            "MAX": 6.283185307179586,
            "MIN": 0,
            "NAME": "angle_in",
            "TYPE": "float"
        },
        {
            "DEFAULT": 12,
            "LABEL": "Steps",
            "MAX": 32,
            "MIN": 0,
            "NAME": "Steps",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
}
*/

//define value
    #define PI 3.141592653589793
    #define TAU = PI *2.
    #define OR 1.61803398875
	

//rotate
	mat2 rotate (float angle){
	return mat2(
			-cos(angle_in), sin(angle_in),
			sin(angle_in), cos(angle_in));
}

//Resolution Screen
	float res = (RENDERSIZE.x/RENDERSIZE.y);
	vec3 resolution = vec3(RENDERSIZE, 1.);
	
//Ring
float ring (float rad, float thick)
{
	vec2 uv = ( -resolution.xy + 2.0 * gl_FragCoord.xy ) / resolution.y;
    float r = size * rad;   								// radius
    float d = length(uv/.5);							// distance of this pixel from origin
   	float c = smoothstep(r, r - (thick / 2.0)*size, d) + 	// calculate color of this pixel based on
        	 smoothstep(r, r + (thick / 2.0)*size, d); 	// ring parameters
    return c;
}


//Ring 2
float ring2(vec2 st, float radius)
{
    float r = 0.4 * radius;   						// radius
    float dr = 0.015; 								// delta radius (thickness)
    						
    float d = length(st);							// distance of this pixel from origin
    float c = smoothstep(r, r - (dr / 3.), d) + 	// calculate color of this pixel based on
        	  smoothstep(r, r + (dr / 4.0), d); 	// ring parameters
    return c;
}

//Segment
float segment(vec2 p, vec2 a, vec2 b, float thickness )
{
	vec2 pa = p - a;
	vec2 ba = b - a;
	float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
	return smoothstep(thickness * 0.8, thickness * 1.2, length(pa - ba * h));
}

//segment 2
float segment2 (vec2 P1, vec2 P2, vec2 Coord)
{
    //Find the length of the line segment.
    float Length = distance(P1,P2);
    //Find the slope vector of the line.
    vec2 Slope = (P1-P2)/Length;
    //Find the perpendicular vector to the slope.
    vec2 Normal = Slope.yx*vec2(-1,1);
    //Line thickness in pixels.
    const float Width = 4.;
    //Calculate distance to line. (This should to be clamped).
    float LineWidth = Width*.5-abs(dot(Coord-P1,Normal));
    //Calculate distance to line ends. (This should to be clamped).
    float LineLength = Length/2.-abs(dot(Coord-(P1+P2)/2.,Slope));
    //Find the distance to the line edges.
    float Line = clamp(min(LineWidth,LineLength),0.,1.);
    
    return Line; 
}

//circle
float circle(vec2 uv, vec2 p, float r, float blur)
{
	float d = length(uv-p);
	float c = smoothstep (r, r-blur, d);
	return c;
}


	// Repeat around the origin by a fixed angle.
    // For easier use, num of repetitions is use to specify the angle.
float pModPolar(inout vec2 p, float repetitions) {
	float angle = 2.*(3.14152)/repetitions;
	float a = atan(p.y, p.x) + angle/2.;
	float r = length(p);
	float c = floor(a/angle);
	a = mod(a,angle) - angle/2.;
	p = vec2(cos(a), sin(a))*r;
	// For an odd number of repetitions, fix cell index of the cell in -x direction
	// (cell index would be e.g. -5 and 5 in the two halves of the cell):
	if (abs(c) >= (repetitions/2.)) c = abs(c);
	return c;
}

void main()	{
	vec2 uv = (gl_FragCoord.xy - .5 * RENDERSIZE.xy ) / RENDERSIZE.y;
	uv = (rotate((TIME)) * uv);
	
    vec4 color;
    
    float circleposition = pModPolar( uv, Steps);
    
	vec4 ringcolor2 = vec4(1.- ring2(vec2(uv), size));
	vec4 circlecolor = vec4(circle(uv, vec2(circleposition), 0.03, 0.01));
    vec4 segmentcolor = vec4(segment2 (vec2 (0.0), vec2 (0.4), vec2 (uv)));
    color += vec4(segmentcolor+ringcolor2+circlecolor);
	
	gl_FragColor = color;
	

}