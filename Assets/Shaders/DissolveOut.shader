Shader "Unlit/DissolveOut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FadeOutVal ("Fade out percent", Range(0, 1)) = 0.0
        _VertOutVal ("Vert out direction", Vector) = (0,0,0,1)
        _Color ("Color", Color) = (0,0,0,0)
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

            v2f vert (appdata v)
            {
                v2f o;
                float dissolve = tex2Dlod(_MainTex, float4(v.uv, 0,0)).r;
                v.vertex.xyz += _VertOutVal.w * 3 * _FadeOutVal *  dissolve * _VertOutVal.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the dissolve texture
                fixed4 col = _Color;

                float dissolve = tex2D(_MainTex, i.uv).r;
                // float dissolveScroll = tex2D(_MainTex2, 0.1 * _UvScale * IN.uv_MainTex + 0.5 * cos(0.5*_Time.y)).r;
                // finalWireframe *= clamp(dissolve - _FadeOutVal, 0, 1);
                float isVisible = dissolve - _FadeOutVal;
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
