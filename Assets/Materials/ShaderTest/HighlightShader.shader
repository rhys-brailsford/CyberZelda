﻿Shader "Custom/HighlightShader"
{
    Properties
    {
        _UnselectedColor ("UnselectedColor", Color) = (1,0.5,0,1)
        _SelectedColor ("SelectedColor", Color) = (1,1,1,1)
        _Color ("Color", Color) = (1,1,1,1)
        [PerRendererData][MaterialToggle] _IsSelected ("IsSelected", Float) = 0
        [PerRendererData][MaterialToggle] _IsActive ("IsActive", Float) = 1
        _LineWidth ("LineWidth", Float) = 1
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Tags { "Queue" = "Transparent" }
        LOD 200

        // Outline pass
        Pass
        {
            Name "Outline"
            Tags { "RenderType"="Opaque" }
            Tags { "LightMode" = "Always" }
            ZWrite Off
            //AlphaToMask On
            Blend Off

            //ZTest Always
            //ColorMask RGB
            //Blend One OneMinusDstColor

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct vertexInput
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0; 
                float4 normal : NORMAL;
            };
            struct v2f 
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _LineWidth;
            fixed4 _UnselectedColor;
            fixed4 _SelectedColor;
            bool _IsSelected;
            bool _IsActive;

            v2f vert (vertexInput input)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(input.vertex + (input.normal * _LineWidth));
                o.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                if (!_IsActive)
                {
                    discard;
                }
                return _IsSelected ? _SelectedColor : _UnselectedColor;
            }
            ENDCG
        }

        

        Lighting On
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;


        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
          // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            //o.Alpha = 0;
        }
        ENDCG
        
    }
    FallBack "Diffuse"
}