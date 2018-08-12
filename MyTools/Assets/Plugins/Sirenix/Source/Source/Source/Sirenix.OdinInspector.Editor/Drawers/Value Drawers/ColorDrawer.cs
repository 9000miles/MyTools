#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ColorDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities;
    using UnityEditor;
    using UnityEngine;
    using System;
    using Sirenix.Utilities.Editor;

    /// <summary>
    /// Color property drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class ColorDrawer : PrimitiveCompositeDrawer<Color>, IDefinesGenericMenuItems
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyField(IPropertyValueEntry<Color> entry, GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect();

            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            bool disableContext = false;

            if (Event.current.OnMouseDown(rect, 1, false))
            {
                // Disable Unity's color field's own context menu
                GUIHelper.PushEventType(EventType.Used);
                disableContext = true;
            }

            entry.SmartValue = EditorGUI.ColorField(rect, entry.SmartValue);

            if (disableContext)
            {
                GUIHelper.PopEventType();
            }
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