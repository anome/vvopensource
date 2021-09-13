/*{
	"CREDIT": "by isak.burstrom",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	]
}*/


vec3 light = normalize(vec3(1.0,0.9,0.3));

float sdSphere(vec3 p, float s) {
  return length(p)-s;
}

float sdBox(vec3 p, float s) {
  vec3 d = abs(p) - vec3(s);
  return min(max(d.x, max(d.y, d.z)), 0.0) + length(max(d, 0.0));
}

vec3 map(in vec3 p) {
  float d = sdBox(p, 0.2);
  return vec3(d, 0.0, 0.0);
}

vec3 intersexct(in vec3 ro, in vec3 rd) {
  float t = 0.0;
  for (int i = 0; i < 10; i++) {
    vec3 h = map(ro + rd*t);
    if ( h.x<0.001 ) {
      return vec3(t,h.yz);
    }
    t += h.x;
  }
  return vec3(-1.0);
}


vec4 intersect( in vec3 ro, in vec3 rd )
{
    float t = 0.0;
    vec4 res = vec4(-1.0);
	vec4 h = vec4(1.0);
    for( int i=0; i<64; i++ )
    {
		if( h.x<0.002 || t>10.0 ) break;
        h = vec4(map(ro + rd*t), 1.0);
        res = vec4(t,h.yzw);
        t += h.x;
    }
	if( t>10.0 ) res=vec4(-1.0);
    return res;
}


float softshadow( in vec3 ro, in vec3 rd, float mint, float k )
{
    float res = 1.0;
    float t = mint;
	float h = 1.0;
    for( int i=0; i<32; i++ )
    {
        h = map(ro + rd*t).x;
        res = min( res, k*h/t );
		t += clamp( h, 0.005, 0.1 );
    }
    return clamp(res,0.0,1.0);
}


vec3 calcNormal(in vec3 pos)
{
    vec3  eps = vec3(.001,0.0,0.0);
    vec3 nor;
    nor.x = map(pos+eps.xyy).x - map(pos-eps.xyy).x;
    nor.y = map(pos+eps.yxy).x - map(pos-eps.yxy).x;
    nor.z = map(pos+eps.yyx).x - map(pos-eps.yyx).x;
    return normalize(nor);
}


vec4 render(vec3 ro, vec3 rd) {
	vec4 color = intersect(ro, rd);/*
    vec3  nor = calcNormal(pos);
    float occ = color.y;

    vec3  pos = ro + color.x*rd;
    
	float sha = softshadow( pos, light, 0.01, 64.0 );*/
	
	return color;
}


void main( void ) {

	vec2 p = (-RENDERSIZE + 2.0*gl_FragCoord.xy) / RENDERSIZE.y;

    // camera
    vec3 ro = 1.1*vec3(2.5*sin(0.25*TIME),1.0+1.0*cos(TIME*.13),2.5*cos(0.25*TIME));
    vec3 ww = normalize(vec3(0.0) - ro);
    vec3 uu = normalize(cross( vec3(0.0,1.0,0.0), ww ));
    vec3 vv = normalize(cross(ww,uu));
    vec3 rd = normalize( p.x*uu + p.y*vv + 2.5*ww );
    
	vec4 col = render(ro, rd);
	
	
	
	
    gl_FragColor = col;
	
}