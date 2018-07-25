using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(PersistentActiveData))]
    public class PersistentActiveDataEditor : Editor
    {

        public void OnEnable()
        {
            EditorTools.SetInitialDatabaseIfNull();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("target"), true);
            EditorTools.DrawReferenceDatabase();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("condition"), true);
            serializedObject.ApplyModifiedProperties();
        }

    }

}
