Shader "Unlit/DissolveOut"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", float) = 1.000000
        [Space(5)]
        [Header(Color)]
        _Color ("Color", Color) = (0,0,0,0)
        [Toggle]_UsePointColor("Use Point Color?", Float) = 0
        _MainTex ("Texture", 2D) = "white" {}

        [Space(5)]
        [Header(Effects)]
        [Toggle(USE_EFFECTS)] _Effects("Use Effects", Float) = 1
        _Scrolling ("Scroll", Range(0, 1)) = 0.0
        _FadeOutVal ("Fade out percent", Range(0, 1)) = 0.0
        _VertOutVal ("Vert out direction", Vector) = (0,0,0,1)
        _WarpDir ("Warp direction", Vector) = (0,0,0,0)
        _WarpCenter ("Warp center", Vector) = (0,0,0,0)
        [Toggle(USE_LEVELING)] _Leveling("Use Leveling", Float) = 1

        [Toggle]_IgnoreGlobalStretch("Ignore Global Stretch?", Float) = 0
        [Toggle]_IgnoreGlobalStretchFrag("Ignore Global Stretch Frag?", Float) = 0
        _Offset ("Offset ID", Range(0, 1)) = 0.0



    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull [_Cull]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma shader_feature USE_EFFECTS
            #pragma shader_feature USE_LEVELING

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            #if (USE_EFFECTS)
                float _FadeOutVal;
                float4 _VertOutVal;
                float4 _WarpCenter;
                float4 _WarpDir;
                float _Scrolling;
            #endif

            #if (USE_LEVELING)
                float _LevelAmt;
            #endif

            float _UsePointColor;
            float _IgnoreGlobalStretch;
            float _IgnoreGlobalStretchFrag;
            float _GlobalStretch;
            float _Offset;

            v2f vert (appdata v)
            {
                v2f o;

                #if (USE_LEVELING)
                    v.vertex.y *= (1 + 0.5 * _LevelAmt * sin(0.5 * _Time.y + floor(0.1 * v.vertex.z)));
                #endif

                #if (USE_EFFECTS)
                    float dissolve = tex2Dlod(_MainTex, float4(v.uv + _Scrolling * _Time.y, 0,0)).r;
                    v.vertex.xyz += _VertOutVal.w * 3 * _FadeOutVal *  dissolve * _VertOutVal.xyz;
                    // half usingGlobalFade = step(0.001, _GlobalFadeOut) * (1 - _IgnoreGlobalFade);
                    // float fadeOutVal = lerp(_FadeOutVal, _GlobalFadeOut, usingGlobalFade);
                    v.vertex.y *= (1 + (1 - _IgnoreGlobalStretch) * _GlobalStretch);
                #endif

                float3 vWorldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                #if (USE_EFFECTS)
                    float3 dist = (vWorldPos - _WarpCenter.xyz);
                    half radialFalloff = min(dot(dist, dist) * dissolve * _WarpCenter.w, 1);
                    vWorldPos += (1-radialFalloff) * _WarpDir.xyz;
                #endif

                o.vertex = UnityWorldToClipPos(vWorldPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = lerp(_Color, i.color * _Color, _UsePointColor);

                #if (USE_EFFECTS)
                    half usingGlobalStretch = step(0.001, _GlobalStretch) * (1 - _IgnoreGlobalStretchFrag);
                    float dissolve = tex2D(_MainTex, fixed2(i.uv.x + usingGlobalStretch * (1 + sin(_Offset + 0.2 *_Time.y)),i.uv.y)).r;
                    // float dissolveScroll = tex2D(_MainTex2, 0.1 * _UvScale * IN.uv_MainTex + 0.5 * cos(0.5*_Time.y)).r;
                    // finalWireframe *= clamp(dissolve - _FadeOutVal, 0, 1);
                    float isVisible = dissolve - max(_FadeOutVal, usingGlobalStretch * _GlobalStretch * 0.1);
                    clip(isVisible);
                    float isGlowing = smoothstep(0.15, 0.01, isVisible);
                    col += _FadeOutVal * isGlowing * fixed4(0.5,0.5,0.5,1);
                #endif

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
