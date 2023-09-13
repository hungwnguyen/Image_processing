Shader "Custom/Brightness" {
    Properties {
        _Brightness ("Brightness", Range (0.0, 5.0)) = 1
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            uniform sampler2D _MainTex;
            uniform float _Brightness;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
               fixed4 col = tex2D(_MainTex, i.uv);
               //col.rgb = log(1.0 + col.rgb);
               col.rgb *= _Brightness; 
               return col;
            }

            ENDCG
        }
    }
}
