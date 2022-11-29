Shader "Unlit/Outlines"
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
            float4 _CameraDepthNormalsTexture_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            struct DepthNormalDecoded
            {
                float depth;
                float3 normal;
            };

            DepthNormalDecoded fetch_depth_normal
            (
                sampler2D depth_normal_texture, 
                float2 depth_normal_texture_texel_size, 
                float2 uv, 
                float2 uv_shift
            )
            {
                DepthNormalDecoded depth_normal;
                depth_normal.depth = 0.0f;
                depth_normal.normal = float3(0,0,0);
                float4 depthnormal = tex2D(depth_normal_texture, uv + depth_normal_texture_texel_size * uv_shift);
                // Unpack depth and normal.
                DecodeDepthNormal(depthnormal, depth_normal.depth, depth_normal.normal);
                depth_normal.depth = depth_normal.depth * _ProjectionParams.z;
                return depth_normal;
            }

            // read from several pixels around the pixel we’re rendering 
            // and calculate the difference in depth and normals to the center pixel. 
            // The more different they are, the stronger the outline is.
            fixed4 frag (v2f i) : SV_Target
            {
                DepthNormalDecoded depth_normal_center = fetch_depth_normal(
                    _CameraDepthNormalsTexture,
                    _CameraDepthNormalsTexture_TexelSize.xy,
                    i.uv,
                    float2(0,0));

                DepthNormalDecoded depth_normal_right = fetch_depth_normal(
                    _CameraDepthNormalsTexture,
                    _CameraDepthNormalsTexture_TexelSize.xy,
                    i.uv,
                    float2(1,0));

                DepthNormalDecoded depth_normal_left = fetch_depth_normal(
                    _CameraDepthNormalsTexture,
                    _CameraDepthNormalsTexture_TexelSize.xy,
                    i.uv,
                    float2(-1,0));

                DepthNormalDecoded depth_normal_top = fetch_depth_normal(
                    _CameraDepthNormalsTexture,
                    _CameraDepthNormalsTexture_TexelSize.xy,
                    i.uv,
                    float2(0,1));

                DepthNormalDecoded depth_normal_bot = fetch_depth_normal(
                    _CameraDepthNormalsTexture,
                    _CameraDepthNormalsTexture_TexelSize.xy,
                    i.uv,
                    float2(0,-1));
                
                float depth_outlines = 0.0f;
                depth_outlines = depth_normal_center.depth - depth_normal_right.depth;
                depth_outlines += depth_normal_center.depth - depth_normal_left.depth;
                depth_outlines += depth_normal_center.depth - depth_normal_top.depth;
                depth_outlines += depth_normal_center.depth - depth_normal_bot.depth;

                float normal_outlines = 0.0f;
                float3 normal_diff;
                normal_diff = depth_normal_center.normal - depth_normal_right.normal;
                normal_outlines += normal_diff.r + normal_diff.g + normal_diff.b;
                normal_diff = depth_normal_center.normal - depth_normal_left.normal;
                normal_outlines += normal_diff.r + normal_diff.g + normal_diff.b;
                normal_diff = depth_normal_center.normal - depth_normal_top.normal;
                normal_outlines += normal_diff.r + normal_diff.g + normal_diff.b;
                normal_diff = depth_normal_center.normal - depth_normal_bot.normal;
                normal_outlines += normal_diff.r + normal_diff.g + normal_diff.b;

                float4 object_color = tex2D(_MainTex, i.uv);
                float4 outline_color = (1,1,1,1);

                return lerp(object_color, outline_color, depth_outlines+normal_outlines);
            }
            ENDCG
        }
    }
}
