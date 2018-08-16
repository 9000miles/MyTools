using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(ConditionObserver))]
    public class ConditionObserverEditor : Editor
    {

        private bool actionFoldout = true;
        private LuaScriptWizard luaScriptWizard = null;
        private QuestPicker questPicker = null;

        public void OnEnable()
        {
            var trigger = target as ConditionObserver;
            if (trigger == null) return;
            if (EditorTools.selectedDatabase == null) EditorTools.selectedDatabase = EditorTools.FindInitialDatabase();
            luaScriptWizard = new LuaScriptWizard(EditorTools.selectedDatabase);
            questPicker = new QuestPicker(EditorTools.selectedDatabase, trigger.questName, trigger.useQuestNamePicker);
            questPicker.showReferenceDatabase = false;
        }

        public override void OnInspectorGUI()
        {
            var trigger = target as ConditionObserver;
            if (trigger == null) return;

            serializedObject.Update();

            // Reference database:
            EditorTools.selectedDatabase = EditorGUILayout.ObjectField("Reference Database", EditorTools.selectedDatabase, typeof(DialogueDatabase), false) as DialogueDatabase;

            // Frequency, once, observeGameObject:
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frequency"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("once"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("observeGameObject"), true);

            // Condition:
            EditorGUILayout.PropertyField(serializedObject.FindProperty("condition"), true);

            // Action:
            actionFoldout = EditorGUILayout.Foldout(actionFoldout, "Action");
            if (actionFoldout)
            {

                EditorWindowTools.StartIndentedSection();

                serializedObject.ApplyModifiedProperties();

                // Lua code / wizard:
                if (EditorTools.selectedDatabase != luaScriptWizard.database)
                {
                    luaScriptWizard.database = EditorTools.selectedDatabase;
                    luaScriptWizard.RefreshWizardResources();
                }
                trigger.luaCode = luaScriptWizard.Draw(new GUIContent("Lua Code", "The Lua code to run when the condition is true"), trigger.luaCode);

                // Quest:
                if (EditorTools.selectedDatabase != questPicker.database)
                {
                    questPicker.database = EditorTools.selectedDatabase;
                    questPicker.UpdateTitles();
                }
                questPicker.Draw();
                trigger.questName = questPicker.currentQuest;
                trigger.useQuestNamePicker = questPicker.usePicker;

                serializedObject.Update();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("questState"), true);

                // Alert message:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("alertMessage"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("localizedTextTable"), true);

                // Send Messages list:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sendMessages"), true);

                EditorWindowTools.EndIndentedSection();
            }

            serializedObject.ApplyModifiedProperties();
        }

    }

}
