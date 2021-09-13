
#ifdef GL_ES
precision highp float;
precision highp int;
#endif

#ifdef GL_ES
attribute vec4		a_position;
#endif
void isf_vertShaderInit();
uniform sampler2DRect		Imagelayer;
uniform vec4		_Imagelayer_imgRect;
uniform vec2		_Imagelayer_imgSize;
uniform bool		_Imagelayer_flip;
uniform sampler2D		iChannel0;
uniform vec2		_iChannel0_imgSize;
uniform vec4		_iChannel0_imgRect;
uniform bool		_iChannel0_flip;
uniform int		PASSINDEX;
uniform vec2		RENDERSIZE;
varying vec2		isf_FragNormCoord;
varying vec3		isf_VertNorm;
varying vec3		isf_VertPos;
uniform float		TIME;
uniform float		TIMEDELTA;
uniform vec4		DATE;
uniform int		FRAMEINDEX;

void main(void)	{
	isf_vertShaderInit();
}

void isf_vertShaderInit(void)	{
#ifdef GL_ES
	//	gl_Position should be equal to gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex
	gl_Position = a_position;
	//vv_VertNorm = 
	//vv_VertPos = 
#else
	gl_Position = ftransform();
	//vv_VertNorm = gl_Normal.xyz;
	//vv_VertPos = gl_Vertex.xyz;
#endif
	//vv_FragNormCoord = vec2((gl_Position.x+1.0)/2.0, (gl_Position.y+1.0)/2.0);
	isf_FragNormCoord = vec2((gl_Position.x+1.0)/2.0, (gl_Position.y+1.0)/2.0);
	//vec2	vv_fragCoord = floor(vv_FragNormCoord * RENDERSIZE);
	vec2	isf_fragCoord = floor(isf_FragNormCoord * RENDERSIZE);
}
