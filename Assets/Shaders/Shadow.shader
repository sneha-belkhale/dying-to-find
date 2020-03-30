Shader "Lit/Shadow"
{
    Properties
    {
        _LightColor ("LightColor", Color) = (0,0,0,0)
        _ShadowColor ("ShadowColor", Color) = (1,1,1,1)
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

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                SHADOW_COORDS(1)
            };

            fixed4 _LightColor;
            fixed4 _ShadowColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vertex = UnityWorldToClipPos(o.worldPos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz);
                attenuation = 10 * pow(attenuation, 3);
                attenuation = step(attenuation, 0.001);
                fixed4 col = lerp(_LightColor, _ShadowColor,attenuation);
                return col;
            }
            ENDCG
        }
        UsePass "VertexLit/SHADOWCASTER"
    }
}
