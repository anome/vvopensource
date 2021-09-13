/*{
    "CATEGORIES": [
        "XXX"
    ],
    "CREDIT": "by gosub7777777",
    "DESCRIPTION": "",
    "INPUTS": [
        {
            "NAME": "trigA",
            "TYPE": "event"
        },
        {
            "NAME": "trigB",
            "TYPE": "event"
        },
        {
            "NAME": "trigC",
            "TYPE": "event"
        },
        {
            "NAME": "speed",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MAX":1.0,
            "MIN":-1.0
        },
        {
            "NAME": "falloff",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MAX":1.0,
            "MIN":0.0
        }
        
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "FLOAT": false,
            "HEIGHT": "$HEIGHT",
            "PERSISTENT": true,
            "TARGET": "first",
            "WIDTH": "$WIDTH"
        },
        {
        }
    ]
}
*/

vec2 random2D(vec2 n){
    return vec2(
        fract(sin(n.x*n.y) * 43758.5453123),    
        fract(sin(n.y*n.x) * 123445.894321)
    );
}

void main() {
	if(PASSINDEX == 0){
        float c = 0.0;
        vec3 color = vec3(0.0);
        // if(abs(sin(TIME * 1.0)) <= 0.1 || trig){
        if(trigA || trigB || trigC){
            c = abs(isf_FragNormCoord.x - 0.5);
            c = smoothstep( falloff, 0.0, c) * 0.5;
        }

        color = vec3(c);

        // if(trigA){
        //     color.r = c;
        // }else
        // if(trigB){
        //     color.g = c;
        // }else
        // if(trigC){
        //     color.b = c;
        // }

        vec4 prev = IMG_PIXEL(first, gl_FragCoord.xy + vec2(speed * RENDERSIZE.x * -0.1,0.0));
        color += vec3(c) + prev.rgb * 0.99;

		gl_FragColor.rgb = vec3(color);
        gl_FragColor.a = 1.0;
	}
	if(PASSINDEX == 1){
		// gl_FragColor = vec4(0.1, 0.0, 0.0, 10.);
		gl_FragColor = smoothstep(0.5, 0.8, IMG_PIXEL(first, gl_FragCoord.xy));	
    }
}