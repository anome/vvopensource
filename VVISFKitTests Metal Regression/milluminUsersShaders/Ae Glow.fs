/*{
  "DESCRIPTION": "AE / uni.glow風のハイライト抽出＋ガウシアンブラー＋合成を行うGlowエフェクト（IMG_NORM_PIXEL対応版）",
  "CATEGORIES": [ "Stylize", "Blur", "Glow" ],
  "INPUTS": [
    { "NAME": "inputImage",  "TYPE": "image" },

    { "NAME": "threshold",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.6 },
    { "NAME": "softKnee",    "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 0.5 },
    { "NAME": "radius",      "TYPE": "float", "MIN": 0.0, "MAX": 200.0,"DEFAULT": 40.0 },
    { "NAME": "intensity",   "TYPE": "float", "MIN": 0.0, "MAX": 5.0,  "DEFAULT": 1.2 },
    { "NAME": "gamma",       "TYPE": "float", "MIN": 0.1, "MAX": 3.0,  "DEFAULT": 1.0 },

    { "NAME": "tintColor",   "TYPE": "color", "DEFAULT": [1.0, 0.97, 0.9, 1.0] },

    { "NAME": "blendMode",   "TYPE": "float", "MIN": 0.0, "MAX": 2.0,  "DEFAULT": 0.0,
      "LABEL": "Blend: 0=Add 1=Screen 2=SoftLight" },
    { "NAME": "mixOriginal", "TYPE": "float", "MIN": 0.0, "MAX": 1.0,  "DEFAULT": 1.0 },
    { "NAME": "glowOnly",    "TYPE": "bool",  "DEFAULT": false }
  ],
  "PASSES": [
    { "TARGET": "brightTex" },
    { "TARGET": "blurHTex"  },
    { "TARGET": "blurVTex"  }
  ],
  "TARGETS": [
    { "NAME": "brightTex", "TYPE": "image" },
    { "NAME": "blurHTex",  "TYPE": "image" },
    { "NAME": "blurVTex",  "TYPE": "image" }
  ]
}*/

///////////////////////////////////////////////////////////
// 共通ユーティリティ
///////////////////////////////////////////////////////////

float luminance(vec3 c) {
    return dot(c, vec3(0.2126, 0.7152, 0.0722));
}

// uni.glow系の soft knee 付き閾値
float softThreshold(float lum, float thresh, float knee) {
    if (knee <= 0.0001) {
        return step(thresh, lum);
    }

    float kneeStart = thresh - knee;
    float kneeEnd   = thresh + knee;

    if (lum < kneeStart) return 0.0;
    if (lum >= kneeEnd)  return 1.0;

    float x = (lum - kneeStart) / (kneeEnd - kneeStart);
    return x * x * (3.0 - 2.0 * x);
}

// ブレンド系
vec3 blendAdd(vec3 base, vec3 glow) {
    return base + glow;
}

vec3 blendScreen(vec3 base, vec3 glow) {
    return 1.0 - (1.0 - base) * (1.0 - glow);
}

vec3 blendSoftLight(vec3 base, vec3 glow) {
    vec3 result = vec3(0.0);
    for (int i = 0; i < 3; ++i) {
        float b = base[i];
        float g = glow[i];
        if (g < 0.5) {
            result[i] = b - (1.0 - 2.0 * g) * b * (1.0 - b);
        } else {
            result[i] = b + (2.0 * g - 1.0) * (sqrt(b) - b);
        }
    }
    return result;
}

///////////////////////////////////////////////////////////
// パスごとの処理
///////////////////////////////////////////////////////////

void main() {
    vec2 uv = isf_FragNormCoord;

    ///////////////////////////////////////////////////////
    // PASS 0: ハイライト抽出 → brightTex
    ///////////////////////////////////////////////////////
    if (PASSINDEX == 0) {
        vec4 src = IMG_NORM_PIXEL(inputImage, uv);
        float lum = luminance(src.rgb);

        float k = clamp(softKnee, 0.0, 1.0);
        float t = softThreshold(lum, threshold, k * 0.5);

        vec3 bright = src.rgb * t;
        gl_FragColor = vec4(bright, src.a);
        return;
    }

    ///////////////////////////////////////////////////////
    // PASS 1: brightTex を横方向にガウシアンブラー → blurHTex
    ///////////////////////////////////////////////////////
    if (PASSINDEX == 1) {
        // radius を 0〜200 → ブラー強度スケールに
        float r = radius / 40.0;
        r = max(r, 0.0);

        if (r <= 0.0001) {
            gl_FragColor = IMG_NORM_PIXEL(brightTex, uv);
            return;
        }

        vec2 texel = vec2(1.0, 0.0) / RENDERSIZE; // 横
        vec2 step1 = texel * 1.3846154 * r;
        vec2 step2 = texel * 3.2307692 * r;

        vec3 c = IMG_NORM_PIXEL(brightTex, uv).rgb * 0.2270270;
        c += IMG_NORM_PIXEL(brightTex, uv + step1).rgb * 0.3162162;
        c += IMG_NORM_PIXEL(brightTex, uv - step1).rgb * 0.3162162;
        c += IMG_NORM_PIXEL(brightTex, uv + step2).rgb * 0.0702703;
        c += IMG_NORM_PIXEL(brightTex, uv - step2).rgb * 0.0702703;

        gl_FragColor = vec4(c, 1.0);
        return;
    }

    ///////////////////////////////////////////////////////
    // PASS 2: blurHTex を縦方向ブラー + 元映像と合成 → 最終出力
    ///////////////////////////////////////////////////////
    // 縦方向ブラー
    float r2 = radius / 40.0;
    r2 = max(r2, 0.0);

    vec3 blurV;

    if (r2 <= 0.0001) {
        blurV = IMG_NORM_PIXEL(blurHTex, uv).rgb;
    } else {
        vec2 texel = vec2(0.0, 1.0) / RENDERSIZE; // 縦
        vec2 step1 = texel * 1.3846154 * r2;
        vec2 step2 = texel * 3.2307692 * r2;

        blurV  = IMG_NORM_PIXEL(blurHTex, uv).rgb * 0.2270270;
        blurV += IMG_NORM_PIXEL(blurHTex, uv + step1).rgb * 0.3162162;
        blurV += IMG_NORM_PIXEL(blurHTex, uv - step1).rgb * 0.3162162;
        blurV += IMG_NORM_PIXEL(blurHTex, uv + step2).rgb * 0.0702703;
        blurV += IMG_NORM_PIXEL(blurHTex, uv - step2).rgb * 0.0702703;
    }

    vec4 base = IMG_NORM_PIXEL(inputImage, uv);

    // Tint
    vec3 glow = blurV * tintColor.rgb;

    // Gamma 調整
    float g = max(gamma, 0.01);
    glow = pow(max(glow, 0.0), vec3(g));

    // 強度
    glow *= intensity;

    // ブレンド
    vec3 blended;
    if (blendMode < 0.5) {
        blended = blendAdd(base.rgb, glow);
    } else if (blendMode < 1.5) {
        blended = blendScreen(base.rgb, glow);
    } else {
        blended = blendSoftLight(base.rgb, glow);
    }

    vec3 outColor;
    if (glowOnly) {
        outColor = glow;
    } else {
        // mixOriginal = 1.0 → オリジナル強め
        outColor = mix(blended, base.rgb, 1.0 - mixOriginal);
    }

    gl_FragColor = vec4(outColor, base.a);
}