#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ColorUsageAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws Color properties marked with <see cref="UnityEngine.ColorUsageAttribute"/>.
    /// </summary>
    [OdinDrawer]
    public sealed class ColorUsageAttributeDrawer : OdinAttributeDrawer<ColorUsageAttribute, Color>, IDefinesGenericMenuItems
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<Color> entry, ColorUsageAttribute attribute, GUIContent label)
        {
            Rect rect = EditorGUILayout.GetControlRect();

            PropertyContext<ColorPickerHDRConfig> context;
            if (entry.Context.Get<ColorPickerHDRConfig>(this, "HdrConfig", out context))
            {
                context.Value = new ColorPickerHDRConfig(attribute.minBrightness, attribute.maxBrightness, attribute.minExposureValue, attribute.maxExposureValue);
            }

            entry.SmartValue = EditorGUI.ColorField(rect, label ?? GUIContent.none, entry.SmartValue, true, attribute.showAlpha, attribute.hdr, context.Value);
        }

        internal static void PopulateGenericMenu<T>(IPropertyValueEntry<T> entry, GenericMenu genericMenu)
        {
            Color color = (Color)(object)entry.SmartValue;

            Color colorInClipboard;
            bool hasColorInClipboard = ColorExtensions.TryParseString(EditorGUIUtility.systemCopyBuffer, out colorInClipboard);

            if (genericMenu.GetItemCount() > 0)
            {
                genericMenu.AddSeparator("");
            }

            genericMenu.AddItem(new GUIContent("Copy RGBA"), false, () =>
            {
                EditorGUIUtility.systemCopyBuffer = entry.SmartValue.ToString();
            });
            genericMenu.AddItem(new GUIContent("Copy HEX"), false, () =>
            {
                EditorGUIUtility.systemCopyBuffer = "#" + ColorUtility.ToHtmlStringRGBA(color);
            });
            genericMenu.AddItem(new GUIContent("Copy Color Code Declaration"), false, () =>
            {
                EditorGUIUtility.systemCopyBuffer = ColorExtensions.ToCSharpColor(color);
            });

            if (hasColorInClipboard)
            {
                genericMenu.ReplaceOrAdd("Paste", false, () =>
                {
                    entry.Property.Tree.DelayActionUntilRepaint(() =>
                    {
                        entry.SmartValue = (T)(object)colorInClipboard;
                    });

                    GUIHelper.RequestRepaint();
                });
            }
            else if (Clipboard.CanPaste(typeof(Color)) || Clipboard.CanPaste(typeof(Color32)))
            {
                genericMenu.ReplaceOrAdd("Paste", false, () =>
                {
                    entry.Property.Tree.DelayActionUntilRepaint(() =>
                    {
                        entry.SmartValue = (T)Clipboard.Paste();
                    });

                    GUIHelper.RequestRepaint();
                });
            }
            else
            {
                genericMenu.AddDisabledItem(new GUIContent("Paste"));
            }
        }

        void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            PopulateGenericMenu((IPropertyValueEntry<Color>)property.ValueEntry, genericMenu);
        }
    }
}
#endif