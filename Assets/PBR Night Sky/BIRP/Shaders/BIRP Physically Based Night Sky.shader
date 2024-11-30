Shader"Custom/Physically Based Night Sky"
{
    Properties
    {
        [NoScaleOffset] _StarsCubeMap ("Stars Cube Map", Cube) = "black" {}
        [NoScaleOffset] _ConstellationsCubeMap ("Constellations Cube Map", Cube) = "black" {}
        [NoScaleOffset] _MoonTex("Moon Cube Map", Cube) = "black" {}
        _ConstellationsFade("Constellations Fade", float) = 1
        _MoonRadius("Moon Radius", float) = 1
        _PlanetsScale("Planets Scale", float) = 1
        _Exposure("Exposure", float) = 1
        _StarTwinkleNoise("Star Twinkle Noise", 2D) = "white" {}
        _StarTwinkleSpeed("Stars Twinkle Speed", float) = 0.1
        _StarTwinkleIntensity("Stars Twinkle Intensity", Range(0, 1)) = 0.1
        _StarTwinkleNoiseTiling("Stars Twinkle Noise Tiling", float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 texCoords : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            samplerCUBE _StarsCubeMap;
            samplerCUBE _ConstellationsCubeMap;
            samplerCUBE _MoonTex;
            sampler2D _StarTwinkleNoise;
            sampler2D _CameraDepthTexture;
            float _ConstellationsFade;
            float _MoonRadius;
            float _PlanetsScale;
            float3 _PlanetDirections[8];
            float3 _PlanetColours[8];
            float _PlanetSizes[8];
            float3 _MoonDirection;
            float4x4 _GlobalToLocalMoonMat;
            float4x4 _StarsRotationMatrix;
            float3 _SunDirection;
            float _Exposure;
            float _StarTwinkleSpeed;
            float _StarTwinkleIntensity;
            float _StarTwinkleNoiseTiling;

            float RaySphereIntersection(float3 rayDir, float3 spherePos, float radius) {
                float3 oc = -spherePos;
                float b = dot(oc, rayDir);
                float c = dot(oc, oc) - radius * radius;
                float h = b * b - c;
                if (h < 0.0) {
                    return -1.0;
                }
                h = sqrt(h);
                return -b - h;
            }

            float4 DrawCelesital(float LdotV, float angularDiameter) {
                return pow(saturate(LdotV), rcp(angularDiameter * 0.5) * 1e4);
            }

            float TwinklingStarsAlpha(float3 worldPos) {
                float3 worldNorm = normalize(worldPos);
                float PI = 3.1415926;
                float TAU = 6.28318530;
                float u = atan2(worldNorm.x, worldNorm.z) / TAU;
                float v = asin(worldNorm.y) / (PI * 0.5);
                float2 uv = float2(u, v);

                float2 sampleUV = uv * _StarTwinkleNoiseTiling + _Time.x * _StarTwinkleSpeed;
                return clamp(tex2D(_StarTwinkleNoise, sampleUV), 1 - _StarTwinkleIntensity, 1);
            }

            v2f vert (appdata v) {
                v2f o;
                o.texCoords = v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldPos = worldPos;
                o.viewDir = UnityWorldSpaceViewDir(worldPos);
                o.screenPos = ComputeScreenPos(o.vertex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                float depthValue = Linear01Depth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r);
                
                if (depthValue < 0.999) {
                    discard;
                }

                float3 sunDirection = normalize(_SunDirection.xyz);
                float3 normViewDir = normalize(i.viewDir);
                float sunViewDot = dot(sunDirection, normViewDir);
                float sunViewDot01 = saturate((sunViewDot + 2) * 0.5);
                float sunZenithDot = sunDirection.y;
                float downDot = dot(float3(0, -1, 0), normViewDir);
                float fogBlend = lerp(0, 1, downDot * 8);

                // Stars
                float3 rotatedTexCoords = mul(_StarsRotationMatrix, i.texCoords);
                float4 starsCol = texCUBE(_StarsCubeMap, rotatedTexCoords);
                float starStrength = sunViewDot01 * (saturate(sunZenithDot));
                starsCol.a = starsCol.r * starStrength * fogBlend * TwinklingStarsAlpha(i.worldPos);

                // Moon
                float moonViewDot = dot(_MoonDirection, normViewDir);
                float moonIntersect = RaySphereIntersection(normViewDir, _MoonDirection, _MoonRadius);
                float moonMask = saturate(round(moonIntersect));
                float3 moonNormal = normalize(_MoonDirection - normViewDir * moonIntersect);
                float moonNdotL = saturate(dot(moonNormal, -sunDirection));

                float3 moonTexCoords = mul(_GlobalToLocalMoonMat, float4(moonNormal, 0)).xyz;

                moonTexCoords = float3(moonTexCoords.x, -moonTexCoords.y, moonTexCoords.z);
                float4 moonCol = texCUBE(_MoonTex, moonTexCoords);

                moonCol.rgb *= 2 * moonMask * moonNdotL;
                moonCol.a = moonMask * moonNdotL * fogBlend * lerp(0.1, 0.5, starStrength);

                // Solar eclipse
                float solarEclipse01 = saturate(smoothstep(1 - _MoonRadius * _MoonRadius, 1, dot(_MoonDirection, sunDirection)));
                moonCol.a = lerp(moonCol.a, moonMask, saturate(solarEclipse01 * 3));
                moonCol.rgb = lerp(moonCol.rgb, 0, solarEclipse01 * moonMask);

                // Lunar eclipse
                float lunarEclipseMask = 1 - step(1 - _MoonRadius * _MoonRadius, -sunViewDot);
                float lunarEclipse01 = smoothstep(1 - _MoonRadius * _MoonRadius * 0.5, 1, -dot(_MoonDirection, sunDirection));
                moonCol.rgb *= lerp(lunarEclipseMask, float3(0.2, 0.05, 0), lunarEclipse01);

                // Constellations
                float4 constellationsCol = texCUBE(_ConstellationsCubeMap, rotatedTexCoords) * _ConstellationsFade;
                constellationsCol.a = constellationsCol.r * starStrength * fogBlend;

                // Planets
                float4 planetsCol = float4(0, 0, 0, 0);
                [unroll(8)]
                for (int index = 0; index < 8; index++) {
                    float3 planetDirection = _PlanetDirections[index];
                    float planetSize =  _PlanetSizes[index] * _PlanetsScale;
                    float4 planetColour = float4(_PlanetColours[index], 1);
                    float planetViewDot = dot(planetDirection, normViewDir);
                    planetsCol += DrawCelesital(planetViewDot, planetSize) * planetColour;
                }
                planetsCol.a *= starStrength * fogBlend;

                // Blend
                float4 starsPlanets = (starsCol + constellationsCol + planetsCol) * (1 - moonMask);
                float4 col = (starsPlanets + moonCol);
                col.a = saturate(moonCol.a + starsCol.a + planetsCol.a + constellationsCol.a);
                
                if (solarEclipse01 > 0 && starStrength <= 0) {
                    col.a += solarEclipse01 * 0.9 * downDot;
                    col.a = saturate(col.a);
                }

                col.rgb *= _Exposure;
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }
    }
}
