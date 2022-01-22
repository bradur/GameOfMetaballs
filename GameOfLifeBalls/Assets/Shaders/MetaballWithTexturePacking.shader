Shader "Custom/MetaballWithTexturePacking"
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

            //sampler2D _MainTex;
            sampler2D _XPosTexture;
            sampler2D _YPosTexture;
            uniform float4 _XPosTexture_TexelSize;
            uniform float4 _YPosTexture_TexelSize;
            int _MetaballTextureWidth;
            float _MultiplyValue;
            float4 _Color;

            float4 x1;
            float4 y1;

            int _NumberOfMetaballs;
            float _MetaballRadius;

            float FloatFromTexture(float x, float y, bool xPos) {
                if (xPos) {
                    float2 uv = float2(x, y) * _XPosTexture_TexelSize.xy;
                    float4 col = tex2D(_XPosTexture, uv);
                    return DecodeFloatRGBA(col) * _MultiplyValue;
                }
                else {
                    float2 uv = float2(x, y) * _YPosTexture_TexelSize.xy;
                    float4 col = tex2D(_YPosTexture, uv);
                    return DecodeFloatRGBA(col) * _MultiplyValue;
                }
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float mx = 0;
                float x = 0;
                float y = 0;
                
                for(int m = 0; m < _NumberOfMetaballs; ++m) {
                    if (x >= _MetaballTextureWidth) {
                        x = 0;
                        y += 1;
                    }
                    float xPos = FloatFromTexture(x, y, true);
                    float yPos = FloatFromTexture(x, y, false);
                    if (xPos == 0 && yPos == 0) { 
                        x += 1;
                        continue;
                    }
                    mx += pow(_MetaballRadius, 2) / (pow(i.uv.x - xPos, 2) + pow(i.uv.y - yPos, 2));
                    x += 1;
                }

                if (mx > 1)
                return _Color;
                else 
                return fixed4(0, 0, 0, 0);
            }


            
            ENDCG
        }
    }
}
