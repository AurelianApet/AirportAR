// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.35 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.35;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:33837,y:32638,varname:node_3138,prsc:2|emission-2314-OUT,alpha-6888-A,clip-1710-A;n:type:ShaderForge.SFN_Tex2d,id:1710,x:32843,y:32531,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:_DIfuse,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:faae31c917b29a44fba121ad3f2ee6e0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:4194,x:32832,y:32360,ptovrint:False,ptlb:dif_color,ptin:_dif_color,varname:_dif_color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:9521,x:33117,y:32526,varname:node_9521,prsc:2|A-1710-RGB,B-1710-A,C-4194-RGB;n:type:ShaderForge.SFN_Multiply,id:6517,x:33277,y:32871,varname:node_6517,prsc:2|A-3820-OUT,B-6888-RGB;n:type:ShaderForge.SFN_Tex2d,id:6888,x:32935,y:32889,ptovrint:False,ptlb:Tex_mask,ptin:_Tex_mask,varname:_light_sweep,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:True,tagnrm:False,tex:4f6b8d6d5799dcb409a68dac5d0258d4,ntxv:2,isnm:False|UVIN-3171-UVOUT;n:type:ShaderForge.SFN_Slider,id:3820,x:33092,y:33129,ptovrint:False,ptlb:Mask_strenght,ptin:_Mask_strenght,varname:_light,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_TexCoord,id:7530,x:32532,y:32889,varname:node_7530,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Rotator,id:3171,x:32717,y:32889,varname:node_3171,prsc:2|UVIN-7530-UVOUT;n:type:ShaderForge.SFN_Multiply,id:2314,x:33541,y:32744,varname:node_2314,prsc:2|A-9521-OUT,B-6517-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:2576,x:32644,y:32625,ptovrint:False,ptlb:node_2576,ptin:_node_2576,varname:node_2576,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:1710-4194-6888-3820;pass:END;sub:END;*/

Shader "IIA/IIA_mask_shader" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _dif_color ("dif_color", Color) = (1,1,1,1)
        [NoScaleOffset]_Tex_mask ("Tex_mask", 2D) = "black" {}
        _Mask_strenght ("Mask_strenght", Range(0, 2)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _dif_color;
            uniform sampler2D _Tex_mask;
            uniform float _Mask_strenght;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                clip(_Diffuse_var.a - 0.5);
////// Lighting:
////// Emissive:
                float3 node_9521 = (_Diffuse_var.rgb*_Diffuse_var.a*_dif_color.rgb);
                float4 node_3044 = _Time + _TimeEditor;
                float node_3171_ang = node_3044.g;
                float node_3171_spd = 1.0;
                float node_3171_cos = cos(node_3171_spd*node_3171_ang);
                float node_3171_sin = sin(node_3171_spd*node_3171_ang);
                float2 node_3171_piv = float2(0.5,0.5);
                float2 node_3171 = (mul(i.uv0-node_3171_piv,float2x2( node_3171_cos, -node_3171_sin, node_3171_sin, node_3171_cos))+node_3171_piv);
                float4 _Tex_mask_var = tex2D(_Tex_mask,node_3171);
                float3 node_6517 = (_Mask_strenght*_Tex_mask_var.rgb);
                float3 emissive = (node_9521*node_6517);
                float3 finalColor = emissive;
                return fixed4(finalColor,_Tex_mask_var.a);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                clip(_Diffuse_var.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
