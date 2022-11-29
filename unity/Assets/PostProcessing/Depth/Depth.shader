Shader "Unlit/Depth"
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
            sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample depth texture.
                float depth = tex2D(_CameraDepthTexture, i.uv).r;
                depth = Linear01Depth(depth);
                //depth = depth * _ProjectionParams.z; // independent of the clipping planes of the camera and in a unit of measurement we can understand (unity units).
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return depth;
            }
            ENDCG
        }
    }
}
