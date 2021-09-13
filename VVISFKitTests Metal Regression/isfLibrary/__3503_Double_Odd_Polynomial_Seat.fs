/*{
	"CREDIT": "by xdxst",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "scale",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.1,
			"MAX": 1
		},
		{
			"NAME": "offset",
			"TYPE": "point2D",
			"DEFAULT": [0, 0],
			"MIN": [0,0],
			"MAX": [1,1]
		},
		{
			"NAME": "controlPoint",
			"TYPE": "point2D",
			"DEFAULT": [0.5,0.5],
			"MIN": [0,0],
			"MAX": [1,1]
		},
		{
			"NAME": "n",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 1,
			"MAX": 20
		}
	]
}*/

#ifdef GL_ES
precision mediump float;
#endif

#define TAU 2.0*3.14159

// Plot a line on X or Y using a value between 0.0-1.0
float plot(float var, float pct, float thickness){
	return smoothstep( pct-thickness, pct, var) -
		   smoothstep( pct, pct+thickness, var);
}

float doubleOddPolynomialSeat(float x, float a, float b, int n) {
	float epsilon = 0.00001;
	float min_param_a = 0.0 + epsilon;
	float max_param_a = 1.0 - epsilon;
	float min_param_b = 0.0;
	float max_param_b = 1.0;
	a = min(max_param_a, max(min_param_a, a));
	b = min(max_param_b, max(min_param_b, b));
	
	float p = 2.0*float(n) + 1.0;
	float y = 0.0;
	if (x<=a){
		y = b - b*pow(1.0-x/a, p);
	} else {
		y = b + (1.0-b)*pow((x-a)/(1.0-a), p);
	}
	return y;
}

void main() {
	vec2 st = gl_FragCoord.xy/RENDERSIZE;
	st = (st - offset)/scale;
	
	int n = int(n);
    float y = doubleOddPolynomialSeat(st.x, controlPoint.x, controlPoint.y, n);

    vec3 color = vec3(y);
    
    // Plot axes
    float yAxis;
    float xAxis;
    float axes;
    float axesThickness;
    vec3 axesColor;
    for (float n=-4.; n<5.; ++n) {
    	if (floor(n) == 0.) {
    		axesThickness = 0.003;
    		axesColor = vec3(0.4, 0.0, 0.5);
    	} else {
    		axesThickness = 0.002;
    		axesColor = vec3(0.0, 0.4, 0.5);
    	}
    	yAxis = plot(st.x, floor(n), axesThickness/scale);
		xAxis = plot(st.y, floor(n), axesThickness/scale);
    	axes = yAxis + xAxis;
    	color = (1.0-axes)*color+axes*axesColor;	
    }

    // Plot a line
    float pct = plot(st.y, y, 0.02/scale);
    color = (1.0-pct)*color+pct*vec3(0.0,1.0,0.0);

	gl_FragColor = vec4(color,1.0);
}