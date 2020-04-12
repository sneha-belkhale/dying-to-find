Shader "Effects/MotionVector"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _CameraMotionVectorsTexture;
            sampler2D _MotionVecs;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mv = tex2D(_MotionVecs, i.uv);
                fixed4 col = tex2D(_MainTex, i.uv + 3 * mv.rg);
                fixed4 coolerColorMv = 30 * fixed4(abs(mv.r), 0, abs(mv.g), 1);
                return col;
            }
            ENDCG
        }
    }
}
