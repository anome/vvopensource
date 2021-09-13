varying vec2		texOffsets[6];

/*

         X   1   1 
     1   1   1
         1

       (1/8)

*/

void main(void)	{
	isf_vertShaderInit();
	vec2 texc = vec2(isf_FragNormCoord[0],isf_FragNormCoord[1]);
	vec2 d = 1.0/RENDERSIZE;
	
	//	Using an Atkinson kernel!
	texOffsets[0] = clamp(vec2(texc.xy + vec2(-d.x , 0)),0.0,1.0);
	texOffsets[1] = clamp(vec2(texc.xy + vec2(-2.0 * d.x , 0)),0.0,1.0);
	
	texOffsets[2] = clamp(vec2(texc.xy + vec2(-d.x , d.y)),0.0,1.0);
	texOffsets[3] = clamp(vec2(texc.xy + vec2(0.0 , d.y)),0.0,1.0);
	texOffsets[4] = clamp(vec2(texc.xy + vec2(d.x , d.y)),0.0,1.0);
	
	texOffsets[5] = clamp(vec2(texc.xy + vec2(0.0 , 2.0*d.y)),0.0,1.0);

}