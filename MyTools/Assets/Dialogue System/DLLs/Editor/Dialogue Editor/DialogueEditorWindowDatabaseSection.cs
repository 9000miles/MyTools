using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.DialogueEditor
{

    /// <summary>
    /// This part of the Dialogue Editor window handles the Database tab.
    /// </summary>
    public partial class DialogueEditorWindow
    {

        [Serializable]
        private class DatabaseFoldouts
        {
            public bool database = true;
            public bool emphasisSettings = false;
            public bool[] emphasisSetting = new bool[DialogueDatabase.NumEmphasisSettings];
            public bool merge = false;
            public bool export = false;
            public bool editorSettings = false;
        }

        [SerializeField]
        private DatabaseFoldouts databaseFoldouts = new DatabaseFoldouts();
        [SerializeField]
        private DialogueDatabase databaseToMerge = null;
        [SerializeField]
        private DatabaseMerger.ConflictingIDRule conflictingIDRule = DatabaseMerger.ConflictingIDRule.ReplaceConflictingIDs;
        [SerializeField]
        private bool mergeProperties = true;
        [SerializeField]
        private bool mergeActors = true;
        [SerializeField]
        private bool mergeItems = true;
        [SerializeField]
        private bool mergeLocations = true;
        [SerializeField]
        private bool mergeVariables = true;
        [SerializeField]
        private bool mergeConversations = true;

        private enum ExportFormat { ChatMapperXML, CSV, VoiceoverScript, LanguageText };
        [SerializeField]
        private ExportFormat exportFormat = ExportFormat.ChatMapperXML;
        [SerializeField]
        private string chatMapperExportPath = string.Empty;
        [SerializeField]
        private string csvExportPath = string.Empty;
        [SerializeField]
        private string voiceoverExportPath = string.Empty;
        [SerializeField]
        private string languageTextExportPath = string.Empty;
        [SerializeField]
        private static bool exportActors = true;
        [SerializeField]
        private static bool exportItems = true;
        [SerializeField]
        private static bool exportLocations = true;
        [SerializeField]
        private static bool exportVariables = true;
        [SerializeField]
        private static bool exportConversations = true;
        [SerializeField]
        private static bool exportCanvasRect = true;
        [SerializeField]
        private static bool exportConversationsAfterEntries = false;
        [SerializeField]
        private EntrytagFormat entrytagFormat = EntrytagFormat.ActorName_ConversationID_EntryID;
        [SerializeField]
        private EncodingType encodingType = EncodingType.UTF8;

        private void DrawDatabaseSection()
        {
            EditorGUILayout.LabelField(database.name, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Database Properties", EditorStyles.boldLabel);
            EditorWindowTools.StartIndentedSection();
            EditorGUILayout.BeginVertical(GroupBoxStyle);
            database.author = EditorGUILayout.TextField("Author", database.author);
            database.version = EditorGUILayout.TextField(new GUIContent("Version", "By default, this is the version of the Chat Mapper data model, but you can use it for your own purposes"), database.version);
            EditorGUILayout.LabelField("Description");
            database.description = EditorGUILayout.TextArea(database.description);
            EditorGUILayout.LabelField(new GUIContent("Global User Script", "Optional Lua code to run when this database is loaded at runtime."));
            database.globalUserScript = EditorGUILayout.TextArea(database.globalUserScript);
            databaseFoldouts.emphasisSettings = EditorGUILayout.Foldout(databaseFoldouts.emphasisSettings, new GUIContent("Emphasis Settings", "Settings to use for [em#] tags in dialogue text."));
            if (databaseFoldouts.emphasisSettings) DrawEmphasisSettings();
            EditorGUILayout.EndVertical();
            EditorWindowTools.EndIndentedSection();
            databaseFoldouts.merge = EditorGUILayout.Foldout(databaseFoldouts.merge, new GUIContent("Merge Database", "Options to merge another database into this one."));
            if (databaseFoldouts.merge) DrawMergeSection();
            databaseFoldouts.export = EditorGUILayout.Foldout(databaseFoldouts.export, new GUIContent("Export Database", "Options to export the database to Chat Mapper XML format."));
            if (databaseFoldouts.export) DrawExportSection();
            databaseFoldouts.editorSettings = EditorGUILayout.Foldout(databaseFoldouts.editorSettings, new GUIContent("Editor Settings", "Editor settings."));
            if (databaseFoldouts.editorSettings) DrawEditorSettings();
        }

        private void DrawEditorSettings()
        {
            EditorWindowTools.StartIndentedSection();
            var newFreq = EditorGUILayout.FloatField(new GUIContent("Auto Backup Frequency", "Seconds between auto backups. Set to zero for no auto backups."), autoBackupFrequency);
            if (newFreq != autoBackupFrequency)
            {
                autoBackupFrequency = newFreq;
                timeForNextAutoBackup = Time.realtimeSinceStartup + autoBackupFrequency;
            }
            EditorGUILayout.BeginHorizontal();
            autoBackupFolder = EditorGUILayout.TextField(new GUIContent("Auto Backup Folder", "Location to write auto backups."), autoBackupFolder);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(22)))
            {
                var newAutoBackupFolder = EditorUtility.OpenFolderPanel("Auto Backup Folder", autoBackupFolder, string.Empty);
                if (!string.IsNullOrEmpty(newAutoBackupFolder)) autoBackupFolder = newAutoBackupFolder;
            }
            EditorGUILayout.EndHorizontal();
            showDatabaseName = EditorGUILayout.ToggleLeft(new GUIContent("Show Database Name", "Show the database name in the lower left of the editor window."), showDatabaseName);
            registerCompleteObjectUndo = EditorGUILayout.ToggleLeft(new GUIContent("Fast Undo for Large Databases", "Use Undo.RegisterCompleteObjectUndo instead of Undo.RegisterUndo. Tick if operations such as deleting a conversation become slow in very large databases."), registerCompleteObjectUndo);
            debug = EditorGUILayout.ToggleLeft(new GUIContent("Debug", "For internal debugging of the dialogue editor."), debug);
            EditorWindowTools.EndIndentedSection();
        }

        private void DrawEmphasisSettings()
        {
            EditorWindowTools.StartIndentedSection();
            for (int i = 0; i < DialogueDatabase.NumEmphasisSettings; i++)
            {
                databaseFoldouts.emphasisSetting[i] = EditorGUILayout.Foldout(databaseFoldouts.emphasisSetting[i], string.Format("Emphasis {0}", i + 1));
                if (databaseFoldouts.emphasisSetting[i]) DrawEmphasisSetting(database.emphasisSettings[i]);
            }
            EditorWindowTools.EndIndentedSection();
        }

        private void DrawEmphasisSetting(EmphasisSetting setting)
        {
            EditorWindowTools.StartIndentedSection();
            EditorGUILayout.BeginVertical(GroupBoxStyle);
            setting.color = EditorGUILayout.ColorField("Color", setting.color);
            setting.bold = EditorGUILayout.Toggle("Bold", setting.bold);
            setting.italic = EditorGUILayout.Toggle("Italic", setting.italic);
            setting.underline = EditorGUILayout.Toggle("Underline", setting.underline);
            EditorGUILayout.EndVertical();
            EditorWindowTools.EndIndentedSection();
        }

        private void DrawMergeSection()
        {
            EditorWindowTools.StartIndentedSection();
            EditorGUILayout.BeginVertical(GroupBoxStyle);
            EditorGUILayout.HelpBox("Use this feature to add the contents of another database to this database.", MessageType.None);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Database to Merge into " + database.name);
            databaseToMerge = EditorGUILayout.ObjectField(databaseToMerge, typeof(DialogueDatabase), false) as DialogueDatabase;
            EditorGUILayout.EndHorizontal();
            mergeProperties = EditorGUILayout.Toggle("Merge DB Properties", mergeProperties);
            mergeActors = EditorGUILayout.Toggle("Merge Actors", mergeActors);
            mergeItems = EditorGUILayout.Toggle("Merge Items", mergeItems);
            mergeLocations = EditorGUILayout.Toggle("Merge Locations", mergeLocations);
            mergeVariables = EditorGUILayout.Toggle("Merge Variables", mergeVariables);
            mergeConversations = EditorGUILayout.Toggle("Merge Conversations", mergeConversations);
            EditorGUILayout.BeginHorizontal();
            conflictingIDRule = (DatabaseMerger.ConflictingIDRule)EditorGUILayout.EnumPopup(new GUIContent("If IDs Conflict", "Replace Existing IDs: If the same ID exists in both databases, replace the original one with the new one.\nAllow Conflicting IDs: Append assets even if IDs conflict.\nAssign Unique IDs: Append and assign new IDs to assets from source database."), conflictingIDRule, GUILayout.Width(300));
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(databaseToMerge == null);
            if (GUILayout.Button("Merge...", GUILayout.Width(100)))
            {
                if (ConfirmMerge()) MergeDatabase();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorWindowTools.EndIndentedSection();
        }

        private bool ConfirmMerge()
        {
            return EditorUtility.DisplayDialog("Confirm Merge", GetMergeWarning(), "Merge", "Cancel");
        }

        private string GetMergeWarning()
        {
            switch (conflictingIDRule)
            {
                case DatabaseMerger.ConflictingIDRule.ReplaceConflictingIDs:
                    return string.Format("If assets with the same IDs exist in both databases, the versions in {0} will replace the versions in this database. Continue to merge?", databaseToMerge.name);
                case DatabaseMerger.ConflictingIDRule.AllowConflictingIDs:
                    return string.Format("If IDs in {0} already exist in this database, you'll get overlaps that may break conversations. Continue to merge?", databaseToMerge.name);
                case PixelCrushers.DialogueSystem.DatabaseMerger.ConflictingIDRule.AssignUniqueIDs:
                    return string.Format("IDs in {0} will be automatically renumbered with different IDs than existing existing IDs in this database. Continue to merge?", databaseToMerge.name);
                default:
                    return string.Format("Internal error. Merge type {0} is unsupported. Please inform support@pixelcrushers.com.", conflictingIDRule);
            }
        }

        private void MergeDatabase()
        {
            if (databaseToMerge != null)
            {
                DatabaseMerger.Merge(database, databaseToMerge, conflictingIDRule, mergeProperties, mergeActors, mergeItems, mergeLocations, mergeVariables, mergeConversations);
                Debug.Log(string.Format("{0}: Merged contents of {1} into {2}.", DialogueDebug.Prefix, databaseToMerge.name, database.name));
                databaseToMerge = null;
                SetDatabaseDirty("Merge Database");
            }
        }

        private void DrawExportSection()
        {
            EditorWindowTools.StartIndentedSection();
            EditorGUILayout.BeginVertical(GroupBoxStyle);
            EditorGUILayout.HelpBox("Use this feature to export your database to external text-based formats.\nIf exporting to Chat Mapper, you must also prepare a Chat Mapper template project that contains all the fields defined in this database. You can use the Dialogue System Chat Mapper template project as a base.", MessageType.None);
            if (exportFormat == ExportFormat.LanguageText)
            {
                EditorGUILayout.HelpBox("This will export a file for each language containing all the localized text for the language. You can use these text dumps to determine which characters your language-specific fonts need to support.", MessageType.Info);
                encodingType = (EncodingType)EditorGUILayout.EnumPopup("Encoding", encodingType);
            }
            else
            {
                exportActors = EditorGUILayout.Toggle("Export Actors", exportActors);
                exportItems = EditorGUILayout.Toggle("Export Items/Quests", exportItems);
                exportLocations = EditorGUILayout.Toggle("Export Locations", exportLocations);
                exportVariables = EditorGUILayout.Toggle("Export Variables", exportVariables);
                exportConversations = EditorGUILayout.Toggle("Export Conversations", exportConversations);
                if (exportFormat == ExportFormat.ChatMapperXML) exportCanvasRect = EditorGUILayout.Toggle(new GUIContent("Export Canvas Positions", "Export the positions of dialogue entry nodes in the Dialogue Editor's canvas"), exportCanvasRect);
                if (exportFormat == ExportFormat.CSV) exportConversationsAfterEntries = EditorGUILayout.Toggle(new GUIContent("Convs. After Entries", "Put the Conversations section after the DialogueEntries section in the CSV file. Normally the Conversations section is before."), exportConversationsAfterEntries);
                entrytagFormat = (EntrytagFormat)EditorGUILayout.EnumPopup("Entrytag Format", entrytagFormat, GUILayout.Width(400));
            }
            EditorGUILayout.BeginHorizontal();
            exportFormat = (ExportFormat)EditorGUILayout.EnumPopup("Format", exportFormat, GUILayout.Width(280));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Export...", GUILayout.Width(100)))
            {
                switch (exportFormat)
                {
                    case ExportFormat.ChatMapperXML:
                        TryExportToChatMapperXML();
                        break;
                    case ExportFormat.CSV:
                        TryExportToCSV();
                        break;
                    case ExportFormat.VoiceoverScript:
                        TryExportToVoiceoverScript();
                        break;
                    case ExportFormat.LanguageText:
                        TryExportToLanguageText();
                        break;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorWindowTools.EndIndentedSection();
        }

        private void TryExportToChatMapperXML()
        {
            string newChatMapperExportPath = EditorUtility.SaveFilePanel("Save Chat Mapper XML", EditorWindowTools.GetDirectoryName(chatMapperExportPath), chatMapperExportPath, "xml");
            if (!string.IsNullOrEmpty(newChatMapperExportPath))
            {
                chatMapperExportPath = newChatMapperExportPath;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    chatMapperExportPath = chatMapperExportPath.Replace("/", "\\");
                }
                if (exportCanvasRect) AddCanvasRectTemplateField();
                ValidateDatabase(database, false);
                ConfirmSyncAssetsAndTemplate();
                ChatMapperExporter.Export(database, chatMapperExportPath, exportActors, exportItems, exportLocations, exportVariables, exportConversations, exportCanvasRect);
                string templatePath = chatMapperExportPath.Replace(".xml", "_Template.txt");
                ExportTemplate(chatMapperExportPath, templatePath);
                EditorUtility.DisplayDialog("Export Complete", "The dialogue database was exported to Chat Mapper XML format.\n\n" +
                                            "Remember to apply a template after importing it into Chat Mapper. " +
                                            "The required fields in the template are listed in " + templatePath + ".\n\n" +
                                            "If you have any issues importing, please contact us at support@pixelcrushers.com.", "OK");
            }
        }

        private void AddCanvasRectTemplateField()
        {
            var field = template.dialogueEntryFields.Find(f => string.Equals(f.title, "canvasRect"));
            if (field == null)
            {
                template.dialogueEntryFields.Add(new Field("canvasRect", string.Empty, FieldType.Text));
            }
        }

        private void ExportTemplate(string chatMapperExportPath, string templatePath)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(templatePath))
                {
                    file.WriteLine("Required Chat Mapper Template Fields for: " + chatMapperExportPath);
                    file.WriteLine("\nACTORS (in this order):");
                    ExportTemplateFields(file, template.actorFields);
                    file.WriteLine("\nITEMS (in this order):");
                    ExportTemplateFields(file, template.itemFields);
                    file.WriteLine("\nLOCATIONS (in this order):");
                    ExportTemplateFields(file, template.locationFields);
                    file.WriteLine("\nUSER VARIABLES (in this order):");
                    ExportTemplateFields(file, template.variableFields);
                    file.WriteLine("\nCONVERSATIONS (in this order):");
                    ExportTemplateFields(file, template.conversationFields);
                    file.WriteLine("\nDIALOGUE NODES (in this order):");
                    ExportTemplateFields(file, template.dialogueEntryFields);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("{0}: Error writing Chat Mapper template file {1}: {2}", DialogueDebug.Prefix, templatePath, e.Message));
            }
        }

        private void ExportTemplateFields(System.IO.StreamWriter file, List<Field> fields)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                Field field = fields[i];
                file.WriteLine(string.Format("[{0}] {1}: {2}", i + 1, field.title, field.type.ToString()));
            }
        }

        private void TryExportToCSV()
        {
            string newCSVExportPath = EditorUtility.SaveFilePanel("Save CSV", EditorWindowTools.GetDirectoryName(csvExportPath), csvExportPath, "csv");
            if (!string.IsNullOrEmpty(newCSVExportPath))
            {
                csvExportPath = newCSVExportPath;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    csvExportPath = csvExportPath.Replace("/", "\\");
                }
                CSVExporter.Export(database, csvExportPath, exportActors, exportItems, exportLocations, exportVariables, exportConversations, exportConversationsAfterEntries, entrytagFormat);
                EditorUtility.DisplayDialog("Export Complete", "The dialogue database was exported to CSV (comma-separated values) format. ", "OK");
            }
        }

        private void TryExportToVoiceoverScript()
        {
            string newVoiceoverPath = EditorUtility.SaveFilePanel("Save Voiceover Script", EditorWindowTools.GetDirectoryName(voiceoverExportPath), voiceoverExportPath, "csv");
            if (!string.IsNullOrEmpty(newVoiceoverPath))
            {
                voiceoverExportPath = newVoiceoverPath;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    voiceoverExportPath = voiceoverExportPath.Replace("/", "\\");
                }
                VoiceoverScriptExporter.Export(database, voiceoverExportPath, exportActors, entrytagFormat);
                EditorUtility.DisplayDialog("Export Complete", "The voiceover script was exported to CSV (comma-separated values) format. ", "OK");
            }
        }

        private void TryExportToLanguageText()
        {
            string newLanguageTextPath = EditorUtility.SaveFilePanel("Save Language Text", EditorWindowTools.GetDirectoryName(languageTextExportPath), languageTextExportPath, "txt");
            if (!string.IsNullOrEmpty(newLanguageTextPath))
            {
                languageTextExportPath = newLanguageTextPath;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    languageTextExportPath = languageTextExportPath.Replace("/", "\\");
                }
                LanguageTextExporter.Export(database, languageTextExportPath, encodingType);
                EditorUtility.DisplayDialog("Export Complete", "The language texts have been exported to " + languageTextExportPath + " with the language code appended to the end of each filename. ", "OK");
            }
        }

        private void DrawNoDatabaseSection()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select a dialogue database.");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create New", GUILayout.Width(120))) CreateNewDatabase();
            EditorGUILayout.EndHorizontal();
        }

        private void CreateNewDatabase()
        {
            DialogueSystemMenuItems.CreateDialogueDatabase();
        }

        // This method will probably move to a more appropriate location in a future version:
        private void ValidateDatabase(DialogueDatabase database, bool debug)
        {
            if (database == null || database.actors == null || database.conversations == null) return;
            if (database.actors.Count < 1) database.actors.Add(template.CreateActor(1, "Player", true));

            // Keeps track of nodes that are already connected (link's IsConnector=false):
            HashSet<string> alreadyConnected = new HashSet<string>();

            // Check each conversation:
            foreach (Conversation conversation in database.conversations)
            {

                // Check item and location fields:
                foreach (Field field in conversation.fields)
                {
                    if (field.type == FieldType.Location)
                    {
                        if (database.GetLocation(Tools.StringToInt(field.value)) == null)
                        {
                            if (debug) Debug.Log(string.Format("Fixing location field '{0}' for conversation {1}", field.title, conversation.id));
                            if (database.locations.Count < 1) database.locations.Add(template.CreateLocation(1, "Nowhere"));
                            field.value = database.locations[0].id.ToString();
                        }
                    }
                    else if (field.type == FieldType.Item)
                    {
                        if (database.GetItem(Tools.StringToInt(field.value)) == null)
                        {
                            if (debug) Debug.Log(string.Format("Fixing item field '{0}' for conversation {1}", field.title, conversation.id));
                            if (database.items.Count < 1) database.items.Add(template.CreateItem(1, "No Item"));
                            field.value = database.items[0].id.ToString();
                        }
                    }
                }

                // Get valid actor IDs for conversation:
                int conversationActorID = (database.GetActor(conversation.ActorID) != null) ? conversation.ActorID : database.actors[0].id;
                if (conversationActorID != conversation.ActorID)
                {
                    if (debug) Debug.Log(string.Format("Fixing actor ID for conversation {0}", conversation.id));
                    conversation.ActorID = conversationActorID;
                }
                int conversationConversantID = (database.GetActor(conversation.ConversantID) != null) ? conversation.ConversantID : database.actors[0].id;
                if (conversationConversantID != conversation.ConversantID)
                {
                    if (debug) Debug.Log(string.Format("Fixing conversant ID for conversation {0}", conversation.id));
                    conversation.ConversantID = conversationConversantID;
                }

                // Check all dialogue entries:
                foreach (DialogueEntry entry in conversation.dialogueEntries)
                {

                    // Make sure actor IDs are valid:
                    if (database.GetActor(entry.ActorID) == null)
                    {
                        if (debug) Debug.Log(string.Format("Fixing actor ID for conversation {0}, entry {1}: actor ID {2}-->{3}", conversation.id, entry.id, entry.ActorID, ((entry.ConversantID == conversationConversantID) ? conversationActorID : conversationConversantID)));
                        entry.ActorID = (entry.ConversantID == conversationConversantID) ? conversationActorID : conversationConversantID;
                    }
                    if (database.GetActor(entry.ConversantID) == null)
                    {
                        if (debug) Debug.Log(string.Format("Fixing conversant ID for conversation {0}, entry {1}: conversant ID {2}-->{3}", conversation.id, entry.id, entry.ConversantID, ((entry.ActorID == conversationConversantID) ? conversationActorID : conversationConversantID)));
                        entry.ConversantID = (entry.ActorID == conversationConversantID) ? conversationActorID : conversationConversantID;
                    }

                    // Make sure all outgoing links' origins point to this conversation and entry, and set as connector:
                    foreach (Link link in entry.outgoingLinks)
                    {
                        if (link.originConversationID != conversation.id)
                        {
                            if (debug) Debug.Log(string.Format("Fixing link.originConversationID convID={0}, entryID={1}", conversation.id, entry.id));
                            link.originConversationID = conversation.id;
                        }
                        if (link.originDialogueID != entry.id)
                        {
                            if (debug) Debug.Log(string.Format("Fixing link.originDialogueID convID={0}, entryID={1}", conversation.id, entry.id));
                            link.originDialogueID = entry.id;
                        }
                        link.isConnector = true;
                    }
                }

                // Traverse tree, assigning non-connector to the first occurrence in the tree:
                AssignConnectors(conversation.GetFirstDialogueEntry(), conversation, alreadyConnected, new HashSet<int>(), 0);
            }
        }

        private void AssignConnectors(DialogueEntry entry, Conversation conversation, HashSet<string> alreadyConnected, HashSet<int> entriesVisited, int level)
        {
            // Sanity check to prevent infinite recursion:
            if (level > 10000) return;

            // Set non-connectors:
            foreach (Link link in entry.outgoingLinks)
            {
                if (link.originConversationID == link.destinationConversationID)
                {
                    string destination = string.Format("{0}.{1}", link.destinationConversationID, link.destinationDialogueID);
                    if (alreadyConnected.Contains(destination))
                    {
                        link.isConnector = true;
                    }
                    else
                    {
                        link.isConnector = false;
                        alreadyConnected.Add(destination);
                    }
                }
            }

            // Then process each child:
            foreach (Link link in entry.outgoingLinks)
            {
                if (link.originConversationID == link.destinationConversationID)
                {
                    if (!entriesVisited.Contains(link.destinationDialogueID))
                    {
                        entriesVisited.Add(link.destinationDialogueID);
                        var childEntry = conversation.GetDialogueEntry(link.destinationDialogueID);
                        if (childEntry != null)
                        {
                            AssignConnectors(childEntry, conversation, alreadyConnected, entriesVisited, level + 1);
                        }
                    }
                }
            }
        }

    }

}