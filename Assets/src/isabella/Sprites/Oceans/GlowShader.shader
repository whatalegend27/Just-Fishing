Shader "Custom/GlowRadial"
{
    Properties
    {
        _Color ("Glow Color", Color) = (1,0.8,0.3,1)
        _Intensity ("Intensity", Float) = 2
        _Power ("Falloff", Float) = 2
        _Alpha ("Alpha", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _Intensity;
            float _Power;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // center UV space (-1 to 1)
                float2 uv = i.uv * 2 - 1;

                // distance from center
                float dist = length(uv);

                // radial falloff (core glow shape)
                float glow = pow(1 - saturate(dist), _Power);

                // final color
                float3 col = _Color.rgb * glow * _Intensity;

                return float4(col, glow * _Alpha);
            }
            ENDCG
        }
    }
}