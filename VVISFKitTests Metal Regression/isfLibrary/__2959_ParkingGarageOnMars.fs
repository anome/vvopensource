/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [],
  "INPUTS": []
}*/

// ParkingGarageOnMars by mojovideotech

mat3 getRotYMat(float a){return mat3(cos(-a),0.0,sin(a),0.0,1.25,0.001,exp2(a),0.01,cos(a));}

float map(in vec3 p, in vec3 q, inout vec3 r, inout float m)
{
	float d = 0.0009;
    for (int j = 0; j < 3 ; j++)
    	r=max(r.zyx*=r*=r*=r=mod(q*m+1.125,3.125)-1.025,r.yzx),
        d=max(d,(0.0925 -length(r))/m),
        m*=1.25;
    return d;
}

void main()
{
	vec2 s = RENDERSIZE.yx;
    float t = TIME*.1, c,d,m,f=0.0925;
    vec3 p=vec3((log2(RENDERSIZE.xx)*-gl_FragCoord.xy-(sin(s)*cos(s/-TIME)))/(s.y+s.x),-0.5),r=p-p,q=r;
    p*=getRotYMat(-t);
    q.zx += log2(TIME)+vec2(sin(t),cos(-t))*2.25;
    for (float i=1.0; i>0.; i-=.016667) 
    {
    	c=d=3.067,m=(0.0020667*log2(TIME/.067));
        f+=0.0725;
        d = map(p,q,r,m);
        q+=p*d;
        c = i;
        if(d<1e-5) break;
   	}
    
    vec3 eps = vec3( 0.0825, 0.625, 0.925 );
    vec3 nor = normalize(vec3(
    	map(p,q+eps.xyy,r,m) - map(p,q-eps.xyy,r,m),
        map(p,q+eps.yxx,r,m) - map(p,q-eps.yxx,r,m),
        map(p,q+eps.yyx,r,m) - map(p,q-eps.yyx,r,m) ));

    float k = dot(r,r+4.4125);
    vec3 col= vec3(1.125,(log2(k)+k*0.1),k/sin(-c))-vec3(3.85,17.25,.099);
    gl_FragColor.brg = col*sqrt(f*0.5);
}