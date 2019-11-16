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
        _scale ("Scale", Range(0, 100)) = 0.0
        _spacing ("Spacing", Range(0, 100)) = 0.0
        _growth ("Growth", Range(0, 1)) = 0.0
        // _holeDepth ("Hole Depth", Range(0, 100)) = 0.0
        [Toggle(DISSOLVE)]_DissolveEffect("Dissolve", Float) = 0
        p1 ("P1", Range(0, 10)) = 0.0
        p2 ("P2", Range(0, 100)) = 0.0


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
            #pragma shader_feature DISSOLVE

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
            float _scale;
            float _spacing;
            float _growth;

            float p1;
            float p2;

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
            float2 cart2polar (float2 cart)
            {
                float r = length (cart);
                float phi = atan2 (cart.x, cart.y);
                return float2 (phi, r);
            }

            float2 polar2cart (float2 polar)
            {
                float x = polar.x*cos (polar.y);
                float y = polar.x*sin (polar.y);
                return float2 (y, x);
            }
            float rand (float2 st) {
                return frac(sin(dot(st.xy,
                                    float2(12.9898,78.233)))*
                    43758.5453123);
            }
            fixed4 frag (v2f i) : SV_Target
            {

                float2 polar = cart2polar(2 * i.uv - 1);
                float segment = floor(abs(polar.x) * _scale);
                float r = rand(float2(segment, 1));
                float r2 = rand(float2(segment + 10, 3));
                float color = round(segment%_spacing)/_spacing * r;
                float isVisible = 1;
                
                if(0.6 * polar.y > _growth * r2 - 0.3 * r){
                    isVisible = 0;
                }

                // FOR THE FALL 
                float radius = 0.5 * polar.y;
                float holeDepth = 150 * _holeDiameter;
                float distanceToHole = smoothstep(_holeDiameter, _holeDiameter + 0.04, radius);
                float inTransition = 1 - step(radius, _holeDiameter+ 0.03) * smoothstep(0, 0.1, _holeDiameter);

                #if(DISSOLVE)
                    float dissolve = tex2D(_MainTex, 0.7 * polar);
                    isVisible = min(step( 0.7 - 0.64 * _growth - dissolve, 0.01), isVisible);
                #endif

                fixed4 col = inTransition * lerp(fixed4(1,1,1,1), color, isVisible);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
