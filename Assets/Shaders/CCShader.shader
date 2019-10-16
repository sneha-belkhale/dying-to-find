Shader "Custom/CCShader"
{
    Properties
    {
       [Header(Base Parameters)]
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _AmbientLightColor ("Ambient Light Color", Color) = (1, 1, 1, 1)
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimAmount ("Rim Amount", Range(0, 2)) = 1.2
        _StepWidth ("Step Width Amount", Range(0, 1)) = 0.05
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

    CGPROGRAM
    #pragma surface surf Toon fullforwardshadows

    float _StepWidth;
    float _RimAmount;
    fixed4 _RimColor;
    fixed4 _Color;
    fixed4 _AmbientLightColor;
    
    struct Input {
        float2 uv_MainTex;
    };
    
    sampler2D _MainTex;
    float random( float2 p )
    {
        return frac(sin(dot(p.xy,float2(1.,65.115)))*2773.8856);
    }
    
    
    struct CCSurfaceOutput {
        fixed3 Albedo;
        fixed3 Normal;
        fixed3 Emission;
        half Specular;
        fixed Gloss;
        fixed Alpha;
        fixed2 uv;
    };

    void surf (Input IN, inout CCSurfaceOutput o) {
        o.Albedo = _Color.rgb * tex2D (_MainTex, IN.uv_MainTex).rgb;
        o.uv = IN.uv_MainTex;
    }
    
    half4 LightingToon (CCSurfaceOutput s, fixed3 viewDir, UnityGI gi) {
        half NdotL = dot (s.Normal, gi.light.dir);
        float lightIntensity = smoothstep(0, _StepWidth, NdotL);
        float3 lightColor = lightIntensity * gi.light.color;
        
        float4 rimDot = 1 - dot(viewDir, s.Normal);
        float rimIntensity = rimDot * _RimAmount * pow( NdotL, 3.0);

        rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
        float4 rim = rimIntensity * _RimColor;

        half4 c;
        c.rgb = s.Albedo * (lightColor + (1-lightIntensity)*_AmbientLightColor + rim.rgb);
        c.a = 1.0;
        
        // GRAIN 
        //float rand = random(floor(4000 * s.uv));
        //fixed4 grain = fixed4(rand,rand,rand,1.0);
        //c = lerp(c, grain, 0.25);
        
        return c;
    }
    
    inline void LightingToon_GI(CCSurfaceOutput s, UnityGIInput data, inout UnityGI gi)
    {
    }
    ENDCG
    }
    FallBack "Diffuse"
}