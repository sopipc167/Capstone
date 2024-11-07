Shader "Custom/Mask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SunPosition ("Sun Position", Vector) = (0,0,0,0)
        _MoonPosition ("Moon Position", Vector) = (0,0,0,0)
        _SunRadius ("Sun Radius", Float) = 0.5
        _MoonRadius ("Moon Radius", Float) = 1.0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent+1" "RenderType"="Transparent"}
        LOD 100  // 안드로이드 호환성을 위해 추가

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma target 3.0  // 타겟 쉐이더 모델 명시
            #pragma vertex vert
            #pragma fragment frag
            
            // 안드로이드 호환성을 위한 precision 선언
            #ifdef SHADER_API_GLES
            precision mediump float;
            #endif
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;  // float3에서 float4로 변경
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _SunPosition;   // float3에서 float4로 변경
            float4 _MoonPosition;  // float3에서 float4로 변경
            float _SunRadius;
            float _MoonRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldPos = i.worldPos.xyz;
                float distToMoon = length(worldPos - _MoonPosition.xyz);
                float distToSun = length(worldPos - _SunPosition.xyz);

                if (distToMoon < _MoonRadius && distToSun < _SunRadius)
                {
                    return fixed4(0, 0, 0, 1);
                }
                return fixed4(0.5, 0.5, 0.5, 1);
            }
            ENDCG
        }
    }
    // 폴백 추가
    FallBack "Transparent/VertexLit"
}
