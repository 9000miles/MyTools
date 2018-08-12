#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="LazyEditorIcon.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Globalization;

namespace Sirenix.Utilities.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Lazy loading Editor Icon.
    /// </summary>
    [InitializeOnLoad]
    public class LazyEditorIcon : EditorIcon
    {
        private static readonly string iconShader = @"
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
        Blend SrcAlpha Zero
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
                float4 _Color;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = _Color;
                    float2 uv = i.uv;
                    uv *= _Rect.zw;
					uv += _Rect.xy;
					col.a *= tex2D(_MainTex, uv).a;
					return col;
				}
			ENDCG
		}
	}
}
";

        private static Color inactiveColorPro =     new Color(0.40f, 0.40f, 0.40f, 1);
        private static Color activeColorPro =       new Color(0.55f, 0.55f, 0.55f, 1);
        private static Color highlightedColorPro =  new Color(0.90f, 0.90f, 0.90f, 1);

        private static Color inactiveColor =        new Color(0.72f, 0.72f, 0.72f, 1);
        private static Color activeColor =          new Color(0.40f, 0.40f, 0.40f, 1);
        private static Color highlightedColor =     new Color(0.20f, 0.20f, 0.20f, 1);

        private static Material iconMat;

        private Sprite icon;
        private Texture inactive;
        private Texture active;
        private Texture highlighted;

        private string iconName;

        /// <summary>
        /// Loads an EditorIcon from the spritesheet.
        /// </summary>
        public LazyEditorIcon(string icon)
        {
            this.iconName = icon;
            ReloadIcon();
        }

        private void ReloadIcon()
        {
            this.icon = AssetDatabase.LoadAllAssetsAtPath("Assets/" + SirenixAssetPaths.SirenixAssetsPath + "Editor/Icons.png").OfType<Sprite>().First(x => x.name.ToLower() == this.iconName);

            if (this.highlighted != null) UnityEngine.Object.DestroyImmediate(this.highlighted);
            if (this.active != null) UnityEngine.Object.DestroyImmediate(this.active);
            if (this.inactive != null) UnityEngine.Object.DestroyImmediate(this.inactive);

            this.inactive = this.RenderIcon(EditorGUIUtility.isProSkin ? inactiveColorPro : inactiveColor);
            this.active = this.RenderIcon(EditorGUIUtility.isProSkin ? activeColorPro : activeColor);
            this.highlighted = this.RenderIcon(EditorGUIUtility.isProSkin ? highlightedColorPro : highlightedColor);
        }

        /// <summary>
        /// Gets the icon's highlight texture.
        /// </summary>
        public override Texture Highlighted
        {
            get
            {
                if (this.highlighted == null)
                {
                    this.ReloadIcon();
                }
                return this.highlighted;
            }
        }

        /// <summary>
        /// Gets the icon's active texture.
        /// </summary>
        public override Texture Active
        {
            get
            {
                if (this.active == null)
                {
                    this.ReloadIcon();
                }
                return this.active;
            }
        }

        /// <summary>
        /// Gets the icon's inactive texture.
        /// </summary>
        public override Texture Inactive
        {
            get
            {
                if (this.inactive == null)
                {
                    this.ReloadIcon();
                }
                return this.inactive;
            }
        }

        private Texture RenderIcon(Color color)
        {
            var rect = this.icon.rect;

            if (iconMat == null || iconMat.shader == null)
            {
                iconMat = new Material(ShaderUtil.CreateShaderAsset(iconShader));
            }

            iconMat.SetColor("_Color", color);
            iconMat.SetVector("_TexelSize", new Vector2(1f / this.icon.texture.width, 1f / this.icon.texture.height));
            iconMat.SetVector("_Rect", new Vector4(
                rect.x / this.icon.texture.width,
                rect.y / this.icon.texture.height,
                rect.width / this.icon.texture.width,
                rect.height / this.icon.texture.height
            ));

            RenderTexture prev = RenderTexture.active;
            var rt = new RenderTexture((int)rect.width, (int)rect.height, 0, RenderTextureFormat.ARGB32);
            rt.antiAliasing = 8;
            rt.filterMode = FilterMode.Bilinear;
            rt.Create();
            RenderTexture.active = rt;
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
            Graphics.Blit(this.icon.texture, rt, iconMat);
            GL.sRGBWrite = false;
            RenderTexture.active = prev;
            return rt;
        }

        /// <summary>
        /// Gets the name of the icon.
        /// </summary>
        public override string ToString()
        {
            return this.iconName.ToString(CultureInfo.InvariantCulture);
        }
    }
}
#endif