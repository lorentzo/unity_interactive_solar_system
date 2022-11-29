Shader "Unlit/Normal"
{
    Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

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

            sampler2D _MainTex;
            sampler2D _CameraDepthNormalsTexture;
            float4x4 _viewToWorld; // view/camera to world space matrix

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample depth&normal texture.
                float4 depthnormal = tex2D(_CameraDepthNormalsTexture, i.uv);

                // Unpack depth and normal.
                float3 normal; // NOTE: view/camera space normals!
                float depth; // linear depth!
                DecodeDepthNormal(depthnormal, depth, normal);

                // Adapt depth and normals.
                float depth_as_distance_from_camera_in_unity_unity = depth * _ProjectionParams.z;
                normal = mul((float3x3)_viewToWorld, normal); // cast it to a 3x3 matrix before the multiplication so the position change doesn’t get applied only the rotation

                // Color the top.
                float up = dot(float3(0,1,0), normal);
                up = step(0.5, up);

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return up;
            }
            ENDCG
        }
    }
}
