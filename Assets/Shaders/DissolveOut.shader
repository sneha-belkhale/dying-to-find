Shader "Unlit/DissolveOut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FadeOutVal ("Fade out percent", Range(0, 1)) = 0.0
        _VertOutVal ("Vert out direction", Vector) = (0,0,0,1)
        _WarpDir ("Warp direction", Vector) = (0,0,0,0)
        _WarpCenter ("Warp center", Vector) = (0,0,0,0)
        _Color ("Color", Color) = (0,0,0,0)
        _Offset ("Offset ID", Range(0, 1)) = 0.0
        [FloatToggle]_IgnoreGlobalStretch("Ignore Global Stretch?", Float) = 0
        [FloatToggle]_IgnoreGlobalStretchFrag("Ignore Global Stretch?", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _FadeOutVal;
            float4 _VertOutVal;
            float4 _WarpCenter;
            float4 _WarpDir;

            float _IgnoreGlobalStretch;
            float _IgnoreGlobalStretchFrag;
            float _GlobalStretch;
            float _Offset;

            v2f vert (appdata v)
            {
                v2f o;
                float dissolve = tex2Dlod(_MainTex, float4(v.uv, 0,0)).r;
                v.vertex.xyz += _VertOutVal.w * 3 * _FadeOutVal *  dissolve * _VertOutVal.xyz;

                // half usingGlobalFade = step(0.001, _GlobalFadeOut) * (1 - _IgnoreGlobalFade);
                // float fadeOutVal = lerp(_FadeOutVal, _GlobalFadeOut, usingGlobalFade);

                v.vertex.y *= (1 + (1 - _IgnoreGlobalStretch) * _GlobalStretch);

                float3 vWorldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                float dist = (vWorldPos - _WarpCenter.xyz);
                half radialFalloff = min(dot(dist, dist) * _WarpCenter.w, 1);
                vWorldPos += (1-radialFalloff) * _WarpDir.xyz;

                o.vertex = UnityWorldToClipPos(vWorldPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the dissolve texture
                fixed4 col = _Color;

                half usingGlobalStretch = step(0.001, _GlobalStretch) * (1 - _IgnoreGlobalStretchFrag);
                float dissolve = tex2D(_MainTex, fixed2(i.uv.x + usingGlobalStretch * (1 + sin(_Offset + 0.2 *_Time.y)),i.uv.y)).r;
                // float dissolveScroll = tex2D(_MainTex2, 0.1 * _UvScale * IN.uv_MainTex + 0.5 * cos(0.5*_Time.y)).r;
                // finalWireframe *= clamp(dissolve - _FadeOutVal, 0, 1);
                float isVisible = dissolve - max(_FadeOutVal, usingGlobalStretch * _GlobalStretch * 0.1);
                clip(isVisible);

                float isGlowing = smoothstep(0.15, 0.01, isVisible);
                col += _FadeOutVal * isGlowing * fixed4(0.5,0.5,0.5,1);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
