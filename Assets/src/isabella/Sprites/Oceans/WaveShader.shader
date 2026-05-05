Shader "Custom/WaveShader"
{
    Properties
    {
        _Color ("Water Color", Color) = (0,0.4,1,1)
        _Amplitude ("Wave Height", Float) = 0.1
        _Frequency ("Wave Frequency", Float) = 5
        _Speed ("Wave Speed", Float) = 2
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _Amplitude;
            float _Frequency;
            float _Speed;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                float3 pos = v.vertex.xyz;
                float t = _Time.y * _Speed;


                float2 dir1 = normalize(float2(1.0, 0.3));
                float2 dir2 = normalize(float2(-0.7, 1.0));
                float2 dir3 = normalize(float2(0.9, -0.8));
                float2 dir4 = normalize(float2(-1.0, -0.2));
                float2 dir5 = normalize(float2(0.2, 1.0));

                float p1 = dot(pos.xz, dir1);
                float p2 = dot(pos.xz, dir2);
                float p3 = dot(pos.xz, dir3);
                float p4 = dot(pos.xz, dir4);
                float p5 = dot(pos.xz, dir5);

                float wave1 = sin(p1 * _Frequency + t);
                float wave2 = sin(p2 * (_Frequency * 0.7) + t * 1.2 + 2.1);
                float wave3 = sin(p3 * (_Frequency * 1.3) + t * 0.9 + 4.7);
                float wave4 = sin(p4 * (_Frequency * 1.6) + t * 1.4 + 1.3);
                float wave5 = sin(p5 * (_Frequency * 0.5) + t * 1.7 + 3.9);


                wave1 = pow(abs(wave1), 1.5) * sign(wave1);
                wave2 = pow(abs(wave2), 1.3) * sign(wave2);
                wave3 = pow(abs(wave3), 1.7) * sign(wave3);


                float wave = (wave1 + wave2 + wave3 + wave4 + wave5) * 0.2;

                if (pos.y > 0)
                {
                    pos.y += wave * _Amplitude;
                }

                o.vertex = UnityObjectToClipPos(float4(pos, 1));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = _Color;
                col.a = 0.9; 
                return col;
            }

            ENDCG
        }
    }
}