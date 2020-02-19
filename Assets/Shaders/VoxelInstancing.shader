
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
                float4 color : COLOR;
            };

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);            
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 worldSpace = mul(unity_ObjectToWorld, v.vertex);
                o.uv = worldSpace;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // float r = step(0.3, round(frac(5 * i.uv.y + 30 * _Time.x) * 5.0)/5.0);
                float r = 0.3;
                i.color = float4(r,r,r,r);
                return i.color;
            }
            ENDCG
        }
    }
}