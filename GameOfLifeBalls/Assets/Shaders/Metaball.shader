Shader "Custom/Metaball"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0, 0, 1, 1)
    }
    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            float4 _Color;

            int _MetaballCount = 2;
            int _NumberOfMetaballs;
            float _MetaballRadius;
            float3 _MetaballData[1000];

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float mx = 0;

                for (int m = 0; m < _NumberOfMetaballs; ++m)
                {
                    mx += pow(_MetaballRadius, 2) / (pow(i.uv.x - _MetaballData[m].x, 2) + pow(i.uv.y - _MetaballData[m].y, 2));
                }

                if (mx > 1) {
                    return _Color;
                    float first = mx;
                    return fixed4(
                        _Color.r + first,
                        _Color.g + first,
                        _Color.b + first,
                        _Color.a
                    );
                    //return fixed4(first, 0, 0, first);
                }
                else  {
                    return fixed4(0, 0, 0, 0);
                }
            }


            
            ENDCG
        }
    }
}
