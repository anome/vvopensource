/*
{
    "CREDIT": "Lightform",
    "DESCRIPTION": "",
    "CATEGORIES": [
        "generators"
    ],
    "INPUTS": [
        {
            "NAME": "loopCount",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 20.0
        },
        {
            "NAME": "center",
            "TYPE": "point2D",
            "DEFAULT": [
                0.5,
                0.5
            ],
            "MIN": [
                0.0,
                0.0
            ],
            "MAX": [
                1.0,
                1.0
            ]
        },
        {
            "NAME": "radiusIntensity",
            "TYPE": "float",
            "MAX": 1,
            "DEFAULT": 0.05,
            "MIN": 0
        },
        {
            "NAME": "angleIntensity",
            "TYPE": "float",
            "MAX": 1,
            "DEFAULT": 1.0,
            "MIN": 0
        },
        {
            "NAME": "spiralChoice",
            "TYPE": "long",
            "DEFAULT": 2,
			"LABELS": ["A", "B", "A+B", "A*B"],
			"VALUES": [0, 1, 2, 3]
        },
        {
            "NAME": "powerPop",
            "TYPE": "bool",
            "DEFAULT": 0
        }
    ],
    "PRESETS": [
        {
            "NAME": "Standard",
            "VALUES": {
                "angleIntensity": 1.0,
                "center": [
                    0.5,
                    0.5
                ],
                "loopCount": 1.0,
                "powerPop": 0,
                "radiusIntensity": 0.05,
                "spiralChoice": 0
            }
        },
        {
            "NAME": "Gems",
            "VALUES": {
                "angleIntensity": 0.75,
                "center": [
                    0.5,
                    0.5
                ],
                "loopCount": 1.0,
                "powerPop": 0,
                "radiusIntensity": 0.3,
                "spiralChoice": 1
            }
        },
        {
            "NAME": "Wheels",
            "VALUES": {
                "angleIntensity": 0.5,
                "center": [
                    0.5,
                    0.5
                ],
                "loopCount": 1.0,
                "powerPop": 0,
                "radiusIntensity": 0.7,
                "spiralChoice": 0
            }
        },
        {
            "NAME": "Tunnel Vision",
            "VALUES": {
                "angleIntensity": 0.0,
                "center": [
                    0.5,
                    0.5
                ],
                "loopCount": 1.0,
                "powerPop": 0,
                "radiusIntensity": 0.15,
                "spiralChoice": 1
            }
        },
        {
            "NAME": "Unfold",
            "VALUES": {
                "angleIntensity": 1.0,
                "center": [
                    0.5,
                    0.5
                ],
                "loopCount": 1.0,
                "powerPop": 1,
                "radiusIntensity": 0.0,
                "spiralChoice": 0
            }
        },
        {
            "NAME": "Enter the Loop",
            "VALUES": {
                "angleIntensity": 0.3,
                "center": [
                    0.5,
                    0.5
                ],
                "loopCount": 1.0,
                "powerPop": 1,
                "radiusIntensity": 0.15,
                "spiralChoice": 0
            }
        },
        {
            "NAME": "Kicks",
            "VALUES": {
                "angleIntensity": 0.75,
                "center": [
                    0.5,
                    0.5
                ],
                "loopCount": 1.0,
                "powerPop": 0,
                "radiusIntensity": 0.15,
                "spiralChoice": 1
            }
        }
    ]
}
*/


vec3 spiral(vec2 uv, vec2 center)
{
    float t = TWO_PI * loopCount * TIME / SLIDE_LENGTH;

    vec2  d = uv - center;
    float r = length(d);
    float a = atan(d.y,d.x);
    
    uv.y = a+r;
    
    float rings = sin(r*4.0);

    // default intensity: 1.0 -> 5.0
    float r_int = radiusIntensity * 100.0;
    
    float a_int = floor(angleIntensity * 5.0);

    vec3 c = vec3(sin(r * r_int + uv.y * a_int + t) + rings);
    uv.y*=5.0;
    c.r +=   sin(r * 5.0 + uv.y + t);
    c.g +=   sin(r * 6.0 + uv.y - t);
    c.b +=   cos(r * 7.0 + uv.y + t);
    
    if (spiralChoice == 0) return c;
    
    r = log(r);    
    uv.y = a;

    // default intensity: 1.0 -> 20.0
    a_int *= 8.0;
    
    vec3 l = vec3(sin(r + uv.y * a_int + t));
    r *= 10.0;
    float ruv = sin(r+uv.y);
    t    = sin(t);
    l.r += ruv + t;
    l.g += ruv - t;
    l.b += ruv + t;
    
    if (spiralChoice == 1)
    {
        return l;
    }
    else if (spiralChoice == 2)
    {
        return c+l;
    }
    else // if (spiralChoice == 3)
    {
        return c*l;
    }
}

void main()
{
    // normalize to -1 to 1 + aspect correction
    vec2 uv = 2.0 * gl_FragCoord.xy/RENDERSIZE.xy - 1.0;
    uv.x *= RENDERSIZE.x/RENDERSIZE.y;

    vec2 c = 2.0 * center - 1.0;
    c.x *= RENDERSIZE.x/RENDERSIZE.y;

    vec4 col = vec4(spiral(uv, c), 1.0);

    // live parameter broken?
    if (powerPop)
        col = vec4(pow(col.r, 2.0),pow(col.g, 2.0),pow(col.b, 2.0),1.0);

    gl_FragColor = col;
}
