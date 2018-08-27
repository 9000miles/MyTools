using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MarsPC;
using Common;

namespace PixelCrushers.DialogueSystem.DialogueEditor
{
    /// <summary>
    /// This part of the Dialogue Editor window handles the outline-style dialogue tree editor.
    /// </summary>
    public partial class DialogueEditorWindow
    {
        private readonly string[] falseConditionActionStrings = { "Block", "Passthrough" };
        private readonly string[] priorityStrings = { "Low", "Below Normal", "Normal", "Above Normal", "High" };
        private const float DialogueEntryIndent = 16;
        private const int MaxNodeTextLength = 26;

        private class DialogueNode
        {
            public DialogueEntry entry;
            public Link originLink;
            public GUIStyle guiStyle;
            public float indent;
            public bool isEditable;
            public bool hasFoldout;
            public List<DialogueNode> children;

            public DialogueNode(DialogueEntry entry, Link originLink, GUIStyle guiStyle, float indent, bool isEditable, bool hasFoldout)
            {
                this.entry = entry;
                this.originLink = originLink;
                this.guiStyle = guiStyle;
                this.indent = indent;
                this.isEditable = isEditable;
                this.hasFoldout = hasFoldout;
                this.children = new List<DialogueNode>();
            }
        }

        private bool dialogueTreeFoldout = false;
        private bool orphansFoldout = false;
        private Dictionary<int, string> dialogueEntryText = new Dictionary<int, string>();
        private Dictionary<int, string> dialogueEntryNodeText = new Dictionary<int, string>();
        private Dictionary<int, bool> dialogueEntryFoldouts = new Dictionary<int, bool>();
        private DialogueNode dialogueTree = null;
        private List<DialogueNode> orphans = new List<DialogueNode>();
        private Field currentEntryActor = null;
        private Field currentEntryConversant = null;
        private bool entryEventFoldout = false;
        private bool entryFieldsFoldout = false;

        private DialogueEntry _currentEntry = null;
        [SerializeField]
        private int currentEntryID = -1;
        private DialogueEntry currentEntry
        {
            get
            {
                return _currentEntry;
            }
            set
            {
                _currentEntry = value;
                if (value != null)
                {
                    currentEntryID = value.id;
                    if (value.fields != null) BuildLanguageListFromFields(value.fields);
                }
                if (verboseDebug && value != null) Debug.Log("<color=magenta>Set current entry ID to " + currentEntryID + "</color>");
            }
        }

        private LuaConditionWizard luaConditionWizard = new LuaConditionWizard(null);
        private LuaScriptWizard luaScriptWizard = new LuaScriptWizard(null);

        private void SetCurrentEntryByID()
        {
            if (verboseDebug) Debug.Log("<color=magenta>Set current entry to ID " + currentEntryID + "</color>");
            var entry = (currentConversation != null) ? currentConversation.GetDialogueEntry(currentEntryID) : null;
            SetCurrentEntry(entry);
            ResetNodeEditorConversationList();
            dialogueTreeFoldout = true;
            InitializeDialogueTree();
        }

        private void ResetCurrentEntryID()
        {
            if (verboseDebug) Debug.Log("<color=magenta>Reset current entry ID</color>");
            currentEntryID = -1;
        }

        private void ResetDialogueTreeSection()
        {
            dialogueTreeFoldout = false;
            orphansFoldout = false;
            ResetDialogueEntryText();
            dialogueEntryFoldouts.Clear();
            ResetDialogueTree();
            currentEntry = null;
            ResetLuaWizards();
        }

        private void ResetDialogueEntryText()
        {
            dialogueEntryText.Clear();
            dialogueEntryNodeText.Clear();
        }

        public void ResetDialogueEntryText(DialogueEntry entry)
        {
            if (entry == null) return;
            if (dialogueEntryText.ContainsKey(entry.id)) dialogueEntryText[entry.id] = null;
            if (dialogueEntryNodeText.ContainsKey(entry.id)) dialogueEntryNodeText[entry.id] = null;
        }

        private void ResetDialogueTreeCurrentEntryParticipants()
        {
            currentEntryActor = null;
            currentEntryConversant = null;
        }

        private void ResetCurrentEntry()
        {
            currentEntry = null;
            ResetConditionsWizard();
            ResetScriptWizard();
            ResetDialogueTreeCurrentEntryParticipants();
            ResetUnityEventSection();
        }

        public void ResetLuaWizards()
        {
            CheckLuaWizards();
            ResetConditionsWizard();
            ResetScriptWizard();
        }

        private void ResetConditionsWizard()
        {
            luaConditionWizard.ResetWizard();
        }

        private void ResetScriptWizard()
        {
            luaScriptWizard.ResetWizard();
        }

        private void CheckLuaWizards()
        {
            if (currentEntry == null) return;
            if (luaConditionWizard.IsOpen) currentEntry.conditionsString = luaConditionWizard.AcceptConditionsWizard(); //[WIP]
            if (luaScriptWizard.IsOpen) currentEntry.userScript = luaScriptWizard.AcceptScriptWizard();
        }

        private void DrawDialogueTreeFoldout()
        {
            CheckDialogueTreeGUIStyles();
            if (AreConversationParticipantsValid())
            {
                bool isDialogueTreeOpen = EditorGUILayout.Foldout(dialogueTreeFoldout, "Dialogue Tree");
                if (isDialogueTreeOpen && !dialogueTreeFoldout) InitializeDialogueTree();
                dialogueTreeFoldout = isDialogueTreeOpen;
                if (dialogueTreeFoldout) DrawDialogueTree();
            }
            else
            {
                EditorGUILayout.LabelField("Dialogue Tree: Assign Actor and Conversant first.");
            }
        }

        private void InitializeDialogueTree()
        {
            ValidateStartEntryID();
            ResetDialogueTree();
            BuildDialogueTree();
            ResetOrphanIDs();
        }

        private void ValidateStartEntryID()
        {
            if (startEntry == null) startEntry = (currentConversation != null) ? currentConversation.GetFirstDialogueEntry() : null;
            if (startEntry != null)
            {
                if (startEntry.conversationID != currentConversation.id)
                {
                    startEntry.conversationID = currentConversation.id;
                    SetDatabaseDirty("Check/Set START entry conversation ID");
                }
            }
        }

        private void ResetDialogueTree()
        {
            dialogueTree = null;
            orphans.Clear();
            ResetLanguageList();
        }

        private void BuildDialogueTree()
        {
            if (currentConversation == null) return;
            List<DialogueEntry> visited = new List<DialogueEntry>();
            dialogueTree = BuildDialogueNode(startEntry, null, 0, visited);
            RecordOrphans(visited);
            BuildLanguageListFromConversation();
        }

        private void BuildLanguageListFromConversation()
        {
            for (int i = 0; i < currentConversation.dialogueEntries.Count; i++)
            {
                var entry = currentConversation.dialogueEntries[i];
                BuildLanguageListFromFields(entry.fields);
            }
        }

        private DialogueNode BuildDialogueNode(DialogueEntry entry, Link originLink, int level, List<DialogueEntry> visited)
        {
            if (entry == null) return null;
            bool wasEntryAlreadyVisited = visited.Contains(entry);
            if (!wasEntryAlreadyVisited) visited.Add(entry);

            // Create this node:
            float indent = DialogueEntryIndent * level;
            bool isLeaf = (entry.outgoingLinks.Count == 0);
            bool hasFoldout = !(isLeaf || wasEntryAlreadyVisited);
            GUIStyle guiStyle = wasEntryAlreadyVisited ? grayGUIStyle
                : isLeaf ? GetDialogueEntryLeafStyle(entry) : GetDialogueEntryStyle(entry);
            DialogueNode node = new DialogueNode(entry, originLink, guiStyle, indent, !wasEntryAlreadyVisited, hasFoldout);
            if (!dialogueEntryFoldouts.ContainsKey(entry.id)) dialogueEntryFoldouts[entry.id] = true;

            // Add children:
            if (!wasEntryAlreadyVisited)
            {
                for (int i = 0; i < entry.outgoingLinks.Count; i++)
                {
                    var link = entry.outgoingLinks[i];
                    if (link.destinationConversationID == currentConversation.id) // Only show connection if within same conversation.
                    {
                        node.children.Add(BuildDialogueNode(currentConversation.GetDialogueEntry(link.destinationDialogueID), link, level + 1, visited));
                    }
                }
            }
            return node;
        }

        private void RecordOrphans(List<DialogueEntry> visited)
        {
            if (visited.Count < currentConversation.dialogueEntries.Count)
            {
                for (int i = 0; i < currentConversation.dialogueEntries.Count; i++)
                {
                    var entry = currentConversation.dialogueEntries[i];
                    if (!visited.Contains(entry))
                    {
                        orphans.Add(new DialogueNode(entry, null, GetDialogueEntryStyle(entry), 0, false, false));
                    }
                }
            }
        }

        private GUIStyle GetDialogueEntryStyle(DialogueEntry entry)
        {
            return ((entry != null) && database.IsPlayerID(entry.ActorID)) ? pcLineGUIStyle : npcLineGUIStyle;
        }

        private GUIStyle GetDialogueEntryLeafStyle(DialogueEntry entry)
        {
            return ((entry != null) && database.IsPlayerID(entry.ActorID)) ? pcLineLeafGUIStyle : npcLineLeafGUIStyle;
        }

        private GUIStyle GetLinkButtonStyle(DialogueEntry entry)
        {
            return ((database != null) && (entry != null) && database.IsPlayerID(entry.ActorID)) ? pcLinkButtonGUIStyle : npcLinkButtonGUIStyle;
        }

        private string GetDialogueEntryText(DialogueEntry entry)
        {
            if (entry == null) return string.Empty;
            if (!dialogueEntryText.ContainsKey(entry.id) || (dialogueEntryText[entry.id] == null))
            {
                dialogueEntryText[entry.id] = BuildDialogueEntryText(entry);
            }
            return dialogueEntryText[entry.id];
        }

        private string BuildDialogueEntryText(DialogueEntry entry)
        {
            string text = entry.MenuText;
            if (string.IsNullOrEmpty(text)) text = entry.DialogueText;
            if (string.IsNullOrEmpty(text)) text = "<" + entry.Title + ">";
            if (entry.isGroup) text = "{group} " + text;
            if (text.Contains("\n")) text = text.Replace("\n", string.Empty);
            string speaker = GetActorNameByID(entry.ActorID);
            text = string.Format("[{0}] {1}: {2}", entry.id, speaker, text);
            if (!string.IsNullOrEmpty(entry.conditionsString)) text += " [condition]";
            if (!string.IsNullOrEmpty(entry.userScript)) text += " {script}";
            if ((entry.outgoingLinks == null) || (entry.outgoingLinks.Count == 0)) text += " [END]";
            return text;
        }

        private string GetDialogueEntryNodeText(DialogueEntry entry)
        {
            if (!dialogueEntryNodeText.ContainsKey(entry.id) || (dialogueEntryNodeText[entry.id] == null))
            {
                dialogueEntryNodeText[entry.id] = BuildDialogueEntryNodeText(entry);
            }
            return dialogueEntryNodeText[entry.id];
        }

        private string BuildDialogueEntryNodeText(DialogueEntry entry)
        {
            string text = entry.MenuText;
            if (string.IsNullOrEmpty(text)) text = entry.DialogueText;
            if (string.IsNullOrEmpty(text)) text = "<" + entry.Title + ">";
            if (entry.isGroup) text = "{group} " + text;
            if (text.Contains("\n")) text = text.Replace("\n", string.Empty);
            int extraLength = 0;
            if (showAllActorNames)
            {
                string actorName = GetActorNameByID(entry.ActorID);
                if (actorName != null) extraLength = actorName.Length;
                text = string.Format("{0}:\n{1}", actorName, text);
            }
            else if (showOtherActorNames && entry.ActorID != currentConversation.ActorID && (entry.ActorID != currentConversation.ConversantID))
            {
                text = string.Format("{0}: {1}", GetActorNameByID(entry.ActorID), text);
            }
            if (showNodeIDs)
            {
                text = "[" + entry.id + "] " + text;
            }
            if (!string.IsNullOrEmpty(entry.conditionsString)) text += " [condition]";
            if (!string.IsNullOrEmpty(entry.userScript)) text += " {script}";
            if ((entry.outgoingLinks == null) || (entry.outgoingLinks.Count == 0)) text += " [END]";
            return text.Substring(0, Mathf.Min(text.Length, MaxNodeTextLength + extraLength));
        }

        private void DrawDialogueTree()
        {
            // Setup:
            Link linkToDelete = null;
            DialogueEntry entryToLinkFrom = null;
            DialogueEntry entryToLinkTo = null;
            bool linkToAnotherConversation = false;

            // Draw the tree:
            EditorWindowTools.StartIndentedSection();
            DrawDialogueNode(dialogueTree, null, ref linkToDelete, ref entryToLinkFrom, ref entryToLinkTo, ref linkToAnotherConversation);
            EditorWindowTools.EndIndentedSection();

            // Handle deletion:
            if (linkToDelete != null)
            {
                DeleteLink(linkToDelete);
                InitializeDialogueTree();
            }

            // Handle linking:
            if (entryToLinkFrom != null)
            {
                if (entryToLinkTo == null)
                {
                    if (linkToAnotherConversation)
                    {
                        LinkToAnotherConversation(entryToLinkFrom);
                    }
                    else
                    {
                        LinkToNewEntry(entryToLinkFrom);
                    }
                }
                else
                {
                    CreateLink(entryToLinkFrom, entryToLinkTo);
                }
                InitializeDialogueTree();
            }

            // Draw orphans:
            DrawOrphansFoldout();
        }

        private const float FoldoutRectWidth = 10;

        private void DrawDialogueNode(
            DialogueNode node,
            Link originLink,
            ref Link linkToDelete,
            ref DialogueEntry entryToLinkFrom,
            ref DialogueEntry entryToLinkTo,
            ref bool linkToAnotherConversation)
        {
            if (node == null) return;

            // Setup:
            bool deleted = false;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(string.Empty, GUILayout.Width(node.indent));
            if (node.isEditable)
            {
                // Draw foldout if applicable:
                if (node.hasFoldout && node.entry != null)
                {
                    Rect rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(FoldoutRectWidth));
                    dialogueEntryFoldouts[node.entry.id] = EditorGUI.Foldout(rect, dialogueEntryFoldouts[node.entry.id], string.Empty);
                }

                // Draw label/button to edit:
                if (GUILayout.Button(GetDialogueEntryText(node.entry), node.guiStyle))
                {
                    GUIUtility.keyboardControl = 0;
                    currentEntry = (currentEntry != node.entry) ? node.entry : null;
                    ResetLuaWizards();
                    ResetDialogueTreeCurrentEntryParticipants();
                }

                // Draw delete-node button:
                GUI.enabled = (originLink != null);
                deleted = GUILayout.Button(new GUIContent(" ", "Delete entry."), "OL Minus", GUILayout.Width(16));
                if (deleted) linkToDelete = originLink;
                GUI.enabled = true;
            }
            else
            {
                // Draw uneditable node:
                EditorGUILayout.LabelField(GetDialogueEntryText(node.entry), node.guiStyle);
                GUI.enabled = false;
                GUILayout.Button(" ", "OL Minus", GUILayout.Width(16));
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();

            // Draw contents if this is the currently-selected entry:
            if (!deleted && (node.entry == currentEntry) && node.isEditable)
            {
                DrawDialogueEntryContents(currentEntry, ref linkToDelete, ref entryToLinkFrom, ref entryToLinkTo, ref linkToAnotherConversation);
            }

            // Draw children:
            if (!deleted && node.hasFoldout && (node.entry != null) && dialogueEntryFoldouts[node.entry.id])
            {
                for (int i = 0; i < node.children.Count; i++)
                {
                    var child = node.children[i];
                    if (child != null)
                    {
                        DrawDialogueNode(child, child.originLink, ref linkToDelete, ref entryToLinkFrom, ref entryToLinkTo, ref linkToAnotherConversation);
                    }
                }
            }
        }

        private void DrawOrphansFoldout()
        {
            if (orphans.Count > 0)
            {
                orphansFoldout = EditorGUILayout.Foldout(orphansFoldout, "Orphan Entries");
                if (orphansFoldout)
                {
                    DialogueEntry entryToDelete = null;
                    DrawOrphans(ref entryToDelete);
                    if (entryToDelete != null)
                    {
                        currentConversation.dialogueEntries.Remove(entryToDelete);
                        InitializeDialogueTree();
                        SetDatabaseDirty("Delete Dialogue Entry");
                    }
                }
            }
        }

        private void DrawOrphans(ref DialogueEntry entryToDelete)
        {
            EditorWindowTools.StartIndentedSection();
            entryToDelete = null;
            for (int i = 0; i < orphans.Count; i++)
            {
                var node = orphans[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(GetDialogueEntryText(node.entry), node.guiStyle);
                bool deleted = GUILayout.Button(new GUIContent(" ", "Delete entry."), "OL Minus", GUILayout.Width(16));
                if (deleted) entryToDelete = node.entry;
                EditorGUILayout.EndHorizontal();
            }
            EditorWindowTools.EndIndentedSection();
        }

        private void DrawDialogueEntryContents(
            DialogueEntry entry,
            ref Link linkToDelete,
            ref DialogueEntry entryToLinkFrom,
            ref DialogueEntry entryToLinkTo,
            ref bool linkToAnotherConversation)
        {
            EditorGUILayout.BeginVertical("button");

            bool changed = DrawDialogueEntryFieldContents();

            // Links:
            changed = DrawDialogueEntryLinks(entry, ref linkToDelete, ref entryToLinkFrom, ref entryToLinkTo, ref linkToAnotherConversation) || changed;

            EditorGUILayout.EndVertical();

            if (changed)
            {
                ResetDialogueEntryText(entry);
                SetDatabaseDirty("Links Changed [2]");
            }
        }

        public bool DrawDialogueEntryFieldContents()
        {
            if (currentEntry == null) return false;

            EditorGUI.BeginChangeCheck();

            DialogueEntry entry = currentEntry;
            bool isStartEntry = (entry == startEntry) || (entry.id == 0);
            EditorGUI.BeginDisabledGroup(isStartEntry);
            entry.id = StringToInt(EditorGUILayout.TextField(new GUIContent("ID", "Internal ID. Change at your own risk."), entry.id.ToString()), entry.id);

            // Title:
            entry.Title = EditorGUILayout.TextField(new GUIContent("Title", "Optional title for your reference only."), entry.Title);
            EditorGUI.EndDisabledGroup();

            if (isStartEntry)
            {
                EditorGUILayout.HelpBox("This is the START entry. In most cases, you should leave this entry alone and begin your conversation with its child entries.", MessageType.Warning);
            }

            // Description:
            var description = Field.Lookup(entry.fields, "Description");
            if (description != null)
            {
                EditorGUILayout.LabelField(new GUIContent("Description", "Description of this entry; notes for the author"));
                description.value = EditorGUILayout.TextArea(description.value);
            }

            // Actor & conversant:
            DrawDialogueEntryParticipants(entry);

            // Is this a group or regular entry:
            entry.isGroup = EditorGUILayout.Toggle(new GUIContent("Group", "Tick to organize children as a group."), entry.isGroup);

            if (!entry.isGroup)
            {
                // Menu text (including localized if defined in template):
                EditorGUILayout.LabelField(new GUIContent("Menu Text", "Response menu text (e.g., short paraphrase). If blank, uses Dialogue Text"));
                entry.DefaultMenuText = EditorGUILayout.TextArea(entry.DefaultMenuText);
                DrawLocalizedVersions(entry.fields, "Menu Text {0}", false, FieldType.Text);

                // Dialogue text (including localized):
                EditorGUILayout.LabelField(new GUIContent("Dialogue Text", "Line spoken by actor. If blank, uses Menu Text."));
                entry.DefaultDialogueText = EditorGUILayout.TextArea(entry.DefaultDialogueText);
                DrawLocalizedVersions(entry.fields, "{0}", true, FieldType.Localization);

                // Sequence (including localized if defined):
                entry.DefaultSequence = SequenceEditorTools.DrawLayout(new GUIContent("Sequence", "Cutscene played when speaking this entry. If set, overrides Dialogue Manager's Default Sequence"), entry.DefaultSequence);
                //---Was:
                //EditorGUILayout.LabelField(new GUIContent("Sequence", "Cutscene played when speaking this entry. If set, overrides Dialogue Manager's Default Sequence"));
                //entry.DefaultSequence = EditorGUILayout.TextArea(entry.DefaultSequence);
                DrawLocalizedVersions(entry.fields, "Sequence {0}", false, FieldType.Text);

                // Response Menu Sequence:
                bool hasResponseMenuSequence = entry.HasResponseMenuSequence();
                if (hasResponseMenuSequence)
                {
                    EditorGUILayout.LabelField(new GUIContent("Response Menu Sequence", "Cutscene played during response menu following this entry."));
                    entry.DefaultResponseMenuSequence = EditorGUILayout.TextArea(entry.DefaultResponseMenuSequence);
                    DrawLocalizedVersions(entry.fields, "Response Menu Sequence {0}", false, FieldType.Text);
                }
                else
                {
                    hasResponseMenuSequence = EditorGUILayout.ToggleLeft(new GUIContent("Add Response Menu Sequence", "Tick to add a cutscene that plays during the response menu that follows this entry."), false);
                    if (hasResponseMenuSequence) entry.DefaultResponseMenuSequence = string.Empty;
                }
            }

            // Conditions and Script:
            int falseConditionIndex = EditorGUILayout.Popup("False Condition Action", GetFalseConditionIndex(entry.falseConditionAction), falseConditionActionStrings);
            entry.falseConditionAction = falseConditionActionStrings[falseConditionIndex];

            // Conditions:
            luaConditionWizard.database = database;
            entry.conditionsString = luaConditionWizard.Draw(new GUIContent("Conditions", "Optional Lua statement that must be true to use this entry."), entry.conditionsString);

            // Script:
            luaScriptWizard.database = database;
            entry.userScript = luaScriptWizard.Draw(new GUIContent("Script", "Optional Lua code to run when entry is spoken."), entry.userScript);

            // Other primary fields defined in template:
            DrawOtherDialogueEntryPrimaryFields(entry);

            // Events:
            entryEventFoldout = EditorGUILayout.Foldout(entryEventFoldout, "Events");
            if (entryEventFoldout) DrawUnityEvents();

            // Notes:
            Field notes = Field.Lookup(entry.fields, "Notes");
            if (notes != null)
            {
                EditorGUILayout.LabelField("Notes");
                notes.value = EditorGUILayout.TextArea(notes.value);
            }

            // All Fields foldout:
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            entryFieldsFoldout = EditorGUILayout.Foldout(entryFieldsFoldout, "All Fields");
            if (entryFieldsFoldout)
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Template", "Add any missing fields from the template."), EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    ApplyDialogueEntryTemplate(entry.fields);
                }
                if (GUILayout.Button(new GUIContent("Copy", "Copy these fields to the clipboard."), EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    CopyFields(entry.fields);
                }
                EditorGUI.BeginDisabledGroup(clipboardFields == null);
                if (GUILayout.Button(new GUIContent("Paste", "Paste the clipboard into these fields."), EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    PasteFields(entry.fields);
                }
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button(new GUIContent(" ", "Add new field."), "OL Plus", GUILayout.Width(16))) entry.fields.Add(new Field());
            }
            EditorGUILayout.EndHorizontal();
            if (entryFieldsFoldout)
            {
                DrawFieldsSection(entry.fields);
            }
            if (EditorGUI.EndChangeCheck()) BuildLanguageListFromFields(entry.fields);

            bool changed = EditorGUI.EndChangeCheck();
            if (changed) SetDatabaseDirty("Dialogue Entry Fields Changed");
            SetTimelineByQTEResult(entry);
            SelectListButtonPositionClass.Instance.SetData(database, currentConversation, currentEntry);
            SelectListButtonPositionClass.Instance.SaveSelectListButtonPosition();

            if (GUILayout.Button("Item"))
            {
                List<Link> links = currentEntry.outgoingLinks;
                DialogueEntry[] entries = links.ToArray().Select(t => database.GetDialogueEntry(t));
                EQTEResult result = EQTEResult.Failure;
                for (int i = 0; i < entries.Length; i++)
                {
                    Field qteResultField = entries[i].fields.Find(t => t.title == "EQTEResult" && t.type == FieldType.Item && t.value == ((int)result).ToString());
                    Debug.Log(qteResultField.value);
                }
            }

            return changed;
        }

        private void SetTimelineByQTEResult(DialogueEntry entry)
        {
            entry.eQTEResult = (EQTEResult)EditorGUILayout.EnumPopup("QTEResult", entry.eQTEResult);

            //List<string> timelineList = new List<string>();
            //timelineList.Add("None");
            //GameObject timelineGO = GameObject.Find("Timeline");
            //if (timelineGO != null)
            //{
            //    Transform[] timelineTFs = timelineGO.GetComponentsInChildren<Transform>();
            //    foreach (var item in timelineTFs)
            //    {
            //        if (item.name != timelineGO.name)
            //        {
            //            timelineList.Add(item.name);
            //        }
            //    }

            //    entry.timelineNameIndex = EditorGUILayout.Popup("Timeline", entry.timelineNameIndex, timelineList.ToArray());
            //    //if (entry.timelineNameIndex != 0)
            //    //{
            //    //    string[] sequenceStrs = entry.Sequence.Split(';');
            //    //    int index = Array.FindIndex(sequenceStrs, t => t.Contains(timelineList[entry.timelineNameIndex]));
            //    //    if (index >= 0)
            //    //    {
            //    //        sequenceStrs[index] = "Timeline(play," + timelineList[entry.timelineNameIndex] + ");";
            //    //        string sequenceStr = "";
            //    //        foreach (var item in sequenceStrs)
            //    //        {
            //    //            sequenceStr += item;
            //    //        }
            //    //        entry.Sequence = sequenceStr;
            //    //    }
            //    //    else
            //    //    {
            //    //        entry.Sequence += "Timeline(play," + timelineList[entry.timelineNameIndex] + ");";
            //    //    }
            //    //}
            //}
        }

        private static List<string> dialogueEntryBuiltInFieldTitles = new List<string>(new string[] { "Title", "Description", "Actor", "Conversant", "Dialogue Text" });

        private void DrawOtherDialogueEntryPrimaryFields(DialogueEntry entry)
        {
            if (entry == null || entry.fields == null || template.dialogueEntryPrimaryFieldTitles == null) return;
            foreach (var field in entry.fields)
            {
                var fieldTitle = field.title;
                if (string.IsNullOrEmpty(fieldTitle)) continue;
                if (!template.dialogueEntryPrimaryFieldTitles.Contains(field.title)) continue;
                if (dialogueEntryBuiltInFieldTitles.Contains(fieldTitle)) continue;
                if (fieldTitle.StartsWith("Menu Text") || fieldTitle.StartsWith("Sequence") || fieldTitle.StartsWith("Response Menu Sequence")) continue;
                EditorGUILayout.BeginHorizontal();
                DrawField(field, false, false);
                EditorGUILayout.EndHorizontal();
            }
        }

        private DialogueEntry serializedObjectCurrentEntry = null;
        private SerializedProperty onExecuteProperty = null;

        private void ResetUnityEventSection()
        {
            serializedObjectCurrentEntry = null;
            onExecuteProperty = null;
        }

        private void DrawUnityEvents()
        {
            if (serializedObject == null)
            {
                EditorGUILayout.LabelField("Error displaying UnityEvent. Please report to developer.");
                return;
            }
            if (serializedObjectCurrentEntry != currentEntry)
            {
                serializedObject.Update();
                serializedObjectCurrentEntry = currentEntry;
                var conversationsProperty = serializedObject.FindProperty("conversations");
                if (conversationsProperty == null || !conversationsProperty.isArray) return;
                SerializedProperty conversationProperty = null;
                for (int i = 0; i < conversationsProperty.arraySize; i++)
                {
                    var sp = conversationsProperty.GetArrayElementAtIndex(i);
                    if (sp.FindPropertyRelative("id").intValue == currentConversation.id)
                    {
                        conversationProperty = sp;
                        break;
                    }
                }
                if (conversationProperty == null) return;
                var entriesProperty = conversationProperty.FindPropertyRelative("dialogueEntries");
                if (entriesProperty == null || !entriesProperty.isArray) return;
                SerializedProperty entryProperty = null;
                for (int i = 0; i < entriesProperty.arraySize; i++)
                {
                    var sp = entriesProperty.GetArrayElementAtIndex(i);
                    if (sp.FindPropertyRelative("id").intValue == currentEntry.id)
                    {
                        entryProperty = sp;
                        break;
                    }
                }
                if (entryProperty == null) return;
                onExecuteProperty = entryProperty.FindPropertyRelative("onExecute");
            }
            if (onExecuteProperty != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(onExecuteProperty);
                if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
            }
        }

        public bool DrawDialogueEntryInspector()
        {
            // Draw field contents:
            bool changedFieldContents = DrawDialogueEntryFieldContents();

            // Draw links:
            Link linkToDelete = null;
            DialogueEntry entryToLinkFrom = null;
            DialogueEntry entryToLinkTo = null;
            bool linkToAnotherConversation = false;
            bool changedLinks = DrawDialogueEntryLinks(currentEntry, ref linkToDelete, ref entryToLinkFrom, ref entryToLinkTo, ref linkToAnotherConversation);
            // Handle deletion:
            if (linkToDelete != null)
            {
                changedLinks = true;
                DeleteLink(linkToDelete);
                InitializeDialogueTree();
            }
            // Handle linking:
            if (entryToLinkFrom != null)
            {
                changedLinks = true;
                if (entryToLinkTo == null)
                {
                    if (linkToAnotherConversation)
                    {
                        LinkToAnotherConversation(entryToLinkFrom);
                    }
                    else
                    {
                        LinkToNewEntry(entryToLinkFrom);
                    }
                }
                else
                {
                    CreateLink(entryToLinkFrom, entryToLinkTo);
                }
                InitializeDialogueTree();
            }

            return changedFieldContents || changedLinks;
        }

        private void ApplyDialogueEntryTemplate(List<Field> fields)
        {
            if (template == null || template.dialogueEntryFields == null || fields == null) return;
            ApplyTemplate(fields, template.dialogueEntryFields);
        }

        private void DrawDialogueEntryParticipants(DialogueEntry entry)
        {
            // Make sure we have references to the actor and conversant fields:
            VerifyParticipantField(entry, "Actor", ref currentEntryActor);
            VerifyParticipantField(entry, "Conversant", ref currentEntryConversant);

            // If actor and conversant are unassigned, use conversation's values:
            if (IsActorIDUnassigned(currentEntryActor)) currentEntryActor.value = currentConversation.ActorID.ToString();
            if (IsActorIDUnassigned(currentEntryConversant)) currentEntryConversant.value = currentConversation.ConversantID.ToString(); ;

            // Participant IDs:
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            DrawParticipantField(currentEntryActor, "Speaker of this entry.");
            DrawParticipantField(currentEntryConversant, "Listener.");
            EditorGUILayout.EndVertical();
            var swap = GUILayout.Button(new GUIContent(" ", "Swap participants."), "Popup", GUILayout.Width(24));
            EditorGUILayout.EndHorizontal();

            if (swap) SwapParticipants(ref currentEntryActor, ref currentEntryConversant);
        }

        private void VerifyParticipantField(DialogueEntry entry, string fieldTitle, ref Field participantField)
        {
            if (participantField == null) participantField = Field.Lookup(entry.fields, fieldTitle);
            if (participantField == null)
            {
                participantField = new Field(fieldTitle, string.Empty, FieldType.Actor);
                entry.fields.Add(participantField);
                SetDatabaseDirty("Add Participant Field");
            }
        }

        private bool IsActorIDUnassigned(Field field)
        {
            return (field == null) || string.IsNullOrEmpty(field.value) || string.Equals(field.value, "0");
        }

        private void DrawParticipantField(Field participantField, string tooltipText)
        {
            string newValue = DrawAssetPopup<Actor>(participantField.value, (database != null) ? database.actors : null, new GUIContent(participantField.title, tooltipText));
            if (newValue != participantField.value)
            {
                participantField.value = newValue;
                ResetDialogueEntryText();
                SetDatabaseDirty("Change Participant");
            }
        }

        private void SwapParticipants(ref Field currentActor, ref Field currentConversant)
        {
            var newActorValue = currentConversant.value;
            var newConversantValue = currentActor.value;
            currentActor.value = newActorValue;
            currentConversant.value = newConversantValue;
        }

        private int GetFalseConditionIndex(string falseConditionString)
        {
            for (int i = 0; i < falseConditionActionStrings.Length; i++)
            {
                if (string.Equals(falseConditionString, falseConditionActionStrings[i]))
                {
                    return i;
                }
            }
            return 0;
        }

        private bool DrawDialogueEntryLinks(
            DialogueEntry entry,
            ref Link linkToDelete,
            ref DialogueEntry entryToLinkFrom,
            ref DialogueEntry entryToLinkTo,
            ref bool linkToAnotherConversation)
        {
            if (currentConversation == null) return false;

            bool changed = false;
            try
            {
                EditorGUI.BeginChangeCheck();

                List<GUIContent> destinationList = new List<GUIContent>();
                destinationList.Add(new GUIContent("(Link To)", string.Empty));
                destinationList.Add(new GUIContent("(Another Conversation)", string.Empty));
                destinationList.Add(new GUIContent("(New Entry)", string.Empty));
                for (int i = 0; i < currentConversation.dialogueEntries.Count; i++)
                {
                    var destinationEntry = currentConversation.dialogueEntries[i];
                    if (destinationEntry != entry) destinationList.Add(new GUIContent(Tools.StripRichTextCodes(GetDialogueEntryText(destinationEntry)), string.Empty));
                }
                int destinationIndex = EditorGUILayout.Popup(new GUIContent("Links To:", "Add a link to another entry. Select (New Entry) to create and link to a new entry."), 0, destinationList.ToArray());
                if (destinationIndex > 0)
                {
                    entryToLinkFrom = entry;
                    if (destinationIndex == 1)
                    { // (Another Conversation)
                        entryToLinkTo = null;
                        linkToAnotherConversation = true;
                    }
                    else if (destinationIndex == 2)
                    { // (New Entry)
                        entryToLinkTo = null;
                        linkToAnotherConversation = false;
                    }
                    else
                    {
                        int destinationID = AssetListIndexToID(destinationIndex, destinationList.ToArray());
                        entryToLinkTo = currentConversation.dialogueEntries.Find(e => e.id == destinationID);
                        if (entryToLinkTo == null)
                        {
                            entryToLinkFrom = null;
                            Debug.LogError(string.Format("{0}: Couldn't find destination dialogue entry in database.", DialogueDebug.Prefix));
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    return false;
                }
                int linkIndexToMoveUp = -1;
                int linkIndexToMoveDown = -1;
                if ((entry != null) && (entry.outgoingLinks != null))
                {
                    for (int linkIndex = 0; linkIndex < entry.outgoingLinks.Count; linkIndex++)
                    {
                        Link link = entry.outgoingLinks[linkIndex];
                        EditorGUILayout.BeginHorizontal();

                        if (link.destinationConversationID == currentConversation.id)
                        {
                            // Is a link to an entry in the current conversation, so handle normally:
                            DialogueEntry linkEntry = database.GetDialogueEntry(link);
                            if (linkEntry != null)
                            {
                                string linkText = (linkEntry == null) ? string.Empty
                                    : (linkEntry.isGroup ? GetDialogueEntryText(linkEntry) : linkEntry.ResponseButtonText);
                                if (string.IsNullOrEmpty(linkText)) linkText = "<" + linkEntry.Title + ">";
                                GUIStyle linkButtonStyle = GetLinkButtonStyle(linkEntry);
                                if (GUILayout.Button(linkText, linkButtonStyle))
                                {
                                    currentEntry = database.GetDialogueEntry(link);
                                    EditorGUILayout.EndHorizontal();
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            // Cross-conversation link:
                            link.destinationConversationID = DrawConversationsPopup(link.destinationConversationID);
                            link.destinationDialogueID = DrawCrossConversationEntriesPopup(link.destinationConversationID, link.destinationDialogueID);
                        }

                        EditorGUI.BeginDisabledGroup(linkIndex == 0);
                        if (GUILayout.Button(new GUIContent("↑", "Move up"), EditorStyles.miniButton, GUILayout.Width(22))) linkIndexToMoveUp = linkIndex;
                        EditorGUI.EndDisabledGroup();
                        EditorGUI.BeginDisabledGroup(linkIndex == entry.outgoingLinks.Count - 1);
                        if (GUILayout.Button(new GUIContent("↓", "Move down"), EditorStyles.miniButton, GUILayout.Width(22))) linkIndexToMoveDown = linkIndex;
                        EditorGUI.EndDisabledGroup();
                        link.priority = (ConditionPriority)EditorGUILayout.Popup((int)link.priority, priorityStrings, GUILayout.Width(100));
                        bool deleted = GUILayout.Button(new GUIContent(" ", "Delete link."), "OL Minus", GUILayout.Width(16));
                        if (deleted) linkToDelete = link;
                        EditorGUILayout.EndHorizontal();
                    }
                }

                if (linkIndexToMoveUp != -1) MoveLink(entry, linkIndexToMoveUp, -1);
                if (linkIndexToMoveDown != -1) MoveLink(entry, linkIndexToMoveDown, 1);
            }
            catch (NullReferenceException)
            {
                // Hide error if it occurs.
            }
            finally
            {
                changed = EditorGUI.EndChangeCheck();
                if (changed) SetDatabaseDirty("Links Changed [1]");
            }
            return changed;
        }

        private int DrawConversationsPopup(int conversationID)
        {
            List<string> conversations = new List<string>();
            int index = -1;
            for (int i = 0; i < database.conversations.Count; i++)
            {
                var conversation = database.conversations[i];
                conversations.Add(conversation.Title + " [" + conversation.id + "]");
                if (conversation.id == conversationID)
                {
                    index = i;
                }
            }
            index = EditorGUILayout.Popup(index, conversations.ToArray());
            if (0 <= index && index < database.conversations.Count && (database.conversations[index].id != currentConversation.id))
            {
                return database.conversations[index].id;
            }
            else
            {
                return -1;
            }
        }

        private int DrawCrossConversationEntriesPopup(int conversationID, int entryID)
        {
            var conversation = database.GetConversation(conversationID);
            List<string> entries = new List<string>();
            int index = -1;
            if (conversation != null)
            {
                for (int i = 0; i < conversation.dialogueEntries.Count; i++)
                {
                    var entry = conversation.dialogueEntries[i];
                    entries.Add(GetCrossConversationEntryText(entry));
                    if (entry.id == entryID)
                    {
                        index = i;
                    }
                }
            }
            EditorGUI.BeginDisabledGroup(conversation == null);
            index = EditorGUILayout.Popup(index, entries.ToArray());
            EditorGUI.EndDisabledGroup();
            if ((conversation != null) && (0 <= index && index < conversation.dialogueEntries.Count))
            {
                return conversation.dialogueEntries[index].id;
            }
            else
            {
                return -1;
            }
        }

        private string GetCrossConversationEntryText(DialogueEntry entry)
        {
            //if (entry.id == 0) return "[0] <START>";
            var text = entry.DialogueText;
            if (string.IsNullOrEmpty(text))
            {
                text = entry.MenuText;
                if (string.IsNullOrEmpty(text))
                {
                    var title = entry.Title;
                    if (!string.IsNullOrEmpty(title)) text = "<" + title + ">";
                    if (string.IsNullOrEmpty(text))
                    {
                        text = Field.LookupValue(entry.fields, "Description");
                    }
                    if (entry.isGroup) text = "{group} " + text;
                }
            }
            text = "[" + entry.id + "] " + text;
            return Tools.StripRichTextCodes(text);
        }

        private void MoveLink(DialogueEntry entry, int linkIndex, int direction)
        {
            if ((entry != null) && (0 <= linkIndex && linkIndex < entry.outgoingLinks.Count))
            {
                int newIndex = Mathf.Clamp(linkIndex + direction, 0, entry.outgoingLinks.Count - 1);
                Link link = entry.outgoingLinks[linkIndex];
                entry.outgoingLinks.RemoveAt(linkIndex);
                entry.outgoingLinks.Insert(newIndex, link);
                SetDatabaseDirty("Move Link");
            }
        }

        private int AssetListIndexToID(int index, GUIContent[] list)
        {
            if ((0 <= index) && (index < list.Length))
            {
                Regex rx = new Regex(@"^\[[0-9]+\]");
                Match match = rx.Match(list[index].text);
                if (match.Success)
                {
                    int id = 0;
                    string matchString = match.ToString();
                    string idString = matchString.Substring(1, matchString.Length - 2);
                    int.TryParse(idString, out id);
                    return id;
                }
            }
            return -1;
        }

        private void CreateLink(DialogueEntry source, DialogueEntry destination)
        {
            if ((source != null) && (destination != null))
            {
                Link link = new Link();
                link.originConversationID = currentConversation.id;
                link.originDialogueID = source.id;
                link.destinationConversationID = currentConversation.id;
                link.destinationDialogueID = destination.id;
                source.outgoingLinks.Add(link);
                SetDatabaseDirty("Create Link");
            }
        }

        private void LinkToNewEntry(DialogueEntry source)
        {
            if (source != null)
            {
                DialogueEntry newEntry = CreateNewDialogueEntry("New Dialogue Entry");
                newEntry.ActorID = source.ConversantID;
                newEntry.ConversantID = (source.ActorID == source.ConversantID) ? database.playerID : source.ActorID;
                newEntry.canvasRect = new Rect(source.canvasRect.x, source.canvasRect.y + source.canvasRect.height + 20, source.canvasRect.width, source.canvasRect.height);
                Link link = new Link();
                link.originConversationID = currentConversation.id;
                link.originDialogueID = source.id;
                link.destinationConversationID = currentConversation.id;
                link.destinationDialogueID = newEntry.id;
                source.outgoingLinks.Add(link);
                currentEntry = newEntry;
                SetDatabaseDirty("Link to New Entry");
            }
        }

        private void LinkToAnotherConversation(DialogueEntry source)
        {
            Link link = new Link();
            link.originConversationID = currentConversation.id;
            link.originDialogueID = source.id;
            link.destinationConversationID = -1;
            link.destinationDialogueID = -1;
            source.outgoingLinks.Add(link);
            SetDatabaseDirty("Link to Conversation");
        }

        private DialogueEntry CreateNewDialogueEntry(string title)
        {
            DialogueEntry entry = template.CreateDialogueEntry(GetNextDialogueEntryID(), currentConversation.id, title ?? string.Empty);
            currentConversation.dialogueEntries.Add(entry);
            SetDatabaseDirty("Create New Dialogue Entry");
            return entry;
        }

        private int GetNextDialogueEntryID()
        {
            int highestID = -1;
            currentConversation.dialogueEntries.ForEach(entry => highestID = Mathf.Max(highestID, entry.id));
            return highestID + 1;
        }

        private void DeleteLink(Link linkToDelete)
        {
            if ((currentConversation != null) && (linkToDelete != null))
            {
                if (EditorUtility.DisplayDialog("Delete Link?", "Are you sure you want to delete this link?", "Delete", "Cancel"))
                {
                    // Count # dialogue entries linking to same:
                    int numLinksToDestination = 0;
                    for (int i = 0; i < currentConversation.dialogueEntries.Count; i++)
                    {
                        var entry = currentConversation.dialogueEntries[i];
                        for (int j = 0; j < entry.outgoingLinks.Count; j++)
                        {
                            var link = entry.outgoingLinks[j];
                            if (link.destinationDialogueID == linkToDelete.destinationDialogueID)
                            {
                                numLinksToDestination++;
                            }
                        }
                    }

                    // Delete link:
                    DialogueEntry origin = currentConversation.dialogueEntries.Find(e => e.id == linkToDelete.originDialogueID);
                    if (origin != null)
                    {
                        origin.outgoingLinks.Remove(linkToDelete);
                    }

                    // If only 1 linking to same, delete target dialogue entry:
                    //--- Removed this behavior to keep consistent with right-clicking on link and selecting Delete:
                    //DialogueEntry destination = currentConversation.dialogueEntries.Find(e => e.id == linkToDelete.destinationDialogueID);
                    //if ((numLinksToDestination <= 1) && (destination != null))
                    //{
                    //    if (currentEntry == destination) ResetCurrentEntry();
                    //    currentConversation.dialogueEntries.Remove(destination);
                    //}

                    //if (currentConversation.dialogueEntries.Count <= 0)
                    //{
                    //    database.conversations.Remove(currentConversation);
                    //    ResetDialogueTree();
                    //}
                    SetDatabaseDirty("Delete Link");
                }
            }
        }
    }

    internal class SelectListButtonPositionClass
    {
        private bool isSelectByName = true;
        private string UIPosName = "Select List Button Group";
        private UnityEngine.Object go = null;
        private DialogueDatabase _database;
        private Conversation _conversation;
        private DialogueEntry _entry;
        private Vector3 UIPosition;
        private Quaternion UIRotation;
        private Vector3 UIScale;
        private GameObject ScreenCanvas;
        private GameObject WorldCanvas;
        private GameObject selectListButtonPosition;
        private static SelectListButtonPositionClass instance;

        public static SelectListButtonPositionClass Instance
        {
            get
            {
                if (instance == null)
                    instance = new SelectListButtonPositionClass();
                return instance;
            }
        }

        public void SetData(DialogueDatabase database, Conversation conversation, DialogueEntry entry)
        {
            selectListButtonPosition = GameObject.Find(UIPosName);
            ScreenCanvas = GameObject.Find("ScreenCanvas");
            WorldCanvas = GameObject.Find("WorldCanvas");
            _database = database;
            _conversation = conversation;
            _entry = entry;
        }

        public void SaveSelectListButtonPosition()
        {
            EditorGUILayout.Space();
            UIPosName = EditorGUILayout.TextField("选择列表UI位置物体名字：", UIPosName);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存选择列表UI位置"))
            {
                SavePosition(isSelectByName, selectListButtonPosition);
            }
            if (GUILayout.Button("显示位置"))
            {
                DisplayPosition();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void DisplayPosition()
        {
            string title = SelectCanvas() ? "WorldUI.Position" : "ScreenUI.Position";
            Field uiData = _entry.fields.Find(t => t.title.Contains(title));
            if (uiData != null)
            {
                string posStr = "";
                UIPosition = ConverVector3(uiData.value, out posStr);
                selectListButtonPosition.transform.localPosition = UIPosition;

                string rotationStr = uiData.value.Replace(posStr, "");
                string rotStr = "";
                UIRotation = ConvetQuaternion(rotationStr, out rotStr);
                selectListButtonPosition.transform.rotation = UIRotation;

                string scaleStr = uiData.value.Replace(posStr + rotStr, "");
                string scaStr = "";
                UIScale = ConverVector3(scaleStr, out scaStr);
                selectListButtonPosition.transform.localScale = UIScale;

                Selection.activeGameObject = selectListButtonPosition;
            }
            else
            {
                Debug.LogError("该 Dialogue Entry 没有位置信息");
            }
        }

        private bool SelectCanvas()
        {
            DialogueEntry startEntry = _database.GetDialogueEntry(_conversation.id, 0);
            Field isWorldCanvas = startEntry.fields.Find((t) => t.title == "isWorldCanvas");
            if (isWorldCanvas == null || isWorldCanvas.value == "True")
            {
                selectListButtonPosition.transform.SetParent(WorldCanvas.transform);
                return true;
            }
            else
            {
                selectListButtonPosition.transform.SetParent(ScreenCanvas.transform);
                return false;
            }
        }

        private Vector3 ConverVector3(string str, out string posStr)
        {
            Regex regex = new Regex(@"\([\d\.\,\ \-]*\)");
            Match match = regex.Match(str);
            posStr = match.Value;
            string position = match.Value.Replace("(", "").Replace(")", "");
            string[] pos = position.Split(',');
            return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        }

        private Quaternion ConvetQuaternion(string str, out string rotStr)
        {
            Regex regex = new Regex(@"\([\d\.\,\ \-]*\)");
            Match match = regex.Match(str);
            rotStr = match.Value;
            string rotation = match.Value.Replace("(", "").Replace(")", "");
            string[] rot = rotation.Split(',');
            return new Quaternion(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]), float.Parse(rot[3]));
        }

        private void SavePosition(bool isSelectByName, GameObject selectListButtonPosition)
        {
            if (isSelectByName == true)
            {
                if (selectListButtonPosition == null)
                    Debug.LogError("请填写选择列表UI位置物体名字");
                else
                    SaveUIData(selectListButtonPosition);
            }
        }

        private void SaveUIData(GameObject selectListButtonPosition)
        {
            Vector3 pos = selectListButtonPosition.transform.localPosition;
            Quaternion rotation = selectListButtonPosition.transform.rotation;
            Vector3 scale = selectListButtonPosition.transform.localScale;

            string positionStr = "(" + pos.x + ", " + pos.y + ", " + pos.z + ")";
            string rotationStr = "(" + rotation.x + ", " + rotation.y + ", " + rotation.z + ", " + rotation.w + ")";
            string scaleStr = "(" + scale.x + ", " + scale.y + ", " + scale.z + ")";
            string title = SelectCanvas() ? "WorldUI.Position" : "ScreenUI.Position";
            Field uiData = _entry.fields.Find(t => t.title.Contains(title));
            if (uiData != null)
            {
                uiData.value = positionStr + rotationStr + scaleStr;
            }
            else
            {
                uiData = new Field();
                uiData.title = title;
                uiData.value = positionStr + rotationStr + scaleStr;
                uiData.type = FieldType.Text;
                _entry.fields.Add(uiData);
            }
        }
    }
}