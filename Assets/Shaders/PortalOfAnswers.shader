// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/PortalOfAnswers"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", float) = 1.000000
        [Space(5)]
        [Header(Color)]
        _Color ("Color", Color) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        [Space(5)]
        [Header(Effect)]
        _holeDiameter ("Hole Diameter", Range(0, 0.4)) = 0.0
        // _holeDepth ("Hole Depth", Range(0, 100)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull [_Cull]
        ZTest Off

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
                float3 normal : NORMAL;
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

            float _holeDiameter;

            v2f vert (appdata v)
            {
                v2f o;
                float radius = length(v.uv - float2(0.5,0.5));
                float holeDepth = 150 * _holeDiameter;
                float distanceToHole = smoothstep(_holeDiameter, _holeDiameter + 0.04, radius);

                v.vertex.y -= holeDepth * (1 - distanceToHole);
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float radius = length(i.uv - float2(0.5,0.5));
                float holeDepth = 150 * _holeDiameter;
                float distanceToHole = smoothstep(_holeDiameter, _holeDiameter + 0.04, radius);
                float inTransition = 1 - step(radius, _holeDiameter+ 0.03);

                fixed4 col = inTransition * tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
