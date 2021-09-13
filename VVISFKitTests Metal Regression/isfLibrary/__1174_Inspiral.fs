/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/3s23Dc by evvvvil.  \"Infinite Spinal Squid\" - Shader showdown practice session 007.\nLive coded on Twitch with 25 minutes time limit.\nPracticing live on TWITCH every Tuesdays around 20:30 UK time.\nhttps://www.twitch.tv/evvvvil_\n",
  "INPUTS" : [
    {
      "NAME" : "Een",
      "TYPE" : "float",
      "MAX" : 100,
      "DEFAULT" : 0,
      "LABEL" : "Een",
      "MIN" : 0
    },
    {
      "NAME" : "twee",
      "TYPE" : "float",
      "MAX" : 2.5,
      "DEFAULT" : 0,
      "LABEL" : "Lines",
      "MIN" : -2.5
    },
    {
      "NAME" : "drie",
      "TYPE" : "float",
      "MAX" : 4,
      "DEFAULT" : 0.20000000000000001,
      "LABEL" : "drie",
      "MIN" : -1
    },
    {
      "NAME" : "vier",
      "TYPE" : "float",
      "MAX" : 0.050000000000000003,
      "DEFAULT" : 0.20000000000000001,
      "LABEL" : "vier",
      "MIN" : -5
    },
    {
      "NAME" : "vijf",
      "TYPE" : "float",
      "MAX" : 0.10000000000000001,
      "DEFAULT" : 0,
      "LABEL" : "vijf",
      "MIN" : 0
    },
    {
      "NAME" : "zes",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.01,
      "LABEL" : "zes",
      "MIN" : 0
    },
    {
      "NAME" : "zeven",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 0.10000000000000001,
      "LABEL" : "zeven",
      "MIN" : 0
    },
    {
      "NAME" : "acht",
      "TYPE" : "float",
      "MAX" : 3.5,
      "DEFAULT" : 1,
      "LABEL" : "acht",
      "MIN" : -2
    },
    {
      "NAME" : "negen",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 1,
      "LABEL" : "negen",
      "MIN" : 0
    },
    {
      "NAME" : "tien",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 1,
      "LABEL" : "tien",
      "MIN" : 0
    },
    {
      "NAME" : "Maal",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 1,
      "LABEL" : "Maal",
      "MIN" : 1
    },
    {
      "NAME" : "Turn",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 1,
      "LABEL" : "Turn",
      "MIN" : 1
    }
  ],
  "ISFVSN" : "2"
}
*/


////////////////////////////////////////////////////////////////////////////////
//"Infinite Spinal Squid" - Shader Showdown practice session 007

// WHAT THE FUCK IS THE SHADER SHOWDOWN?
// The "Shader Showdown" is a demoscene live-coding shader battle competition.
// 2 coders battle for 25 minutes making a shader from memory on stage. 
// The audience votes for the winner by making noise or by voting on their phone.
// Winner goes through to the next round until the final where champion is crowned.
// Live coding shader software used is BONZOMATIC made by Gargaj from Conspiracy:
// https://github.com/Gargaj/Bonzomatic

// Every tuesdays around 20:30 UK time I practise live on TWITCH.
// This is the result of session 007. 

// COME SEE LIVE CODING EVERY TUESDAYS HERE: https://www.twitch.tv/evvvvil_

// evvvvil / DESiRE demogroup

// "A squid eating dough in a polyethylene bag is fast and bulbous." - Captain Beefheart

vec2 sc,e=vec2(0.00035,-.00035);float t,tt,att,su;vec3 np,pp; //Global fucking variables because we're internafuckingtional
//Cheap fucking box function (actually stollen from UNC I think, not picking IQ's pockets on this one)
float bo(vec3 p,vec3 r){vec3 q=abs(p)-r;return max(max(q.x,q.y),q.z);}//Still stolen from someone though, come on, I'm too cool to be that clever.

//Simple 2d rotate function, nothing to see here, move along, find the angry mother and introduce her to DMT
mat2 r2(float r) {return mat2(cos(r),sin(r),-sin(r),cos(r));}

//Fucking bits function which makes the fucking bit/piece it is a base shape which we clone and repeate to create the whole geometry in mp function
vec2 fb( vec3 p )//Base piece look like a train track, couple of parralel rectangles with some perpendicular ones in middle
{
  vec2 h,t=vec2(bo(abs(p)-vec3(2,0,0),vec3(0.3,0.3,10)),5); //t is the blue bits: 2 rectangles symetry cloned along the x axis
  h=vec2(1000,3);//h is the black bits, we make it really high, just like me, so we can add shapes by using min function
  for(int i=0;i<6;i++) {
    h.x=min(h.x,bo(abs(p)-vec3(0,0,0.5*float(i)),vec3(2,0.1,0.1)));//keep adding one black rectangle per iteration and shift it
    h.x=min(h.x,bo(abs(p)-vec3(2,0,0.5*float(i)),vec3(0.2,0.5,0.2)));//keep adding one smaller black rectangle per iteration and shift it
  }
  t.x=min(t.x,0.8*(length(p-vec3(0,0,6))-1.7));//this creates a sphere in between blue bits
  t=(t.x<h.x)?t:h;
  t.x*=0.5;//This make the shape more defined so that we don't get artifact (glitches that look like shit)
  return t;
}
//Map function / scene / Where the geometry is made.
vec2 mp( vec3 p )//We make a whole complex geometry based on one single piece defined in fb
{//We do that by fucking with the position p in a new variable np(new position) then we pass np to fb
	p.xy*=r2(sin(p.z*.2)*.5+tt*.5);//This does an overall twist and constant rotation 
  	np=p;  //Told you we were gonna fuck with p in a new variable np, take fucking notes broski, this isn't your girlfriend's shitty yoga class
  	att=length(p-vec3(0,0,sin(tt*2.)*20.))-5.;//We create reverse-attractor, it's a sphere that pushes the geometry out
  	np.z=mod(p.z+tt*10.,15.)-7.5*acht; //this make the geometry infinite along the z axis, because, yeah you guess right, Einstein, it's a fucking infinite squid
  	for(int i=0;i<3;i++){
    	np=abs(np)-vec3(0.2,0.2+att*drie+Maal,0);//every loop iteration we push geometry out on x axis and depending on attractor on y axis
    	np.xy*=r2((cos(np.z*vier*float(i))));//rotaroo the fuckeroo: we rotate  a bit more each iteration, making it more interesting
  	}
  	vec2 h,t=fb(np);//Now we finished fucking with p we pass it through fb to create the whole geom from one "fucking bit"

  	pp=abs(p)-vec3(3.2,1.+sin(p.z*0.2),0)-att*twee;//we create one more new position this time called pp and it's gonnu be for the splines + cubes
  	pp.z=mod(pp.z-tt*10.,4.)-2.;//make this pp splines and cube infiintie on z axis again
  	
  	h=vec2(bo(pp,vec3(0.2+att*vijf)),60);//h is gonna be the hollow cubes - one box minus one sphere
 	h.x=max(-(length(pp)-(0.3+att*0.03)),h.x);//boolean operation removes sphere from cube
  	h.x*=0.7;//make them cubes more defined so we dont get artifacts
  	t=(t.x<h.x)?t:h;//This mixes the squid inner geometry with the cubes
  	h=vec2(bo(pp,vec3(zeven+att*0.01,0.1+att*zes,30)),5);//Here, broh, the fucking splines, yeah, stick, around, it, all
  	h.x*=0.8;//more definition less fucking artifact, more pumped pecs, more lasting impression on girls
	t=(t.x<h.x)?t:h;//mixes rest with the splines
	return t;
}

//Main raymarching loop with material ID flex, because driving on the wrong side of the motorway is more fun while sniffing glue!
vec2 tr( vec3 ro,vec3 rd )
{
	vec2 h,t=vec2(0.1);//0.1 is our near plane
    for(int i=0;i<128;i++){
        h=mp(ro+rd*t.x);//get result of running map function at this ray pos
        if(h.x<.0001||t.x>50.) break;//Get out early if we hit geom (<precision which is 0.0001) or we hit far plane (t.x>50)
        t.x+=h.x;t.y=h.y;//t.y=h.y passes the material ID
    }
    if(t.x>50.) t.x=0.;//if we hit far plane then make result 0 to do "some" optimization
    return t;//always return this return that, you're a fucking broken record you know that?
}

void main() {

    

    vec2 uv = vec2(gl_FragCoord.x / RENDERSIZE.x, gl_FragCoord.y / RENDERSIZE.y);
    uv -= 0.5; uv /= vec2(RENDERSIZE.y / RENDERSIZE.x, 1);//Boilerplate code building uvs by default in BONZOMATIC
    //Modulo time because I fucking hate noisey sins or whatever the fuck glitches after certain time, hey? (stops it all getting fucking noisy)  
    tt=negen*Turn;
    
   	vec3 ro=vec3(14,7.*tien+sin(tt*2.)*15.,5.+cos(tt)*Een), //Camera ro=ray origin, rd=ray direction, co=final color, fo=fog, ld=light direction
    cw=normalize(vec3(0.)-ro),cu=normalize(cross(cw,vec3(0,1,0))),cv=normalize(cross(cu,cw)),
    rd=mat3(cu,cv,cw)*normalize(vec3(uv,.5)),co,fo,bk,ld=normalize(vec3(0.2,.2,-.5));//co=final color, fo=fogcolor, bk=backgroundColorm ld=light direction
    su=clamp(dot(ld,rd),0.,1.);//Su=sun, feels good to play god again but fucking hell Pedro I didn't buy that glue for that.
    bk=vec3(1,.5,0)*pow(su,4.)+vec3(0.5,.6,.6)-rd.y*0.4;//bk=background mix of sky + sun. Playing god again, fuck yeah there are no side effects to this drug.
    co=fo=bk; //setting up default color as fog and background color
	sc=tr(ro,rd);t=sc.x;//sc=scene. We get the scene by shooting rays, big one this yeah, bigger than your ass look in those jeans. 
  if(t>0.){
    //We hit some geometry so let's get the current position (po) and build some normals (no). You do the Maths while I serenade them fuckin' angels.
    vec3 po=ro+rd*t,no=normalize(e.xyy*mp(po+e.xyy).x+e.yxy*mp(po+e.yxy).x+e.yyx*mp(po+e.yyx).x+e.xxx*mp(po+e.xxx).x),    
    
    //LIGHTING MICRO ENGINE BROSKI 
    //Default albedo is blue with gradient depending on distance to attractor. (al=albedo)
    al=vec3(0,0.5+att*0.02,1);  
	//Different material id? Changeacolourooo... It's all very black and white, like when I'm right and you're wrong
    if(sc.y<5.) al=vec3(0);
    if(sc.y>5.) al=vec3(1);
    float dif=max(0.,dot(no,ld)),//dif = diffuse because I dont have time to cook torrance
    //ao = ambient occlusion, aor = ambient occlusion range
    aor=t/50.,ao=exp2(-2.*pow(max(0.,1.-mp(po+no*aor).x/aor),2.)),    
    //Fresnel blends the geometry in the background with some sort of gradient edge reflection colouring mother fucker
    fre=pow(1.+dot(no,rd),4.); // yeah  maybe it should be reflected but who gives a shit broski, if you do then fuck you
    //Fake sub surface scattering, sort of reverse ambient occlusion trick from tekf, big up tekf! https://www.shadertoy.com/view/lslXRj
    vec3 sss=vec3(.5)*smoothstep(0.,1.,mp(po+ld*0.4).x/0.4),
    spec=vec3(.5)*pow(max(dot(reflect(-ld,no),-rd),0.),10.);//specular by shane. Even better than the feeling of stroking used tea bags
    co=mix(spec+al*(.2+.8*ao)*(dif+sss),bk,min(fre,0.5));//Final result of color, crunching everything into an RGB shit sandwich
	co+=0.5*vec3(1,.5,0)*pow(su,3.);//Post processing sunglare effect: much better than being slapped across the face with a fish
    co=mix(co,fo,1.-exp(-.00003*t*t*t));//Fog is lovely reminds me of dipping my hand in warm honey while watching angels get naked
  }  	
  //Add some sort of tone mapping... but it's fake like a Hipster's craft beer trying to be a Belgian brew.
  gl_FragColor = vec4(pow(co,vec3(0.45)),1);
}
