#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="OdinEditorWindow.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using UnityEditor;
    using UnityEngine;
    using Sirenix.Serialization;
    using Sirenix.Utilities.Editor;

    /// <summary>
    /// Not yet documented.
    /// </summary>
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class OdinEditorWindow : EditorWindow, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector, ExcludeDataFromInspector]
        private SerializationData serializationData;

        private PropertyTree propertyTree;

        private Vector2 scrollPos;

        private static GUIStyle margin;

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public PropertyTree PropertyTree
        {
            get
            {
                if (this.propertyTree == null)
                {
                    this.propertyTree = PropertyTree.Create(this);
                }

                return this.propertyTree;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
            this.OnAfterDeserialize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
            this.OnBeforeSerialize();
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        protected virtual void OnGUI()
        {
            margin = margin ?? new GUIStyle() { margin = new RectOffset(4, 4, 4, 4), padding = new RectOffset(4, 4, 0, 0) };

            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);
            GUILayout.BeginVertical(margin);
            {
                GUIHelper.PushHierarchyMode(false);
                this.PropertyTree.Draw();
                GUIHelper.PopHierarchyMode();
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            this.RepaintIfRequested();
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        protected virtual void OnAfterDeserialize()
        {
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        protected virtual void OnBeforeSerialize()
        {
        }
    }
}
#endif