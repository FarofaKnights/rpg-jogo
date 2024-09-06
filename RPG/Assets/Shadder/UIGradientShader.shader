Shader "Unlit/UIGradientShader"
{
    Properties
    {
        [PerRendererData]_MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", color) = (1, 1, 1, 1)
        
        _StencilComp ("Stencil Comparison", float) = 8
        _Stencil ("Stencil ID", float ) = 0
        _StencilOp ("Stencil Operation", float) = 0
        _StencilWriteMask ("Stencil Write Mask", float) = 255
        _StencilReadMask ("Stencil Read Mask", float ) = 255
        
        _ColorMask ("Color Mask", float) = 15

        _GradientColor1 ("Gradient Color 1", Color) = (1, 1, 1, 1)
        _GradientColor2 ("Gradient Color 2", Color) = (0, 0, 0, 0)
        _Power ("Power", float) = 1
        
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", float) = 0
    }
    SubShader
    {
        // Venho por meio deste comentário agradecer ao Lima (PedroBirchal no Github) por me ajudar na implementação de Shader de UI transparente
        // Usei o seguinte código como base: https://github.com/PedroBirchal/ShaderStudy/blob/main/TexturasProcedurais_PedroDeLimaBIrchal762971/Assets/Shaders/MapFade.shader

        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        

        Stencil{
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include  "UnityUI.cginc"

            // Importante pra UI
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            //Input
            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            fixed4 _GradientColor1;
            fixed4 _GradientColor2;
            half _Power;

            // Vertex Shader, um foreach por vertice
            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(o.worldPosition);
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.color = v.color * _Color;
                return o;
            }

            // Fragment Shader
            fixed4 frag (v2f i) : SV_Target
            {
                half4 color = (tex2D(_MainTex, i.uv) + _TextureSampleAdd) * i.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping( i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                float pos = pow(i.uv.y, _Power);
                half4 gradient = lerp(_GradientColor1, _GradientColor2, pos);
                
                return color * gradient;
            }
            ENDCG
        }
    }
}
