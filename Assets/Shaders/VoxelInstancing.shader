
Shader "Custom/MinimalInstancedShader"
{
    Properties
    {
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_FOG_COORDS(2)
            };

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            float4 _GlobalPulseColor;
            float4 _GlobalPulseOrigin;
            float _GlobalPulseTimeElapsed;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);   
                UNITY_TRANSFER_INSTANCE_ID(v, o);         
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vWorldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float4 col = 0.3 * float4(1,1,1,1);

                float isCorner = max(
                    step(min(i.uv.x, 1-i.uv.x),0.1),
                    step(min(i.uv.y, 1-i.uv.y),0.1)
                );

                UNITY_APPLY_FOG(i.fogCoord, col);

                // global pulse
                float pulseStep = step(0, _GlobalPulseTimeElapsed);
                float radialFalloff = min(length(i.vWorldPos.xyz - _GlobalPulseOrigin.xyz) * 0.1, 1);    
                float p = step(abs(_GlobalPulseTimeElapsed - radialFalloff), 0.01);

                float pulseAmt = p;
                #ifdef UNITY_INSTANCING_ENABLED
                    pulseAmt += pulseStep * step(fmod(i.instanceID,80.),1);
                #endif

                col = lerp(col, _GlobalPulseColor, isCorner * pulseAmt);

                return col;
            }
            ENDCG
        }
    }
}