#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="TextureUtilities.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities.Editor
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Collection of texture functions.
    /// </summary>
    public static class TextureUtilities
    {
        private static Material extractSpriteMaterial;

        private static readonly string extractSpriteShader = @"
            Shader ""Hidden/Sirenix/Editor/GUIIcon""
            {
	            Properties
	            {
                    _MainTex(""Texture"", 2D) = ""white"" {}
                    _Color(""Color"", Color) = (1,1,1,1)
                    _Rect(""Rect"", Vector) = (0,0,0,0)
                    _TexelSize(""TexelSize"", Vector) = (0,0,0,0)
	            }
                SubShader
	            {
                    Blend SrcAlpha OneMinusSrcAlpha
                    Pass
                    {
                        CGPROGRAM
                            " + "#" + @"pragma vertex vert
                            " + "#" + @"pragma fragment frag
                            " + "#" + @"include ""UnityCG.cginc""

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

                            sampler2D _MainTex;
                            float4 _Rect;

                            v2f vert(appdata v)
                            {
                                v2f o;
                                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                                o.uv = v.uv;
                                return o;
                            }

                            fixed4 frag(v2f i) : SV_Target
				            {
                                float2 uv = i.uv;
                                uv *= _Rect.zw;
					            uv += _Rect.xy;
					            return tex2D(_MainTex, uv);
				            }
			            ENDCG
		            }
	            }
            }";

        /// <summary>
        /// Crops a Texture2D into a new Texture2D.
        /// </summary>
        public static Texture2D CropTexture(this Texture2D texture, Rect source)
        {
            RenderTexture prev = RenderTexture.active;
            var rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 8);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;
            GL.Clear(false, true, new Color(1, 1, 1, 0));
            Graphics.Blit(texture, rt);

            Texture2D clone = new Texture2D((int)source.width, (int)source.height, texture.format, texture.mipmapCount > 1);
            clone.filterMode = FilterMode.Point;
            clone.ReadPixels(source, 0, 0);
            clone.Apply();
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
            return clone;
        }

        /// <summary>
        /// Converts a Sprite to a Texture.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="posProcessPasses"></param>
        /// <returns></returns>
        public static Texture ConvertSpriteToTexture(Sprite sprite, params Material[] posProcessPasses)
        {
            var rect = sprite.rect;

            if (extractSpriteMaterial == null || extractSpriteMaterial.shader == null)
            {
                extractSpriteMaterial = new Material(ShaderUtil.CreateShaderAsset(extractSpriteShader));
            }

            extractSpriteMaterial.SetVector("_TexelSize", new Vector2(1f / sprite.texture.width, 1f / sprite.texture.height));
            extractSpriteMaterial.SetVector("_Rect", new Vector4(
                rect.x / sprite.texture.width,
                rect.y / sprite.texture.height,
                rect.width / sprite.texture.width,
                rect.height / sprite.texture.height
            ));

            RenderTexture prev = RenderTexture.active;
            var rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
            rt.Create();
            RenderTexture.active = rt;
            GL.Clear(false, true, new Color(1, 1, 1, 0));
            Graphics.Blit(sprite.texture, rt, extractSpriteMaterial);

            if (posProcessPasses != null)
            {
                for (int i = 0; i < posProcessPasses.Length; i++)
                {
                    Graphics.Blit(sprite.texture, rt, posProcessPasses[i]);
                }
            }

            RenderTexture.active = prev;
            return rt;
        }
    }
}
#endif