varying vec2		texOffsets[3];

varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;

const float radius = 10.0;

void main(void)	{
	//	load the main shader stuff
	vv_vertShaderInit();
	
	
	//	Edges then bloom
	if (PASSINDEX==0)	{
		vec2 texc = vec2(vv_FragNormCoord[0],vv_FragNormCoord[1]);
		vec2 d = 1.0/RENDERSIZE;
	
		left_coord = clamp(vec2(texc.xy + vec2(-d.x , 0)),0.0,1.0);
		right_coord = clamp(vec2(texc.xy + vec2(d.x , 0)),0.0,1.0);
		above_coord = clamp(vec2(texc.xy + vec2(0,d.y)),0.0,1.0);
		below_coord = clamp(vec2(texc.xy + vec2(0,-d.y)),0.0,1.0);

		lefta_coord = clamp(vec2(texc.xy + vec2(-d.x , d.x)),0.0,1.0);
		righta_coord = clamp(vec2(texc.xy + vec2(d.x , d.x)),0.0,1.0);
		leftb_coord = clamp(vec2(texc.xy + vec2(-d.x , -d.x)),0.0,1.0);
		rightb_coord = clamp(vec2(texc.xy + vec2(d.x , -d.x)),0.0,1.0);
	}
	else if (PASSINDEX==1 || PASSINDEX==3 || PASSINDEX==5 || PASSINDEX==7 || PASSINDEX==9)	{
		float		pixelWidth = 1.0/RENDERSIZE[0]*radius;
		if (PASSINDEX >= 2)
			pixelWidth *= .7;
		else if (PASSINDEX >= 6)
			pixelWidth *= 1.0;
		texOffsets[0] = vv_FragNormCoord;
		texOffsets[1] = clamp(vec2(vv_FragNormCoord[0]-pixelWidth, vv_FragNormCoord[1]),0.0,1.0);
		texOffsets[2] = clamp(vec2(vv_FragNormCoord[0]+pixelWidth, vv_FragNormCoord[1]),0.0,1.0);
	}
	else if (PASSINDEX==10 || PASSINDEX==2 || PASSINDEX==4 || PASSINDEX==6 || PASSINDEX==8)	{
		float		pixelHeight = 1.0/RENDERSIZE[1]*radius;
		if (PASSINDEX >= 3)
			pixelHeight *= .7;
		else if (PASSINDEX >= 6)
			pixelHeight *= 1.0;
		texOffsets[0] = vv_FragNormCoord;
		texOffsets[1] = clamp(vec2(vv_FragNormCoord[0], vv_FragNormCoord[1]-pixelHeight),0.0,1.0);
		texOffsets[2] = clamp(vec2(vv_FragNormCoord[0], vv_FragNormCoord[1]+pixelHeight),0.0,1.0);
	}
}
