using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(DialogueSystemTrigger))]
    public class DialogueSystemTriggerEditor : Editor
    {

        private static bool actionFoldout = false;
        private static bool conversationFoldout = false;
        private static bool barkFoldout = false;
        private static bool sequenceFoldout = false;
        private static bool questFoldout = false;
        private static bool luaFoldout = false;
        private static bool alertFoldout = false;
        //private ConversationPicker conversationPicker = null;
        //private ConversationPicker barkPicker = null;
        private QuestPicker questPicker = null;
        private LuaScriptWizard luaScriptWizard = null;
        private Rect sequenceRect;

        public void OnEnable()
        {
            var trigger = target as DialogueSystemTrigger;
            if (trigger != null)
            {
                if (trigger.selectedDatabase == null)
                {
                    if (EditorTools.selectedDatabase == null) EditorTools.selectedDatabase = EditorTools.FindInitialDatabase();
                    trigger.selectedDatabase = EditorTools.selectedDatabase;
                }
                else
                {
                    EditorTools.selectedDatabase = trigger.selectedDatabase;
                }
                luaScriptWizard = new LuaScriptWizard(trigger.selectedDatabase);
                questPicker = new QuestPicker(trigger.selectedDatabase, trigger.questName, trigger.useQuestNamePicker);
                questPicker.showReferenceDatabase = false;
                //InitConversationPickers();
                //EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
                //EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
            }
        }

        //public void OnDisable()
        //{
        //    EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
        //}

        //private void OnPlaymodeStateChanged()
        //{
        //    InitConversationPickers();
        //}

        //private void InitConversationPickers()
        //{
        //    var trigger = target as DialogueSystemTrigger;
        //    if (trigger != null)
        //    {
        //        conversationPicker = new ConversationPicker(trigger.selectedDatabase, trigger.conversation, trigger.useConversationTitlePicker);
        //        barkPicker = new ConversationPicker(trigger.selectedDatabase, trigger.barkConversation, trigger.useBarkTitlePicker);
        //    }
        //}

        public override void OnInspectorGUI()
        {
            var trigger = target as DialogueSystemTrigger;
            if (trigger == null) return;

            serializedObject.Update();

            // Trigger event:
            var triggerProperty = serializedObject.FindProperty("trigger");
            EditorGUILayout.PropertyField(triggerProperty, true);

            // Reference database:
            var databaseProperty = serializedObject.FindProperty("selectedDatabase");
            var oldDatabase = databaseProperty.objectReferenceValue;
            EditorGUILayout.PropertyField(databaseProperty, new GUIContent("Reference Database"), true);
            var newDatabase = databaseProperty.objectReferenceValue as DialogueDatabase;
            if (newDatabase != oldDatabase)
            {
                luaScriptWizard = new LuaScriptWizard(newDatabase);
                questPicker = new QuestPicker(newDatabase, trigger.questName, trigger.useQuestNamePicker);
                questPicker.showReferenceDatabase = false;
                //InitConversationPickers();
            }

            // Condition:
            if (newDatabase != null) EditorTools.selectedDatabase = newDatabase;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("condition"), true);

            // Action:
            actionFoldout = EditorGUILayout.Foldout(actionFoldout, "Action");
            if (actionFoldout)
            {
                EditorWindowTools.StartIndentedSection();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("once"), true);

                // Quest:
                questFoldout = EditorGUILayout.Foldout(questFoldout, "Set Quest State");
                if (questFoldout)
                {
                    EditorWindowTools.StartIndentedSection();

                    // Quest picker:
                    if (questPicker != null)
                    {
                        serializedObject.ApplyModifiedProperties();
                        questPicker.Draw();
                        trigger.questName = questPicker.currentQuest;
                        trigger.useQuestNamePicker = questPicker.usePicker;
                        trigger.selectedDatabase = questPicker.database;
                        if (EditorTools.selectedDatabase == null) EditorTools.selectedDatabase = trigger.selectedDatabase;
                        serializedObject.Update();
                    }

                    // Quest state:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("setQuestState"), true);
                    if (serializedObject.FindProperty("setQuestState").boolValue)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("questState"), true);
                    }

                    // Quest entry state:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("setQuestEntryState"), true);
                    if (serializedObject.FindProperty("setQuestEntryState").boolValue)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("questEntryNumber"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("questEntryState"), true);
                    }

                    EditorWindowTools.EndIndentedSection();
                }

                // Lua code / wizard:
                luaFoldout = EditorGUILayout.Foldout(luaFoldout, "Run Lua Code");
                if (luaFoldout)
                {
                    EditorWindowTools.StartIndentedSection();
                    if (EditorTools.selectedDatabase != luaScriptWizard.database)
                    {
                        luaScriptWizard.database = EditorTools.selectedDatabase;
                        luaScriptWizard.RefreshWizardResources();
                    }
                    serializedObject.ApplyModifiedProperties();
                    trigger.luaCode = luaScriptWizard.Draw(new GUIContent("Lua Code", "The Lua code to run when the condition is true"), trigger.luaCode);
                    serializedObject.Update();
                    EditorWindowTools.EndIndentedSection();
                }

                // Sequence:
                sequenceFoldout = EditorGUILayout.Foldout(sequenceFoldout, "Play Sequence");
                if (sequenceFoldout)
                {
                    EditorWindowTools.StartIndentedSection();

                    if (DialogueTriggerEventDrawer.IsEnableOrStartEnumIndex(triggerProperty.enumValueIndex))
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("waitOneFrameOnStartOrEnable"), new GUIContent("Wait 1 Frame", "Tick to wait one frame to allow other components to finish their OnStart/OnEnable"), true);

                    }
                    //EditorGUILayout.PropertyField(serializedObject.FindProperty("sequence"), true);
                    serializedObject.ApplyModifiedProperties();
                    trigger.sequence = SequenceEditorTools.DrawLayout(new GUIContent("Sequence"), trigger.sequence, ref sequenceRect);
                    serializedObject.Update();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("sequenceSpeaker"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("sequenceListener"), true);

                    EditorWindowTools.EndIndentedSection();
                }

                // Alert:
                alertFoldout = EditorGUILayout.Foldout(alertFoldout, "Show Alert");
                if (alertFoldout)
                {
                    EditorWindowTools.StartIndentedSection();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("alertMessage"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("localizedTextTable"), true);
                    var alertDurationProperty = serializedObject.FindProperty("alertDuration");
                    bool specifyAlertDuration = !Mathf.Approximately(0, alertDurationProperty.floatValue);
                    specifyAlertDuration = EditorGUILayout.Toggle(new GUIContent("Specify Duration", "Tick to specify an alert duration; untick to use the default"), specifyAlertDuration);
                    if (specifyAlertDuration)
                    {
                        if (Mathf.Approximately(0, alertDurationProperty.floatValue)) alertDurationProperty.floatValue = 5;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("alertDuration"), true);
                    }
                    else
                    {
                        alertDurationProperty.floatValue = 0;
                    }
                    EditorWindowTools.EndIndentedSection();
                }

                // Send Messages list:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sendMessages"), true);

                // Bark:
                barkFoldout = EditorGUILayout.Foldout(barkFoldout, "Bark");
                if (barkFoldout)
                {
                    EditorWindowTools.StartIndentedSection();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("barkConversation"), true);
                    //if (barkPicker != null)
                    //{
                    //    serializedObject.ApplyModifiedProperties();
                    //    if (barkPicker.Draw(false))
                    //    {
                    //        trigger.barkConversation = barkPicker.currentConversation;
                    //    }
                    //    trigger.useBarkTitlePicker = barkPicker.usePicker;
                    //    trigger.selectedDatabase = barkPicker.database;
                    //    if (EditorTools.selectedDatabase == null) EditorTools.selectedDatabase = trigger.selectedDatabase;
                    //    serializedObject.ApplyModifiedProperties();
                    //}
                    //else
                    //{
                    //    EditorGUILayout.PropertyField(serializedObject.FindProperty("barkConversation"), true);
                    //}
                    if (!string.IsNullOrEmpty(serializedObject.FindProperty("barkConversation").stringValue))
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("barkOrder"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("barker"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("barkTarget"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("skipBarkIfNoValidEntries"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("allowBarksDuringConversations"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("cacheBarkLines"), true);
                    }

                    EditorWindowTools.EndIndentedSection();
                }

                // Conversation:
                conversationFoldout = EditorGUILayout.Foldout(conversationFoldout, "Start Conversation");
                if (conversationFoldout)
                {
                    EditorWindowTools.StartIndentedSection();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("conversation"), true);

                    //if (conversationPicker != null)
                    //{
                    //    serializedObject.ApplyModifiedProperties();
                    //    if (conversationPicker.Draw(false))
                    //    {
                    //        trigger.conversation = conversationPicker.currentConversation;
                    //    }
                    //    trigger.useConversationTitlePicker = conversationPicker.usePicker;
                    //    trigger.selectedDatabase = conversationPicker.database;
                    //    serializedObject.Update();
                    //}
                    //else
                    //{
                    //    EditorGUILayout.PropertyField(serializedObject.FindProperty("conversation"), true);
                    //}
                    if (!string.IsNullOrEmpty(serializedObject.FindProperty("conversation").stringValue))
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("conversationActor"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("conversationConversant"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("exclusive"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("skipIfNoValidEntries"), true);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("stopConversationOnTriggerExit"), true);
                    }

                    EditorWindowTools.EndIndentedSection();
                }

                EditorWindowTools.EndIndentedSection();
            }

            serializedObject.ApplyModifiedProperties();
        }

    }

}
