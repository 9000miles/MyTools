using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(LuaTrigger))]
    public class LuaTriggerEditor : Editor
    {

        private LuaScriptWizard luaScriptWizard = null;

        public void OnEnable()
        {
            if (EditorTools.selectedDatabase == null) EditorTools.selectedDatabase = EditorTools.FindInitialDatabase();
            luaScriptWizard = new LuaScriptWizard(EditorTools.selectedDatabase);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var trigger = target as LuaTrigger;
            if (trigger == null || luaScriptWizard == null) return;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trigger"), true);

            var newDatabase = EditorGUILayout.ObjectField("Reference Database", luaScriptWizard.database, typeof(DialogueDatabase), false) as DialogueDatabase;
            if (newDatabase != luaScriptWizard.database)
            {
                EditorTools.selectedDatabase = newDatabase;
                luaScriptWizard.database = newDatabase;
                luaScriptWizard.RefreshWizardResources();
            }

            // Lua code / wizard:
            serializedObject.ApplyModifiedProperties();
            trigger.luaCode = luaScriptWizard.Draw(new GUIContent("Lua Code", "The Lua code to run"), trigger.luaCode);
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("once"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("condition"), true);
            serializedObject.ApplyModifiedProperties();
        }

    }

}
