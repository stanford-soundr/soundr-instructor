Shader "Custom/ConeGridShader" {
    Properties {
        _CellSize ("Grid cell size", Float) = 0.5
        _LineWidth ("Grid line width", Float) = 0.1
        _GridColor ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        pass
        {
            Cull Back
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            struct VertIn {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct VertOut
            {
                float4 position : POSITION;
                float3 locpos : TEXCOORD0;
            };

            float _CellSize;
            float _LineWidth;
            float4 _GridColor;
 
            VertOut vert(VertIn input)
            {
                VertOut output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.locpos = input.uv;
                return output;
            }

            float getGridFact(float pos)
            {
                float snapPos = round(pos / _CellSize) * _CellSize;
                float dist = abs(snapPos - pos);
                return 1 - min(1.f, dist * 2.f / _LineWidth);
            }

            float4 frag(VertOut i) : COLOR
            {
                float factX = getGridFact(i.locpos.x);
                return _GridColor * float4(1.f, 1.f, 1.f, factX);
            }
            ENDCG
        }
    } 
    FallBack "Diffuse"
}