using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(ConversationTrigger))]
    public class ConversationTriggerEditor : Editor
    {

        //--- We use [ConversationPopup] now:
        //private ConversationPicker conversationPicker = null;

        //public void OnEnable()
        //{
        //    InitConversationPicker();
        //    EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
        //    EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
        //}

        //public void OnDisable()
        //{
        //    EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
        //}

        //private void OnPlaymodeStateChanged()
        //{
        //    InitConversationPicker();
        //}

        //private void InitConversationPicker()
        //{
        //    var trigger = target as ConversationTrigger;
        //    if (trigger != null)
        //    {
        //        conversationPicker = new ConversationPicker(trigger.selectedDatabase, trigger.conversation, trigger.useConversationTitlePicker);
        //    }
        //}

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var trigger = target as ConversationTrigger;
            if (trigger == null) return;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trigger"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("conversation"), true);

            //if (conversationPicker != null)
            //{
            //    var oldUsePicker = conversationPicker.usePicker;
            //    var oldDatabase = conversationPicker.database;
            //    var conversationChanged = conversationPicker.Draw();
            //    var usePickerChanged = conversationPicker.usePicker != oldUsePicker;
            //    var databaseChanged = conversationPicker.database != oldDatabase;
            //    if (conversationChanged || usePickerChanged || databaseChanged)
            //    {
            //        serializedObject.ApplyModifiedProperties();
            //        trigger.conversation = conversationPicker.currentConversation;
            //        trigger.useConversationTitlePicker = conversationPicker.usePicker;
            //        trigger.selectedDatabase = conversationPicker.database;
            //        serializedObject.Update();
            //    }
            //}
            //else
            //{
            //    EditorGUILayout.PropertyField(serializedObject.FindProperty("conversation"));
            //}
            //if (trigger.selectedDatabase != null) EditorTools.selectedDatabase = trigger.selectedDatabase;
            //if (EditorTools.selectedDatabase == null) EditorTools.selectedDatabase = EditorTools.FindInitialDatabase();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("actor"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("conversant"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("once"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("exclusive"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("skipIfNoValidEntries"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stopConversationOnTriggerExit"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("condition"), true);
            serializedObject.ApplyModifiedProperties();
        }

    }

}
