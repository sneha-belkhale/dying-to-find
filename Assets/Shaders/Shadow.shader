Shader "Lit/Shadow"
{
    Properties
    {
        [Header(Color)]
        _LightColor ("LightColor", Color) = (0,0,0,0)
        _ShadowColor ("ShadowColor", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}

        [Space(5)]
        [Header(Effects)]
        [Toggle(USE_EFFECTS)] _Effects("Use Effects", Float) = 1
        _FadeOutVal ("Fade out percent", Range(0, 1)) = 0.0
        _VertOutVal ("Vert out direction", Vector) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

            Lighting On
        Pass
        {
            Tags {"LightMode" = "ForwardAdd"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fwdadd_fullshadows
            #pragma shader_feature USE_EFFECTS

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                SHADOW_COORDS(2)
            };

            fixed4 _LightColor;
            fixed4 _ShadowColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            #if (USE_EFFECTS)
                float _FadeOutVal;
                float4 _VertOutVal;
            #endif

            v2f vert (appdata v)
            {
                v2f o;
                #if (USE_EFFECTS)
                    float dissolve = tex2Dlod(_MainTex, float4(v.uv, 0,0)).r;
                    v.vertex.xyz += _VertOutVal.w * 3 * _FadeOutVal *  dissolve * _VertOutVal.xyz;
                #endif
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vertex = UnityWorldToClipPos(o.worldPos);
                o.uv = v.uv;
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz);
                attenuation = 10 * pow(attenuation, 3);
                attenuation = step(attenuation, 0.001);
                fixed4 col = lerp(_LightColor, _ShadowColor, attenuation);

                #if (USE_EFFECTS)
                    float dissolve = tex2D(_MainTex, i.uv.xy).r;
                    float isVisible = dissolve - _FadeOutVal;
                    clip(isVisible);
                #endif

                return col;
            }
            ENDCG
        }
        UsePass "VertexLit/SHADOWCASTER"
    }
}
