using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(OverrideActorName))]
    [CanEditMultipleObjects]
    public class OverrideActorNameEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var overrideActorName = target as OverrideActorName;
            if (overrideActorName == null) return;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("overrideName"), new GUIContent("Override Name", "Use this name instead of the GameObject name in conversations."), true);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("internalName"), new GUIContent("Internal Name", "Use this name instead of the GameObject name when saving persistent data. Leave blank to use Override Name."), true);
            if (GUILayout.Button(new GUIContent("Unique", "Assign a unique internal name."), GUILayout.Width(60)))
            {
                AssignUniqueInternalName();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("useLocalizedNameInDatabase"), new GUIContent("Use Localized Name In Database", "Use the localized version of the Override Actor Name defined in the dialogue database"), true);

            serializedObject.ApplyModifiedProperties();
        }

        private void AssignUniqueInternalName()
        {
            serializedObject.ApplyModifiedProperties();
            foreach (var t in targets)
            {
                var overrideActorName = t as OverrideActorName;
                if (overrideActorName == null) continue;
                Undo.RecordObject(overrideActorName, "Unique ID");
                overrideActorName.internalName = DialogueLua.StringToTableIndex(OverrideActorName.GetActorName(overrideActorName.transform) + "_" + overrideActorName.GetInstanceID());
                EditorUtility.SetDirty(overrideActorName);
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(overrideActorName.gameObject.scene);
#endif
            }
            serializedObject.Update();
        }

    }

}
