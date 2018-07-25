using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(BarkOnIdle))]
    public class BarkOnIdleEditor : Editor
    {

        //--- We use [ConversationPopup] now:
        //private ConversationPicker conversationPicker = null;

        //public void OnEnable()
        //{
        //    var trigger = target as BarkOnIdle;
        //    if (trigger != null)
        //    {
        //        conversationPicker = new ConversationPicker(trigger.selectedDatabase, trigger.conversation, trigger.useConversationTitlePicker);
        //    }
        //}

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var trigger = target as BarkOnIdle;
            if (trigger == null) return;

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

            EditorGUILayout.PropertyField(serializedObject.FindProperty("barkOrder"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("conversant"), new GUIContent("Barker", "The actor speaking the bark. If unassigned, this GameObject."), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("target"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minSeconds"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSeconds"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("once"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("skipIfNoValidEntries"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowDuringConversations"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cacheBarkLines"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("condition"), true);
            serializedObject.ApplyModifiedProperties();
        }

    }

}
