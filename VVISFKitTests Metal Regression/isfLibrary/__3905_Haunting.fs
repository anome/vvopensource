/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/ts23zW by evvvvil.  \"The faces haunting Sarah Connor\" - Shader showdown practice session 005.\nLive coded on Twitch in 25 minutes from memory.\nPracticing live on TWITCH every Tuesdays around 20:30 UK time.\nhttps://www.twitch.tv/evvvvil_\n",
  "INPUTS" : [

  ]
}
*/


////////////////////////////////////////////////////////////////////////////////
//"The faces haunting Sarah Connor" - Shader Showdown practice session 005

// WHAT THE FUCK IS THE SHADER SHOWDOWN?
// The "Shader Showdown" is a demoscene live-coding shader battle competition.
// 2 coders battle for 25 minutes making a shader from memory on stage. 
// The audience votes for the winner by making noise or by voting on their phone.
// Winner goes through to the next round until the final where champion is crowned.
// Live coding shader software used is BONZOMATIC made by Gargaj from Conspiracy:
// https://github.com/Gargaj/Bonzomatic

// Every tuesdays around 20:30 UK time I practise live on TWITCH.
// This is the result of session 005. 

// COME SEE LIVE CODING EVERY TUESDAYS HERE: https://www.twitch.tv/evvvvil_

// evvvvil / DESiRE demogroup

vec2 sc,e=vec2(.00035,-.00035);float t,tt,b,bb;vec3 np; //Global fucking variables
//Cheap fucking box function (stolen from that place called the internet)
float bo(vec3 p,vec3 r){vec3 q=abs(p)-r;return max(max(q.x,q.y),q.z);}

//Simple 2d rotate function, nothing to see here, move along, find the girl surfing the wormhole
mat2 r2(float r) {return mat2(cos(r),sin(r),-sin(r),cos(r));}

//Fucking bits function which makes the the fucking bit/piece: a bunch of thin rectangles around a bunch of cubes
//Just like Tottenham FC's trophy cabinet: it is "small and modest".
vec2 fb( vec3 p )
{
  vec2 h,t=vec2(bo(abs(p)-vec3(3,0,0),vec3(1,.8,1)),5);
  h=vec2(bo(abs(p)-vec3(2,0,0),vec3(1.2,.8,1.2)),3);
  h.x=min(bo(abs(abs(p)-vec3(0,.6,.6))-vec3(0,.3,.3),vec3(3,.1,.1)),h.x);
  t=(t.x<h.x)?t:h; return vec2(t.x*.5,t.y);
}
//IQ/Shane's compact 3d noise function. Because I have more memory than math (despite "smoking away huge chunks of memory")
float noise(vec3 p){
    vec3 ip=floor(p),s=vec3(7,157,113);p-=ip;
    vec4 h=vec4(0,s.yz,s.y+s.z)+dot(ip,s);
    p=p*p*(3.-2.*p);
    h=mix(fract(sin(h)*43758.5),fract(sin(h+s.x)*43758.5),p.x);
    h.xy=mix(h.xz,h.yw,p.y);return mix(h.x,h.y,p.z);
}
//Map function / scene / Where the geometry is made. This fucker is like Richard Pryor after too much freebase.(on fire)
vec2 mp( vec3 p )
{
    //This line is where the twist of face morph animation happen
    //do rotation along xz axis using sin(p.y) as amount to rotate and pushing movement down with tt(time)
    p.xz*=r2(-sin(p.y*0.2+tt*10.)*4.*(.5-abs(b-.5)));
    //This just rotates the face
    p.xz*=r2(.785*2.*bb);
    //Here starts the modelling technique: taking a piece of geometry modelled in fb function above and 
    //make it look like face by first abs symetry clone the piece then rotate it 
    //and then pull it apart and creates eye hole and cheeks and brows
    //(explained in more detail below per line)    
  np=p;
  for(int i=0;i<6;i++){
    //THIS LINE IS WHERE MAGIC IS (took hours of tweaking the three magic numbers below)
    np=abs(np)-mix(vec3(1,2,0),vec3(0,3,1.7),b);
    //I was thinking umm if i rotate twice should get some sort of 45 degrees jaws?
    //a combo of luck and trial and error, I didnt really know how to make a face I just knew when to stop.
    np.yz*=r2(.785*float(i));
    np.xz*=r2(.785*float(i)*.5);
    //This is really sweet trick to pull geometry apart, 
    //sort of creating holes and at same time create organic looking jaws and brows with the sin(p.y)
    np-=.3*sin(p.y)*1.5;
  }
  vec2 h,t=fb(np);
  //NOT over yet though, amazing how much things look like a face when you put some eyes
  //and something in mouth to hide geometry that stops it looking like a mouth; it's not about adding geometry it's about hiding some with some
  h=vec2(length(abs(p-vec3(0,4.-b,0))-vec3(4,0,3.-b))-2.5+b,6);
  h.x=min(length(p-vec3(0,-3,0))-5.,h.x)*.7;
  t=(t.x<h.x)?t:h;//This mixes geometry together like opUnion but with material id and size coded
  return t;      
}
//yx aka lunaSorcery's sick as fuck way to get a pie
#define pi acos(-1.)
//Main raymarching loop with material ID flex, because Tottenham FC ain't gonna win nothing this year again!
vec2 tr( vec3 ro, vec3 rd )
{
  vec2 h,t=vec2(.1);
  for(int i=0;i<128;i++){
    h=mp(ro+rd*t.x);
    if(h.x<.0001||t.x>60.) break;
    t.x+=h.x;t.y=h.y; //This extra line passes the material id (t.y)    
  } if(t.x>60.) t.x=0.;
  return t;
}
void main() {

    

    vec2 uv = vec2(gl_FragCoord.x / RENDERSIZE.x, gl_FragCoord.y / RENDERSIZE.y);
    uv -= 0.5; uv /= vec2(RENDERSIZE.y / RENDERSIZE.x, 1);//Boilerplate code building uvs by default in BONZOMATIC
    //Modulo time because I fucking hate noisey sins or whatever the fuck glitches after certain time, hey? (stops it all getting fucking noisy)  
    tt=mod(TIME,100.);
    bb=ceil(tt/pi)+2.*clamp(fract(tt/pi),0.,.5); //This sweet trick to rotate stop, keep rotating into infinity while sotpping sometimes, etc
  	b=0.5+clamp(sin(tt+2.5),-.5,.5); //this variable is used to animate a lot, like to do the lerp/mix. stops/starts/stop/starts from 0 to 1
    
   vec3 ro=vec3(0,sin(tt)*10.,-20.+cos(tt*2.+sin(tt*2.))*5.), //Camera ro=ray origin, rd=ray direction, co=final color, fo=fog, ld=light direction
    cw=normalize(vec3(0)-ro),cu=normalize(cross(cw,vec3(0,1,0))),cv=normalize(cross(cu,cw)),
    rd=mat3(cu,cv,cw)*normalize(vec3(uv,.5)),co,fo,ld=vec3(.3,.5,-.5);
    co=fo=vec3(.5)*(1.-(length(uv)-.2));//Setting up default background colour and fog colour some shit vignette thing broski
    //Grabbing the fucking scene by shooting fuckin' rays, because we all wanna gather rays for a living
    sc=tr(ro,rd);t=sc.x;  
  if(t>0.){
    //We hit some geometry so let's get the current position (po) and build some normals (no). You do the Maths while I get some fucking beer.
    vec3 po=ro+rd*t,no=normalize(e.xyy*mp(po+e.xyy).x+e.yxy*mp(po+e.yxy).x+e.yyx*mp(po+e.yyx).x+e.xxx*mp(po+e.xxx).x),    
    
    //LIGHTING MICRO ENGINE BROSKI 
    //Default albedo is red because punk rock and you're not hot enough (al=albedo)
    al=vec3(.7,.1,.1);
	//THIS TRICK! Adds some detail to geometry by tweaking the normals. not that much diff this week but still a nice touch.
	no*=(1.+.6*ceil(sin(np*2.)));no=normalize(no);
    //Different material id? Changeacolourooo... It's all very black and white, makes the red a little punchindaface
    if(sc.y<5.) al=vec3(0);
    if(sc.y>5.) al=vec3(1);
    //dif = diffuse because I dont have time to cook torrance
    float dif=max(0.,dot(no,ld)),
    //ao = ambient occlusion, aor = ambient occlusion range
    aor=t/50.,ao=exp2(-2.*pow(max(0.,1.-mp(po+no*aor).x/aor),2.)),
    //spo=specular power, THIS TRICK is some fucking sweet gloss map generated from recursive noise function. Fuck yeah broski!
    spo=exp2(1.+3.*noise(np/vec3(.4,.8,.8)+noise((np+1.)/vec3(.2,.4,.4)))),
    //Fresnel blends the geometry in the background with some sort of gradient edge reflection colouring mother fucker
    fresnel=pow(1.+dot(no,rd),4.); // yeah i know it should be reflected but i don't give a shit broski and the background is a vignette, so fuck you
    //Fake sub surface fucking scattering, sort of reverse ambient occlusion trick from tekf, big up tekf! https://www.shadertoy.com/view/lslXRj
    vec3 sss=vec3(.5)*smoothstep(0.,1.,mp(po+ld*0.4).x/0.4),
    //spec=specular with the spo gloss map above, yeah broski, it's a thing of fucking beauty.
    spec=vec3(1.5)*pow(max(dot(reflect(-ld,no),-rd),0.),spo);
    co=mix(spec+al*(0.8*ao+0.2)*(dif+sss),fo,min(fresnel,0.5));//Ultimate final lighting result
    co=mix(co,fo,1.-exp(-.00002*t*t*t));//Add some fucking fog to blend it even more, get cosy, stay on your branch.
  }
  //Add some sort of tone mapping... but just like a Hipster's beards and boating shoes: it's not the real thing
  gl_FragColor = vec4(pow(co,vec3(0.45)),1);
}
