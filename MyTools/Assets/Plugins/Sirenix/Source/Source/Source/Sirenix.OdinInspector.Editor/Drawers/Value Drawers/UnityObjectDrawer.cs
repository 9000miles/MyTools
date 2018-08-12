#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="UnityObjectDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Unity object drawer.
    /// </summary>
    [OdinDrawer]
    [DrawerPriority(0, 0, 0.25)] // Set priority so that vanilla Unity CustomPropertyDrawers can draw UnityObject types by default
    public sealed class UnityObjectDrawer<T> : OdinValueDrawer<T> where T : UnityEngine.Object
    {
        private static readonly bool IsSprite = typeof(Sprite).IsAssignableFrom(typeof(T));

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, GUIContent label)
        {
            if (IsSprite && entry.ValueCategory == PropertyValueCategory.Member)
            {
                // Sprites that are not in lists are drawn with a special layout, so we need to use EditorGUILayout.ObjectField
                //   instead of EditorGUI.ObjectField, which will not draw them properly.

                bool iconMouseEvent = false;

                var spriteRect = entry.Context.Get(this, "sprite_rect", default(Rect));

                if (Event.current.isMouse && Event.current.type != EventType.MouseMove && spriteRect.Value != default(Rect) && spriteRect.Value.Contains(Event.current.mousePosition))
                {
                    // Just ignore this nice mouse event, heheh *innocent whistle*
                    GUIHelper.PushEventType(EventType.MouseMove);
                    iconMouseEvent = true;
                }

                EditorGUI.BeginChangeCheck();

                entry.WeakSmartValue = label == null ?
                    EditorGUILayout.ObjectField((UnityEngine.Object)entry.WeakSmartValue, entry.BaseValueType, entry.Property.Info.GetAttribute<AssetsOnlyAttribute>() == null) :
                    EditorGUILayout.ObjectField(label, (UnityEngine.Object)entry.WeakSmartValue, entry.BaseValueType, entry.Property.Info.GetAttribute<AssetsOnlyAttribute>() == null);

                if (EditorGUI.EndChangeCheck())
                {
                    entry.Values.ForceMarkDirty();
                }

                if (iconMouseEvent)
                {
                    GUIHelper.PopEventType();
                }

                if (Event.current.type != EventType.Layout)
                {
                    Rect rect;

                    rect = GUILayoutUtility.GetLastRect();
                    rect = new Rect(rect.x + rect.width - 19, rect.y, 19, 19);
                    spriteRect.Value = rect;

                    var obj = (UnityEngine.Object)entry.WeakSmartValue;

                    GUIHelper.PushGUIEnabled(obj != null && AssetDatabase.Contains(obj) && !EditorGUI.showMixedValue);

                    if (GUI.Button(rect, GUIHelper.TempContent("", null, "Inspect object"), SirenixGUIStyles.Button) && Event.current.button == 0)
                    {
                        if (entry.WeakSmartValue != null)
                        {
                            var path = AssetDatabase.GetAssetPath(obj);
                            var asset = AssetDatabase.LoadMainAssetAtPath(path) ?? obj;

                            if (asset != null)
                            {
                                GUIHelper.OpenInspectorWindow(asset);
                            }
                        }
                    }

                    EditorIcons.Pen.Draw(rect);

                    GUIHelper.PopGUIEnabled();
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();

                entry.WeakSmartValue = SirenixEditorFields.UnityObjectField(
                    label,
                    (UnityEngine.Object)entry.WeakSmartValue,
                    entry.BaseValueType,
                    entry.Property.Info.GetAttribute<AssetsOnlyAttribute>() == null);

                if (EditorGUI.EndChangeCheck())
                {
                    entry.Values.ForceMarkDirty();
                }
            }
        }
    }
}
#endif