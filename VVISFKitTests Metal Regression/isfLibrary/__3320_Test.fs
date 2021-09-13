/*{
	"CREDIT": "by colin",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	]
}*/

///Shape Functions///
vec4 colorAssign(vec4 color, float shapeSDF, float shapeStrokeSDF, vec3 fillColor, vec3 strokeColor, bool fillState ){
	//color accumulator
	vec4 c = vec4(0.0);	
	//fill assignment
	c.rgb += shapeSDF * fillColor.rgb;	
	//stroke assignment
	c.rgb += shapeStrokeSDF * strokeColor.rgb;
	//conditional for no fill
	fillState == false ? c.a = shapeStrokeSDF : c.a = shapeSDF + shapeStrokeSDF;
	return c;
}



vec4 drawShape(int shapeType, vec2 screenSpace, vec2 pos, vec2 size, float rounding, bool fillState, vec3 fillColor, vec3 strokeColor, float strokeSize){
	
	//accumulator
	vec4 color = vec4(0.0);
	
	//average width and height
	float floatSize = ((size.x+size.y)/2.0)*.2;
	
	if (shapeType ==0){
		
		//circle distance function
		float circle = length(screenSpace-pos);
		//fill and stroke
		float ellipseFillStep = step(circle+strokeSize*.5,floatSize*3.0);
		float ellipseStrokeStep = step(floatSize*3.0, circle+strokeSize * 0.5) - step(floatSize*3.0, circle - strokeSize * 0.5);
		//assign the color
		color = colorAssign(color,ellipseFillStep,ellipseStrokeStep,fillColor,strokeColor,fillState);

	} else if (shapeType ==1){
		
		//rectangle distance function
		vec2 dist = abs(screenSpace-pos)-(size-vec2(rounding));
		float rect = min(max(dist.x, dist.y),0.0) + length(max(dist,0.0))-rounding;
		//fill and stroke
		float rectFillStep = 1.0 - float(step(0.0,rect+strokeSize *.5));
		float rectStrokeStep = step(1.0 - float(step(0.0,rect+strokeSize *.5)), 0.0 ) -  step(1.0 - float(step(0.0,rect-strokeSize *.5)), 0.0 );	 
		//assign the color
		color = colorAssign(color,rectFillStep,rectStrokeStep,fillColor,strokeColor,fillState);

	} else if (shapeType ==2){
	
		//triangle distance function
		vec2 dist2 = abs(screenSpace-pos);
    	vec2 screenSpaceOffset = screenSpace-pos;
    	float tri = max(dist2.x * 0.866025 + screenSpaceOffset.y* 0.5, -screenSpaceOffset.y * 0.5) - floatSize * 0.5;
    	//fill and stroke
		float triangleFillStep = step(tri+strokeSize*.5,floatSize);
		float triangleStrokeStep = step(floatSize, tri+strokeSize * 0.5) - step(floatSize, tri - strokeSize * 0.5);
		//assign the color
		color = colorAssign(color,triangleFillStep,triangleStrokeStep,fillColor,strokeColor,fillState);
		
	} else if (shapeType ==3){

		//hex distance function
		vec2 dist2 = abs(screenSpace-pos);
		vec2 screenSpaceOffset = screenSpace-pos;
    	vec2 q = abs(dist2);
    	float hexagon = max(abs(q.y), q.x * 0.866025 + q.y * 0.5) - floatSize *1.5;//floatSize scaled up
        //fill and stroke	
		float hexFillStep = step(hexagon+strokeSize*.5,floatSize);
		float hexStrokeStep = step(floatSize, hexagon+strokeSize * 0.5) - step(floatSize, hexagon - strokeSize * 0.5);
		//assign the color
		color = colorAssign(color,hexFillStep,hexStrokeStep,fillColor,strokeColor,fillState);
		
	}else if (shapeType ==4){

		//poly distance function
		
		vec2 dist2 = abs(screenSpace-pos);
    	float a = atan(dist2.x, dist2.y) + 0.2;
    	float b = 6.28319 / floor(rounding);
   		float polygon = cos(floor(0.5 + a / b) * b - a) * length(pos) - floatSize;

        //fill and stroke	
		float polyFillStep = step(polygon+strokeSize*.5,floatSize);
		float polyStrokeStep = step(floatSize, polygon+strokeSize * 0.5) - step(floatSize, polygon - strokeSize * 0.5);
		//assign the color
		color = colorAssign(color,polyFillStep,polyStrokeStep,fillColor,strokeColor,fillState);
		
	}

	
return color;

}


// int shapeType, vec2 screenSpace, vec2 pos, vec2 size, float rounding, bool fillState, vec3 fillColor, vec3 strokeColor, float strokeSize
void main() {
	vec2 screenSpace = isf_FragNormCoord.xy;
	
	int shapeType = 0;
	vec2 pos = vec2(0.0);
	vec2 size = vec2(.5);
	float rounding = 0.0;
	bool fillState = true;
	vec3 fillColor = vec3(1.0,1.0,0.0);
	vec3 strokeColor = vec3(0.0,1.0,1.0);
	float strokeSize = 1.0;
	
	vec4 color = drawShape(
		shapeType,
		screenSpace,
		pos,
		size,
		rounding,
		fillState,
		fillColor,
		strokeColor,
		strokeSize
	);
	
	gl_FragColor = vec4(color);
}