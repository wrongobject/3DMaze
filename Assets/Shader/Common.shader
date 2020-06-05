Shader "Custom/Common"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {        		
       
		Pass{
			Tags{LightMode=ForwardBase}
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			CGPROGRAM
       
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0
			#pragma vertex vert
            #pragma fragment frag
			#define SHADOWS_SCREEN
			#include "UnityCG.cginc"
			#include "UnityStandardCore.cginc"
			 #pragma multi_compile_fwdadd_fullshadows
			struct Input
			{
				float3 vertex : POSITION;
				float3 normal : NORMAL;
				
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				
			};
			struct Output
			{
				float4 pos : SV_POSITION;
				
				float2 uv : TEXCOORD0;
				float3 normal :TEXCOORD1;
				float3 posWorld : TEXCOORD2;
				UNITY_SHADOW_COORDS(3)
			};
		
			Output vert(Input v)
			{
				Output o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.posWorld = mul(unity_ObjectToWorld,v.vertex).xyz;
				o.normal = UnityObjectToWorldNormal(v.normal);
				UNITY_TRANSFER_SHADOW(o, v.uv1);
				return o;
			}

			half4 frag(Output input) :SV_TARGET
			{
				half4 result;
				float3 pos = input.pos.xyz / input.pos.w;
				result = tex2D(_MainTex,input.uv) * _Color;
				float3 dir = UnityWorldSpaceLightDir(pos);
				UNITY_LIGHT_ATTENUATION(atten,input, input.posWorld);
				//result.rgb = result.rgb * atten;
				result.rgb = result.rgb * saturate(dot(dir,input.normal)) * atten  + UNITY_LIGHTMODEL_AMBIENT;
				return result ;
			}

			ENDCG
		}

		Pass{
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			Tags{LightMode=ShadowCaster }
			//Tags{"RenderQueue"="Transparent"}
			
			CGPROGRAM
			#pragma multi_compile_shadowcaster
			#pragma vertex vertCaster
			#pragma fragment fragCaster
			#include "UnityCG.cginc"
			
			struct Input
			{
				float3 vertex : POSITION;				
				float2 uv : TEXCOORD0;				
			};
			struct Output
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;				
			};
			sampler2D _MainTex;
			sampler3D   _DitherMaskLOD;
			Output vertCaster(Input input)
			{
				Output o;
				o.vertex = UnityObjectToClipPos(input.vertex);
				
				o.uv = input.uv;				
				return o;
			}

			half4 fragCaster(Output input) :SV_TARGET
			{
				half4 result;				
				result = tex2D(_MainTex,input.uv);	
				//half alphaRef = tex3D(_DitherMaskLOD, float3(input.vertex.xy*0.25,result.a*0.9375)).a;
				clip(result.a - 0.1);	
				//clip(alphaRef -0.01);
				return 0 ;
			}
			ENDCG
		}
	}
    //FallBack "Diffuse"
}
