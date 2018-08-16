using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.DialogueEditor
{

    /// <summary>
    /// This part of the Dialogue Editor window implements UpdateTemplateFromAssets().
    /// </summary>
    public partial class DialogueEditorWindow
    {

        private void ConfirmUpdateTemplateFromAssets()
        {
            if (EditorUtility.DisplayDialog("Update template from assets?", "This will update the template with any custom fields you've added to any assets in your dialogue database. You cannot undo this action.", "Update", "Cancel"))
            {
                UpdateTemplateFromAssets();
                Debug.Log(string.Format("{0}: Dialogue Editor template now contains all fields listed in any dialogue database asset.", DialogueDebug.Prefix));
            }
        }

        private void ConfirmApplyTemplateToAssets()
        {
            if (EditorUtility.DisplayDialog("Apply template to assets?", "This will apply the template to all assets. You cannot undo this action.", "Update", "Cancel"))
            {
                ApplyTemplateToAssets();
                Debug.Log(string.Format("{0}: All assets now have all fields listed in the template.", DialogueDebug.Prefix));
            }
        }

        private void ConfirmSyncAssetsAndTemplate()
        {
            if (EditorUtility.DisplayDialog("Sync template and assets?", "This will update the template with any custom fields you've added to assets in your dialogue database, and then apply the updated template to all assets. You cannot undo this action.", "Update", "Cancel"))
            {
                SyncAssetsAndTemplate();
                Debug.Log(string.Format("{0}: Dialogue Editor template now contains all fields listed in any dialogue database asset, and those assets now have all fields listed in the template.", DialogueDebug.Prefix));
            }
        }

        private void SyncAssetsAndTemplate()
        {
            NormalizeActors();
            NormalizeItems();
            NormalizeLocations();
            NormalizeVariables();
            NormalizeConversations();
            NormalizeDialogueEntries();
        }

        private void NormalizeActors()
        {
            NormalizeAssets<Actor>(database.actors, template.actorFields);
        }

        private void NormalizeItems()
        {
            AddMissingFieldsToTemplate(template.questFields, template.itemFields);
            NormalizeAssets<Item>(database.items, template.itemFields);
        }

        private void NormalizeLocations()
        {
            NormalizeAssets<Location>(database.locations, template.locationFields);
        }

        private void NormalizeVariables()
        {
            NormalizeAssets<Variable>(database.variables, template.variableFields);
        }

        private void NormalizeConversations()
        {
            NormalizeAssets<Conversation>(database.conversations, template.conversationFields);
        }

        private void NormalizeDialogueEntries()
        {
            foreach (var conversation in database.conversations)
            {
                foreach (var entry in conversation.dialogueEntries)
                {
                    AddMissingFieldsToTemplate(entry.fields, template.dialogueEntryFields);
                }
            }
            foreach (var conversation in database.conversations)
            {
                foreach (var entry in conversation.dialogueEntries)
                {
                    EnforceTemplateOnFields(entry.fields, template.dialogueEntryFields);
                }
            }
        }

        private void NormalizeAssets<T>(List<T> assets, List<Field> templateFields) where T : Asset
        {
            foreach (var asset in assets)
            {
                AddMissingFieldsToTemplate(asset.fields, templateFields);
            }
            foreach (var asset in assets)
            {
                EnforceTemplateOnFields(asset.fields, templateFields);
            }
            SetDatabaseDirty("Normalize Assets to Template");
        }

        private void AddMissingFieldsToTemplate(List<Field> assetFields, List<Field> templateFields)
        {
            foreach (var field in assetFields)
            {
                if (!Field.FieldExists(templateFields, field.title))
                {
                    templateFields.Add(new Field(field.title, string.Empty, field.type));
                }
            }
        }

        private void EnforceTemplateOnFields(List<Field> fields, List<Field> templateFields)
        {
            List<Field> newFields = new List<Field>();
            for (int i = 0; i < templateFields.Count; i++)
            {
                Field templateField = templateFields[i];
                if (!string.IsNullOrEmpty(templateField.title))
                {
                    newFields.Add(Field.Lookup(fields, templateField.title) ?? new Field(templateField));
                }
            }
            fields.Clear();
            for (int i = 0; i < newFields.Count; i++)
            {
                fields.Add(newFields[i]);
            }
        }

        private void UpdateTemplateFromAssets()
        {
            UpdateTemplateFromAssets<Actor>(database.actors, template.actorFields);
            AddMissingFieldsToTemplate(template.questFields, template.itemFields);
            UpdateTemplateFromAssets<Item>(database.items, template.itemFields);
            UpdateTemplateFromAssets<Item>(database.items, template.questFields);
            UpdateTemplateFromAssets<Location>(database.locations, template.locationFields);
            UpdateTemplateFromAssets<Variable>(database.variables, template.variableFields);
            UpdateTemplateFromAssets<Conversation>(database.conversations, template.conversationFields);
            foreach (var conversation in database.conversations)
            {
                foreach (var entry in conversation.dialogueEntries)
                {
                    AddMissingFieldsToTemplate(entry.fields, template.dialogueEntryFields);
                }
            }
        }

        private void UpdateTemplateFromAssets<T>(List<T> assets, List<Field> templateFields) where T : Asset
        {
            foreach (var asset in assets)
            {
                AddMissingFieldsToTemplate(asset.fields, templateFields);
            }
        }

        private void ApplyTemplateToAssets()
        {
            ApplyTemplateToAssets<Actor>(database.actors, template.actorFields);
            foreach (var item in database.items)
            {
                if (item.IsItem)
                {
                    ApplyTemplate(item.fields, template.itemFields);
                }
                else
                {
                    ApplyTemplate(item.fields, template.questFields);
                }
            }
            ApplyTemplateToAssets<Location>(database.locations, template.locationFields);
            ApplyTemplateToAssets<Variable>(database.variables, template.variableFields);
            ApplyTemplateToAssets<Conversation>(database.conversations, template.conversationFields);
            foreach (var conversation in database.conversations)
            {
                foreach (var entry in conversation.dialogueEntries)
                {
                    ApplyDialogueEntryTemplate(entry.fields);
                }
            }
        }

        private void ApplyTemplateToAssets<T>(List<T> assets, List<Field> templateFields) where T : Asset
        {
            foreach (var asset in assets)
            {
                ApplyTemplate(asset.fields, templateFields);
            }
        }

    }

}