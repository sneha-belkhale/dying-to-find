// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/DoubleSide"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", float) = 1.000000
        [Space(5)]
        [Header(Color)]
        _Color ("Color", Color) = (0,0,0,0)
        _ColorBackside ("Color Backside", Color) = (0,0,0,0)
        [Toggle]_UsePointColor("Use Point Color?", Float) = 0
        [Toggle]_UseFog("Use Fog?", Float) = 1
        _MainTex ("Texture", 2D) = "white" {}
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
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
            float4 _ColorBackside;
            float _UseFog;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 viewDir = WorldSpaceViewDir(v.vertex);
                if(dot(viewDir, float3(0,1,0)) > 0){
                    o.color = _Color;
                } else {
                     o.color = _ColorBackside;
                }
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = i.color + tex2D(_MainTex, i.uv);
                // apply fog
                fixed4 colWithFog = col;
                UNITY_APPLY_FOG(i.fogCoord, colWithFog);

                col = lerp(col, colWithFog, _UseFog);

                return col;
            }
            ENDCG
        }
    }
}
