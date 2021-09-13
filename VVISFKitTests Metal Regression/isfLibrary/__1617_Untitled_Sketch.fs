/*{
	"CREDIT": "by thedantheman",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	]
}*/

#define F 
void main()
{
    vec4 p=gl_FragCoord/RENDERSIZE.xyxx-.5,d=p*.5,t;
    p.w += TIME*20.;d.y-=.2;
    for(float i=1.7;i>=0.;i-=.002)
    {
        float s=1.;t=d-d;
        t+=IMG_PIXEL(inputImage,.3+p.xw*s/6e3,-99.)/s;s+=s;
        t+=IMG_PIXEL(inputImage,.3+p.xw*s/6e3,-99.)/s;s+=s;
        t+=IMG_PIXEL(inputImage,.3+p.xw*s/6e3,-99.)/s;s+=s;
        t+=IMG_PIXEL(inputImage,.3+p.xw*s/6e3,-99.)/s;s+=s;
        t+=IMG_PIXEL(inputImage,.3+p.xw*s/6e3,-99.)/s;s+=s;
        t+=IMG_PIXEL(inputImage,.3+p.xw*s/6e3,-99.)/s;s+=s;
		gl_FragColor = vec4(1,.9,.8,1)+d.x-t*i;
        if(t.x>p.y*.01+1.3)break;
        p += d;
    }
}
