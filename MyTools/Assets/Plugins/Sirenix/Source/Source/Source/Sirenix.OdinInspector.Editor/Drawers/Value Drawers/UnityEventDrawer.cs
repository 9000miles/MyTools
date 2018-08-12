#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="UnityEventDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections;
    using System.Reflection;

    /// <summary>
    /// Unity event drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class UnityEventDrawer<T> : OdinValueDrawer<T> where T : UnityEventBase
    {
        private static readonly Action<SerializedProperty> ResetUnityEventDrawerState;

        static UnityEventDrawer()
        {
            try
            {
                var unityEventDrawer = typeof(Editor).Assembly.GetType("UnityEditorInternal.UnityEventDrawer");
                var scriptAttributeUtility = typeof(Editor).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
                var propertyHandler = typeof(Editor).Assembly.GetType("UnityEditor.PropertyHandler");

                var scriptAttributeUtility_getHandlerMethod = scriptAttributeUtility.GetMethod("GetHandler", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var propertyHandler_propertyDrawerField = propertyHandler.GetField("m_PropertyDrawer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var unityEventDrawer_statesField = unityEventDrawer.GetField("m_States", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (scriptAttributeUtility_getHandlerMethod == null
                    || propertyHandler_propertyDrawerField == null
                    || unityEventDrawer_statesField == null)
                {
                    throw new Exception();
                }

                ResetUnityEventDrawerState = (prop) =>
                {
                    object handler = scriptAttributeUtility_getHandlerMethod.Invoke(null, new object[] { prop });
                    object drawer = propertyHandler_propertyDrawerField.GetValue(handler);

                    if (drawer != null && drawer.GetType().Name == "UnityEventDrawer")
                    {
                        IDictionary states = (IDictionary)unityEventDrawer_statesField.GetValue(drawer);
                        states.Remove(prop.propertyPath);
                    }
                };
            }
            catch
            {
                Debug.LogWarning("Could not fetch internal Unity classes and members required for UnityEventDrawer to fix an internal Unity caching bug that causes havoc with UnityEvents rendered in InlineEditors that have been expanded and closed several times. Internal Unity members or types must have changed in the current version of Unity.");
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, GUIContent label)
        {
            var unityProperty = entry.Property.Tree.GetUnityPropertyForPath(entry.Property.Path);

            //if (unityProperty == null)
            //{
            //    SirenixEditorGUI.ErrorMessageBox("Could not create an alias UnityEditor.SerializedProperty for the property '" + entry.Property.Name + "'.");
            //    return;
            //}

            if (unityProperty == null || unityProperty.serializedObject.targetObject is EmittedScriptableObject<T> || entry.Property.Tree.UnitySerializedObject == null || (typeof(Component).IsAssignableFrom(entry.Property.Tree.UnitySerializedObject.targetObject.GetType()) == false))
            {
                SirenixEditorGUI.WarningMessageBox("Cannot properly draw UnityEvents for properties that are not directly serialized by Unity from a component. To get the classic Unity event appearance, please turn " + entry.Property.Name + " into a public field, or a private field with the [SerializedField] attribute on, and ensure that it is defined on a component.");
                this.CallNextDrawer(entry.Property, label);
            }
            else
            {
                if (ResetUnityEventDrawerState != null)
                {
                    var hasResetState = entry.Property.Context.Get(this, "hasResetState", false);

                    if (!hasResetState.Value)
                    {
                        ResetUnityEventDrawerState(unityProperty);
                        hasResetState.Value = true;
                    }
                }

                try
                {
                    EditorGUILayout.PropertyField(unityProperty, true);
                }
                catch (NullReferenceException)
                {
                    SirenixEditorGUI.ErrorMessageBox("Could not fix cached internal Unity state that causes an error when rendering UnityEvents in InlineEditors that have been expanded and closed several times. Please reselect the current object to fix the issue.");
                }
            }
        }
    }
}
#endif