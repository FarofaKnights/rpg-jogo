Shader "Unlit/UITiltedGradientShader"
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

        _GradientColorStart ("Gradient Color Start", Color) = (1, 1, 1, 1)
        _GradientColorMid ("Gradient Color Middle", Color) = (0, 0, 0, 0)
        _GradientColorEnd ("Gradient Color End", Color) = (1, 1, 1, 1)
        
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

            fixed4 _GradientColorStart;
            fixed4 _GradientColorMid;
            fixed4 _GradientColorEnd;

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
            
            // Não sei rodar as coisas da UV, então importei esse código do ShaderGraph (um dia eu aprendo)
            // Nodo de rotate do ShaderGraph
            void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
            {
                Rotation = Rotation * (3.1415926f/180.0f);
                UV -= Center;
                float s = sin(Rotation);
                float c = cos(Rotation);
                float2x2 rMatrix = float2x2(c, -s, s, c);
                rMatrix *= 0.5;
                rMatrix += 0.5;
                rMatrix = rMatrix * 2 - 1;
                UV.xy = mul(UV.xy, rMatrix);
                UV += Center;
                Out = UV;
            }
            
            // Esse código é meu e eu estou traduzindo > diretamente < do ShaderGraph que fiz previamente
            // Não se assuste, eu não sei o que estou fazendo!
            fixed4 CalculateGradient(v2f i) {
                float x = i.uv.x;
                float y = i.uv.y;
                
                // Em questões matemáticas, esse não é o jeito de calcular gradientes, porém foi o jeito
                // que eu achei mais bonitinho pro efeito que eu queria
                fixed4 gradBaixo = pow(x * (1 - y), 2) * _GradientColorStart;
                fixed4 gradCima = pow((1 - x) * y, 2) * _GradientColorEnd;

                // Note que o gradiente do meio não é as partes que sobraram do gradiente de cima e de baixo
                float2 uv = i.uv;
                float2 uv2;
                Unity_Rotate_Degrees_float(uv, float2(0.5, 0.5), -45, uv2);
                // Vey essa parte é muito estranha, mas se vc tentar simplificar essa conta, continua estranha, ent deixa assim
                fixed4 gradMeio = (1-pow(uv2.x, 2)) * (1-pow(1 - uv2.x, 2)) * _GradientColorMid;
                
                gradBaixo.a = _GradientColorStart.a;
                gradMeio.a = _GradientColorMid.a;
                gradCima.a = _GradientColorEnd.a;
                
                return gradBaixo + gradCima + gradMeio;
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
                
                fixed4 gradient = CalculateGradient(i);
                return color * gradient;
            }
            ENDCG
        }
    }
}
