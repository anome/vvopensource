/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/WdfSDj by evvvvil.  \"Foldable double standards\" - Shader showdown practice session 010.\nLive coded on Twitch with 25 minutes time limit.\nPracticing live on TWITCH every Tuesdays around 21:00 UK time.\nhttps://www.twitch.tv/evvvvil_",
  "INPUTS" : [
    {
      "NAME" : "tijd",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 0,
      "LABEL" : "tijd",
      "MIN" : 0
    },
    {
      "NAME" : "een",
      "TYPE" : "float",
      "MAX" : 5,
      "DEFAULT" : 1,
      "MIN" : 0
    },
    {
      "NAME" : "twee",
      "TYPE" : "float",
      "MAX" : 5,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "drie",
      "TYPE" : "float",
      "MAX" : 100,
      "DEFAULT" : 20,
      "MIN" : 0
    },
    {
      "NAME" : "vier",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 1,
      "MIN" : 0
    },
    {
      "NAME" : "vijf",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "vijfplus",
      "TYPE" : "float",
      "MAX" : 1,
      "MIN" : 0
    }
  ],
  "ISFVSN" : "2"
}
*/


////////////////////////////////////////////////////////////////////////////////
//"Foldable double standards" - Shader Showdown practice session 010

// WHAT THE FUCK IS THE SHADER SHOWDOWN?
// The "Shader Showdown" is a demoscene live-coding shader battle competition.
// 2 coders battle for 25 minutes making a shader from memory on stage. 
// The audience votes for the winner by making noise or by voting on their phone.
// Winner goes through to the next round until the final where champion is crowned.
// Live coding shader software used is BONZOMATIC made by Gargaj from Conspiracy:
// https://github.com/Gargaj/Bonzomatic

// Every tuesdays around 21:00 UK time I practise live on TWITCH. This is the result of session 010.

// COME SEE LIVE CODING EVERY TUESDAYS HERE: https://www.twitch.tv/evvvvil_

// evvvvil / DESiRE demogroup

// "Fools running where angels fear to tread" - Alexander Pope

vec2 sc,e=vec2(.00035,-.00035);float t,tt,st,ct;vec3 np,op;//Some fucking globals, about as exciting as a flooded wellbeing yoga centre 
float bo(vec3 p,vec3 r){vec3 q=abs(p)-r;return max(max(q.x,q.y),q.z);}//box function stolen from UNC because UNC from QUITE always finds my shit jokes funny.
vec2 fb( vec3 p )//Fucking bits function which makes the fucking bit/piece it is a base shape which we clone and repeate to create the whole geometry in mp function
{
  vec2 h,t=vec2(bo(p,vec3(6,0.5,0.5)),5);  //Bunch of fucking mediocre boxes
  t.x=min(t.x,bo(abs(p)-vec3(3,0,0),vec3(0.8,0.5,100)));//more fucknig boxes
  h=vec2(bo(p,vec3(6.2,0.7,0.2)),3);//Dude this fucking box thing again? Yeah but with different material id, fucking cheer up yeah?
  h.x=min(h.x,bo(abs(p)-vec3(3,0,0),vec3(1,0.2,100)));  //Stacking boxes mengo
  h.x=min(h.x,length(abs(p)-vec3(7,0,0))-1.2-(vijf*vijfplus));//Oh look little fucking spheres on the sides? cute
  t=(t.x<h.x)?t:h;//Merge both shapes while retaining material id, a friendly and colourful handshake basically
  h=vec2(bo(p,vec3(0.5,(15.+sin(op.z*.4+tt*10.)*5.)*(1.*vier-ct),0.5)),6.);//Makes the spikes in scene 01
  t=(t.x<h.x)?t:h;//Merge both shapes while retaining material id, a friendly and colourful handshake basically
  t.x*=0.8; return t;
}
float noise(vec3 p){//Noise function stolen from Virgil who stole it from Shane who I assume understands this shit, unlike me who is too busy argueing about the poetry of football hooliganism
  vec3 ip=floor(p),s=vec3(7,157,113);
  p-=ip; vec4 h=vec4(0,s.yz,s.y+s.z)+dot(ip,s);
  p=p*p*(3.-2.*p);
  h=mix(fract(sin(h)*43758.5),fract(sin(h+s.x)*43758.5),p.x);
  h.xy=mix(h.xz,h.yw,p.y);
  return mix(h.x,h.y,p.z);
}
mat2 r2(float r){return mat2(cos(r),sin(r),-sin(r),cos(r));}//simple rotate function, it is useful as fuck and short. Bit like Muggsy Bogues in the 90's
vec2 mp( vec3 p ) //This is the main MAP function where all geometry is made/defined. It's centre stage broski, bit like a drunken Belgian monk in a hipster's shitty brewing party
{
  op=p;//Remember that orignal position broski? Yeah I agree, there was more individual freedom in the 90s, bring back smoking and football hooliganism
  p.xy*=r2(sin(p.z*0.2)*0.2+tt);//overall we're sort of twisting the fucker
  p.z=mod(p.z+tt*10.,20.)-10.;//modulo is fucking shit, avoid it but hey to make it infinite on one axis is alright broski, it's even suitable for vegans
  vec4 pp=vec4(p,1);//we make a new position which is vec4 to remember how much we scale it and apply that shit back to tweak domain scale and avoid artifact on line 61
  vec2 h,t=vec2(10000,0);//We start with number super high, just like me on any given good saturday afternoon
  for(int i=0;i<4;i++)//Pseudo fractal bullshit loop de loopo motherfucker
  {
    pp.xy*=r2(0.785*een*(2.*st));//Sometimes we rotate, sometimes we dont, depends on b animation variable
    pp.xyz=abs(pp.xyz)-mix(vec3(0,3,6),vec3(0,10,2),st);//Abs symetry clone geometry to get more, each iteration
    pp*=1.5;//Each iteration we make fractal piece smaller
    pp.xz*=r2(0.785*(1.*twee+st));//rotate the fucking piece
    h=fb(pp.xyz);h.x/=pp.w;//Finally each iteration we draw the piece of fractal
    t=(t.x<h.x)?t:h;//Merge this iteration fractal piece with rest, like building blocks but without the DMT smoking and throwing pieces at the neighbour in euphoria
  }
  np=pp.xyz;//Aye remember the fucking position for later on lighting and gloss map bitch
  p.xy*=r2(cos(p.z*1.5+tt*10.)*0.5+tt*5.);//create pos for those shitty spheres in scene 2
  h=vec2(length(p-vec3(cos(p.z*25.)*0.05+cos(p.x),0,0))-5.*st,6.);//Shitty sphere from scene 02 with nice twisty animation and frills
  h.x*=0.5;//Scale sphere distance field to avoid artifact, keep it shallow and beautiful
  t=(t.x<h.x)?t:h;//Merge the fucking fractal and spheres and shit
  return t;
}
vec2 tr( vec3 ro, vec3 rd,float near,float far,int it )
{
  vec2 h,t=vec2(near);//Near plane because we all started as annoying little shits yeah, and nah, your kids aren't cute
  for(int i=0;i<it;i++){//Main loop de loop 
    h=mp(ro+rd*t.x);//Marching forward like any good fascist army: without any care for culture theft
    if(h.x<.0001||t.x>far) break;//Don't let the bastards break you down! Fuck the system!
    t.x+=h.x;t.y=h.y;//Remember the postion and the material id? Because I can be a poncy interior designer too when I'v drunk enough Sherry
  }
  if(t.x>far) t.x=0.;//If we've gone to far then it's time to get on yer bike and get job
  return t;
}
void main() {



    vec2 uv = vec2(gl_FragCoord.x / RENDERSIZE.x, gl_FragCoord.y / RENDERSIZE.y);
    uv -= 0.5;
    uv /= vec2(RENDERSIZE.y / RENDERSIZE.x, 1);//boilerplate code to get uvs in BONZOMATIC live coding software i use.
    tt=mod(tijd,50.);//MAin time variable, it's modulo'ed to avoid ugly artifact. Holding time in my hand: playing god seems better thanspending two months in rehab.
	st=0.5+clamp(sin(tt*0.5),-0.5,0.5);//These are just aniimation variables used in mp or fb
  	ct=1.2+sin(tt*0.5);//Yeah this one too broski
    vec3 ro=vec3(20,0,cos(tt)*10.-20.+drie),//Ro=ray origin=camera position because everything is relative to a view point, even your wife's failed wellbeing yoga centre
    cw=normalize(vec3(0)-ro),cu=normalize(cross(cw,vec3(0,1,0))),cv=normalize(cross(cu,cw)),
    rd=mat3(cu,cv,cw)*normalize(vec3(uv,.5)),//rd=ray direction (where the camera is pointing), co=final color, fo=fog color
    co,fo,ld=normalize(vec3(.5,.3,-.1));//ld=light direction, because god wouldn't be much without good lighting and vfx
    co=fo=vec3(.04)*(1.-(length(uv)-.2));//By default the color fog color and it's a dark coloured vignette, because it's the colour of the sky in Loretta Lynn's "Fist city"
    sc=tr(ro,rd,0.1,50.,128);t=sc.x;//This is where we shoot the fucking rays to get the fucking scene. Like a soldier but with a pixel gun and less intentions to invade and pillage.
	
    if(t>0.){//If t>0 then we must have hit some geometry so let's fucking shade it. Grab an umbrella, it's like dinning at the beach
        //We hit some geometry so let's get the current position (po) and build some normals (no). You do the Maths while I light up Diogenes' lamp as he searches for an honest person.
        vec3 po=ro+rd*t,no=normalize(e.xyy*mp(po+e.xyy).x+e.yyx*mp(po+e.yyx).x+e.yxy*mp(po+e.yxy).x+e.xxx*mp(po+e.xxx).x),
        //LIGHTING MICRO ENGINE BROSKI 
        al=vec3(0,.2,.4);//Albedo is base colour.
        if(sc.y<5.)  al=vec3(0.9);//Change colour depending on material id, it's like a painting/decorating job but without the football banter, so it's kinda shit
        //More colour change, color is gradient over y axis because Roy Hodgson is still in charge of Crystal Palace and they should be staying up this year
        if(sc.y>5.)  al=mix(vec3(0.9,0.9,0.9),vec3(1,.5,0),0.5+0.5*sin(op.z*0.4+tt*10.));
        no*=(1.+.6*ceil(cos(np*0.5)));no=normalize(no);//TRICK to add more detail to geometry by tweaking the normals don't forget to normalize after though, "Honky Tonk girls better know not to break the rules!"
        float dif=max(0.,dot(no,ld)),//dif=diffuse because i ain't got time to cook torrance
        aor=t/50.,ao=exp2(-2.*pow(max(0.,1.-mp(po+no*aor).x/aor),2.)),//aor =amibent occlusion range, ao = ambient occlusion
        fr=pow(1.+dot(no,rd),4.),//Fr=fresnel which adds reflections on edges to composite geometry better, yeah could be reflected, but who gives a shit? Anyways just like your ex, it doesn't do much.
        spo= exp2(1.+3.*noise(np/vec3(1,2,4)));//TRICK making a gloss map from a 3d noise function is a thing of fucking beauty
        vec3 sss=vec3(0.5)*smoothstep(0.,1.,mp(po+ld*0.4).x/0.4),//sss=subsurface scatterring made by tekf from the wax shader, big up tekf! https://www.shadertoy.com/view/lslXRj
        sp=vec3(0.5)*pow(max(dot(reflect(-ld,no),-rd),0.),spo);//Sp=specualr, sotlen from Shane and it's better than walking into cold puddle of water in the bathroom while only wearing socks
        co=mix(sp+al*(.8*ao+0.2)*(dif+sss),fo,fr);//Building the final lighting result, compressing the fuck outta everything above into an RGB shit sandwich
        co=mix(co,fo,1.-exp(-.00003*t*t*t));//Fog soften things, but it won't save your failed marriage, God will though! Grab a bible and put on some lingerie.
    }
    gl_FragColor = vec4(pow(co,vec3(0.45)),1);//Cheap tone mapping, even cheaper than a date with your future ex girlfriend
}
