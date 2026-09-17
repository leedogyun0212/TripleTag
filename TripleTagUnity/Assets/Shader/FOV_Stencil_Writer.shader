
Shader "Custom/FOV_Stencil_Writer"
{
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent-1" }
        LOD 100

        // ★ 핵심: 화면에 색은 그리지 않고(None), 스텐실 Buffer에 1번 도장만 찍기
        ColorMask 0
        ZWrite Off

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(1, 1, 1, 1);
            }
            ENDCG
        }
    }
}

