Shader "Custom/GPUTreeShaderURP" {
    Properties {
        _MainTex ("Base Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Geometry" 
        }
        
        Pass {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct TreeData { 
                float4x4 mat; 
            };

            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                StructuredBuffer<TreeData> visibleTreesBuffer;
            #endif

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                uint instanceID   : SV_InstanceID;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD1;
                float3 positionWS : TEXCOORD3;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Texture2D _MainTex;
            SamplerState sampler_MainTex;

            void setup() {
                #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                    float4x4 data = visibleTreesBuffer[unity_InstanceID].mat;
                    unity_ObjectToWorld = data;
                #endif
            }

            Varyings vert(Attributes input) {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                float4 worldPos = mul(unity_ObjectToWorld, input.positionOS);
                output.positionWS = worldPos.xyz;
                output.positionCS = mul(unity_MatrixVP, worldPos);
                
                output.normalWS = mul((float3x3)unity_ObjectToWorld, input.normalOS);
                output.uv = input.uv;
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(input);

                half4 textureColor = _MainTex.Sample(sampler_MainTex, input.uv);
                
                // Fetch direct main directional sun light data
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                
                half3 normal = normalize(input.normalWS);
                half intensity = saturate(dot(normal, mainLight.direction));
                
                // FIXED: Replaced the unstable ambient macro function with basic environmental bounce approximation
                half3 ambientLight = half3(0.2, 0.25, 0.3); // Safe subtle blue sky ambient tint
                half3 finalLighting = (mainLight.color * intensity * mainLight.shadowAttenuation) + ambientLight;
                
                return half4(textureColor.rgb * finalLighting, textureColor.a);
            }
            ENDHLSL
        }
    }
}
