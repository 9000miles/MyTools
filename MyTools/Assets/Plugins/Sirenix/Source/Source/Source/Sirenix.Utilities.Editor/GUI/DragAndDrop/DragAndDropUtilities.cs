#if UNITY_EDITOR

//-----------------------------------------------------------------------
// <copyright file="DragAndDropManager.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities.Editor
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Drag and drop utilities for both Unity and non-unity objects.
    /// </summary>
    public static class DragAndDropUtilities
    {
        private static bool currentDragIsMove;
        private static int draggingId;
        private static bool isAccepted;
        private static object dropZoneObject;
        private static object[] dragginObjects = new object[] { };

        /// <summary>
        /// Gets a more percistent id for drag and drop.
        /// </summary>
        public static int GetDragAndDropId(Rect rect)
        {
            var pos = GUIUtility.GUIToScreenPoint(rect.position);
            return 10000 + Mathf.Abs(pos.GetHashCode());
        }

        /// <summary>
        /// Draws a objectpicker butter, in the given rect. This one is designed to look good on top of DrawDropZone().
        /// </summary>
        public static object ObjectPickerZone(Rect rect, object value, Type type, bool allowSceneObjects, GUIStyle style = null)
        {
            var id = GUIUtility.GetControlID(444412, FocusType.Passive);
            var e = Event.current.type;
            var objectPicker = ObjectPicker.GetObjectPicker(type.FullName + "+" + id, type);

            if (e == EventType.MouseMove)
            {
                return value;
            }

            if (rect.Contains(Event.current.mousePosition))
            {
                GUIHelper.RequestRepaint();
                bool showObjectPicker = false;

                if (value != null)
                {
                    var btnRect = rect.Padding(1).SetHeight(14).AlignRight(14);
                    if (GUI.Button(btnRect, GUIContent.none, style ?? SirenixGUIStyles.ToolbarButton))
                    {
                        showObjectPicker = true;
                    }

                    EditorIcons.StarPointer.Draw(btnRect);
                }
                else if (DragAndDrop.activeControlID == 0)
                {
                    var btnRect = rect.Padding(4);
                    if (GUI.Button(btnRect, GUIContent.none, style ?? SirenixGUIStyles.ToolbarButton))
                    {
                        showObjectPicker = true;
                    }

                    EditorIcons.StarPointer.Draw(btnRect, 16);
                }

                if (showObjectPicker)
                {
                    objectPicker.ShowObjectPicker(allowSceneObjects, rect, false);
                }
            }

            if (objectPicker.IsReadyToClaim)
            {
                GUIHelper.RequestRepaint();
                GUI.changed = true;
                var newValue = objectPicker.ClaimObject();

                return newValue;
            }

            return value;
        }

        /// <summary>
        /// Draws a objectpicker butter, in the given rect. This one is designed to look good on top of DrawDropZone().
        /// </summary>
        public static T ObjectPickerZone<T>(Rect rect, T value, bool allowSceneObjects)
        {
            return (T)ObjectPickerZone(rect, value, typeof(T), allowSceneObjects);
        }

        /// <summary>
        /// Draws the graphics for a DropZone.
        /// </summary>
        public static void DrawDropZone<T>(Rect rect, T value, GUIContent label, int id)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var unityObject = value as UnityEngine.Object;
                GUIStyle objectFieldThumb = EditorStyles.objectFieldThumb;
                objectFieldThumb.Draw(rect, GUIContent.none, id, DragAndDrop.activeControlID == id);

                if (unityObject)
                {
                    if (unityObject is Component)
                    {
                        unityObject = (unityObject as Component).gameObject;
                    }

                    Texture image;
                    image = AssetPreview.GetAssetPreview(unityObject);
                    if (image == null)
                    {
                        image = AssetPreview.GetMiniThumbnail(unityObject);
                    }

                    rect = rect.Padding(1);
                    float size = Mathf.Min(rect.width, rect.height);

                    GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
                    EditorGUI.DrawTextureTransparent(rect.AlignCenter(size, size), image);
                    GL.sRGBWrite = false;

                    if (label != null)
                    {
                        rect = rect.AlignBottom(16);
                        GUI.Label(rect, label, EditorStyles.label);
                    }
                }
            }
        }

        /// <summary>
        /// A drop zone area for bot Unity and non-unity objects.
        /// </summary>
        public static object DropZone(Rect rect, object value, Type type, int id)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                var t = Event.current.type;

                if (draggingId == id)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                }
                else if (t == EventType.DragUpdated || t == EventType.DragPerform)
                {
                    object obj = null;

                    if (obj == null) obj = dragginObjects.Where(x => x != null && x.GetType().InheritsFrom(type)).FirstOrDefault();
                    if (obj == null) obj = DragAndDrop.objectReferences.Where(x => x != null && x.GetType().InheritsFrom(type)).FirstOrDefault();

                    if (type.InheritsFrom<Component>() || type.IsInterface)
                    {
                        if (obj == null) obj = dragginObjects.OfType<GameObject>().Where(x => x != null).Select(x => x.GetComponent(type)).Where(x => x != null).FirstOrDefault();
                        if (obj == null) obj = DragAndDrop.objectReferences.OfType<GameObject>().Where(x => x != null).Select(x => x.GetComponent(type)).Where(x => x != null).FirstOrDefault();
                    }

                    bool acceptsDrag = obj != null;

                    if (acceptsDrag)
                    {
                        bool move = Event.current.modifiers != EventModifiers.Shift && draggingId != 0 && currentDragIsMove;
                        if (move)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        }
                        else
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        }

                        Event.current.Use();
                        if (t == EventType.DragPerform)
                        {
                            if (!move)
                            {
                                draggingId = 0;
                            }

                            DragAndDrop.objectReferences = new UnityEngine.Object[] { };
                            DragAndDrop.AcceptDrag();
                            GUI.changed = true;
                            GUI.FocusControl(null);
                            GUIUtility.hotControl = 0;
                            dragginObjects = new object[] { };
                            currentDragIsMove = false;
                            isAccepted = true;
                            dropZoneObject = value;
                            DragAndDrop.activeControlID = 0;
                            GUIHelper.RequestRepaint();
                            return obj;
                        }
                        else
                        {
                            DragAndDrop.activeControlID = id;
                        }
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// A draggable zone for both Unity and non-unity objects.
        /// </summary>
        public static object DragZone(Rect rect, object value, Type type, bool allowMove, bool allowSwap, int id)
        {
            if (value == null) return null;

            var t = Event.current.type;
            var isMouseOver = rect.Contains(Event.current.mousePosition);
            var unityObject = value as UnityEngine.Object;

            if (isMouseOver && t == EventType.MouseDown)
            {
                GUIUtility.hotControl = id;
                GUI.FocusControl(null);
                dragginObjects = new object[] { };
                DragAndDrop.PrepareStartDrag();
                GUIHelper.RequestRepaint();
                isAccepted = false;
                dropZoneObject = null;
                draggingId = 0;
                currentDragIsMove = false;
                //Event.current.Use();
            }

            if (isAccepted && draggingId == id)
            {
                GUIHelper.RequestRepaint();
                GUI.changed = true;
                draggingId = 0;

                return allowMove ? (allowSwap ? dropZoneObject : null) : value;
            }

            if (GUIUtility.hotControl != id)
            {
                return value;
            }
            else if (t == EventType.MouseMove)
            {
                GUIUtility.hotControl = 0;
                GUIHelper.RequestRepaint();
                draggingId = 0;
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new UnityEngine.Object[] { };
                GUI.FocusControl(null);
                dragginObjects = new object[] { };
                currentDragIsMove = false;
            }

            if (Event.current.type == EventType.MouseDrag && (DragAndDrop.objectReferences == null || DragAndDrop.objectReferences.Length == 0))
            {
                isAccepted = false;
                dropZoneObject = null;
                draggingId = id;
                DragAndDrop.StartDrag("Movable drag");
                if (unityObject)
                {
                    DragAndDrop.objectReferences = new UnityEngine.Object[] { unityObject };
                    dragginObjects = new object[] { };
                }
                else
                {
                    DragAndDrop.objectReferences = new UnityEngine.Object[] { };
                    dragginObjects = new object[] { value };
                }

                DragAndDrop.activeControlID = 0;
                currentDragIsMove = allowMove;
                Event.current.Use();
                GUIHelper.RequestRepaint();
            }

            return value;
        }

        /// <summary>
        /// A drop zone area for bot Unity and non-unity objects.
        /// </summary>
        public static T DropZone<T>(Rect rect, T value, int id)
        {
            return (T)DropZone(rect, value, typeof(T), id);
        }

        /// <summary>
        /// A drop zone area for bot Unity and non-unity objects.
        /// </summary>
        public static object DropZone(Rect rect, object value, Type type)
        {
            var id = GetDragAndDropId(rect);
            return DropZone(rect, value, type, id);
        }

        /// <summary>
        /// A drop zone area for bot Unity and non-unity objects.
        /// </summary>
        public static T DropZone<T>(Rect rect, T value)
        {
            var id = GetDragAndDropId(rect);
            return (T)DropZone(rect, value, typeof(T), id);
        }

        /// <summary>
        /// A draggable zone for both Unity and non-unity objects.
        /// </summary>
        public static T DragZone<T>(Rect rect, T value, bool allowMove, bool allowSwap, int id)
        {
            return (T)DragZone(rect, value, typeof(T), allowMove, allowSwap, id);
        }

        /// <summary>
        /// A draggable zone for both Unity and non-unity objects.
        /// </summary>
        public static object DragZone(Rect rect, object value, Type type, bool allowMove, bool allowSwap)
        {
            var id = GetDragAndDropId(rect);
            return DragZone(rect, value, type, allowMove, allowSwap, id);
        }

        /// <summary>
        /// A draggable zone for both Unity and non-unity objects.
        /// </summary>
        public static object DragAndDropZone(Rect rect, object value, Type type, bool allowMove, bool allowSwap)
        {
            var id = GetDragAndDropId(rect);
            value = DropZone(rect, value, type, id);
            value = DragZone(rect, value, type, allowMove, allowSwap, id);
            return value;
        }

        /// <summary>
        /// A draggable zone for both Unity and non-unity objects.
        /// </summary>
        public static T DragZone<T>(Rect rect, T value, bool allowMove, bool allowSwap)
        {
            var id = GetDragAndDropId(rect);
            return (T)DragZone(rect, value, typeof(T), allowMove, allowSwap, id);
        }
    }
}

#endif