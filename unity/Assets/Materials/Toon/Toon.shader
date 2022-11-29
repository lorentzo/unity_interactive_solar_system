Shader "Custom/Toon"
{
    Properties
    {
        toon1 ("Toon 1", Color) = (1,1,1,1)
        toon2 ("Toon 2", Color) = (1,1,1,1)
        sun_center ("Sun Center", Vector) = (0,0,0)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Surace shader: extended by Unity in background for lighting calculation
        // Surface shader function is called `surf` and we use our custom lighting model.
        // `fullforwardshadows` makes sure unity adds the shadow passes the shader might need
        #pragma surface surf Toon fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 toon1;
        fixed4 toon2;

        struct Input
        {
            float2 uv_MainTex;
        };

        // Lighting function. Will be called once per light
        float4 LightingToon(SurfaceOutput surface_output, float3 light_dir, half3 view_dir, float shadow_attenuation)
        {
            float towards_light = dot(surface_output.Normal, light_dir);

            // Toon.
            float towards_light_change = fwidth(towards_light);
            float toon_intensity = smoothstep(0, towards_light_change, towards_light);

            // Result
            return toon1 * toon_intensity;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * toon2;
            o.Albedo = c.rgb;
            //o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
