using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(AlertTrigger))]
    public class AlertTriggerEditor : Editor
    {

        public void OnEnable()
        {
            EditorTools.SetInitialDatabaseIfNull();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trigger"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("localizedTextTable"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("message"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("once"), true);
            EditorTools.DrawReferenceDatabase();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("condition"), true);
            serializedObject.ApplyModifiedProperties();
        }

    }

}
