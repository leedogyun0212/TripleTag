Shader "Custom/FOV_Fog_Volume"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (0, 0, 0, 1)
        _AlphaIntensity ("Alpha Intensity (Brightness)", Range(0, 1)) = 0.8
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off // ★ 큐브 안에서도 투명해지지 않고 안개가 정상 작동하게 함

        // ★ 핵심: 1번 도장이 찍히지 않은(NotEqual) 영역에만 안개를 칠하겠다!
        Stencil
        {
            Ref 1
            Comp NotEqual
            Pass Keep
        }

        Pass
        {
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

            fixed4 _FogColor;
            float _AlphaIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _FogColor;
                col.a = _AlphaIntensity; // 인스펙터의 슬라이더 값 적용
                return col;
            }
            ENDCG
        }
    }
}