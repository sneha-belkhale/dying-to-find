Shader "Effects/DelayedMotionVector"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Lerp ("Lerp", Float) = 0
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
            #include "noise.cginc"

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
            float4 _MainTex_ST;
            float _Lerp;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 newmv = tex2D(_CameraMotionVectorsTexture, i.uv) ;
                fixed4 old = tex2D(_MainTex, 2 * i.uv) ;
                fixed4 old2 = 0.95 * (tex2D(_MainTex, i.uv- 0.01 * snoise(i.uv)) );
                fixed4 old3 = 0.95 * (tex2D(_MainTex, i.uv+ 0.01 * snoise(i.uv)) );    
                fixed4 col =(old2 + old3)/2 + 0.9 * newmv;               
                
                // // fixed4 newmv = 0.01 * snoise(10 * UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST) + _Time.y) * fixed4(1,1,1,1);
                // fixed4 old = tex2D(_MainTex, 2 * i.uv) ;
                // // fixed4 old2 = 0.95 * (tex2D(_MainTex, i.uv- 0.01 * snoise(i.uv)) );
                // // fixed4 old3 = 0.95 * (tex2D(_MainTex, i.uv+ 0.01 * snoise(i.uv)) );    
                // fixed4 col = 0.9 * fixed4(old.xy,0,0) + 0.9 * fixed4(newmv.xy, 0, 0);

                return col;
            }
            ENDCG
        }
    }
}
