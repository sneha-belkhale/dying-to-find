
Shader "Custom/MinimalInstancedShader"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
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
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 vWorldPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            float4 _GlobalPulseOrigin;
            float _GlobalPulseTimeElapsed;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);            
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vWorldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = 0.3 * float4(1,1,1,1);

                float isCorner = max(
                    step(min(i.uv.x, 1-i.uv.x),0.1),
                    step(min(i.uv.y, 1-i.uv.y),0.1)
                );
                // global pulse
                float pulseStep = _GlobalPulseTimeElapsed;
                float radialFalloff = min(length(i.vWorldPos.xyz - _GlobalPulseOrigin.xyz) * 0.1, 1);    
                float p = isCorner * step(abs(pulseStep - radialFalloff), 0.01);
                col.r -= p;
                col.bg += 2 * p;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}