using UnityEditor;

namespace PixelCrushers.DialogueSystem.Articy
{

    /// <summary>
    /// This class provides editor tools to manage articy converter prefs. It allows the converter to save
    /// prefs to EditorPrefs between sessions.
    /// </summary>
    public static class ConverterPrefsTools
    {

        private const string ArticyProjectFilenameKey = "PixelCrushers.DialogueSystem.ArticyProjectFilename";
        private const string ArticyPortraitFolderKey = "PixelCrushers.DialogueSystem.ArticyPortraitFolder";
        private const string ArticyStageDirectionsAreSequencesKey = "PixelCrushers.DialogueSystem.StageDirectionsAreSequences";
        private const string ArticyFlowFragmentModeKey = "PixelCrushers.DialogueSystem.FlowFragmentMode";
        private const string ArticyOutputFolderKey = "PixelCrushers.DialogueSystem.ArticyOutput";
        private const string ArticyOverwriteKey = "PixelCrushers.DialogueSystem.ArticyOverwrite";
        private const string ArticyConversionSettingsKey = "PixelCrushers.DialogueSystem.ArticyConversionSettings";
        private const string ArticyEncodingKey = "PixelCrushers.DialogueSystem.ArticyEncoding";
        private const string ArticyRecursionKey = "PixelCrushers.DialogueSystem.ArticyRecursion";
        private const string ArticyDropdownsKey = "PixelCrushers.DialogueSystem.ArticyDropdowns";
        private const string ArticySlotsKey = "PixelCrushers.DialogueSystem.ArticySlots";
        private const string ArticyDirectConversationLinksToEntry1 = "PixelCrushers.DialogueSystem.DirectConversationLinksToEntry1";
        private const string ArticyFlowFragmentScriptKey = "PixelCrushers.DialogueSystem.ArticyFlowFragmentScript";
        private const string ArticyVoiceOverPropertyKey = "PixelCrushers.DialogueSystem.ArticyVoiceOverPropertyKey";
        private const string ArticyLocalizationXlsKey = "PixelCrushers.DialogueSystem.ArticyLocalizationXlsxKey";

        public static ConverterPrefs Load()
        {
            var converterPrefs = new ConverterPrefs();
            converterPrefs.ProjectFilename = EditorPrefs.GetString(ArticyProjectFilenameKey);
            converterPrefs.PortraitFolder = EditorPrefs.GetString(ArticyPortraitFolderKey);
            converterPrefs.StageDirectionsAreSequences = EditorPrefs.HasKey(ArticyStageDirectionsAreSequencesKey) ? EditorPrefs.GetBool(ArticyStageDirectionsAreSequencesKey) : true;
            converterPrefs.FlowFragmentMode = (ConverterPrefs.FlowFragmentModes)(EditorPrefs.HasKey(ArticyFlowFragmentModeKey) ? EditorPrefs.GetInt(ArticyFlowFragmentModeKey) : 0);
            converterPrefs.OutputFolder = EditorPrefs.GetString(ArticyOutputFolderKey, "Assets");
            converterPrefs.Overwrite = EditorPrefs.GetBool(ArticyOverwriteKey, false);
            converterPrefs.ConversionSettings = ConversionSettings.FromXml(EditorPrefs.GetString(ArticyConversionSettingsKey));
            converterPrefs.EncodingType = EditorPrefs.HasKey(ArticyEncodingKey) ? (EncodingType)EditorPrefs.GetInt(ArticyEncodingKey) : EncodingType.Default;
            converterPrefs.RecursionMode = EditorPrefs.HasKey(ArticyRecursionKey) ? (ConverterPrefs.RecursionModes)EditorPrefs.GetInt(ArticyRecursionKey) : ConverterPrefs.RecursionModes.On;
            converterPrefs.ConvertDropdownsAs = EditorPrefs.HasKey(ArticyDropdownsKey) ? (ConverterPrefs.ConvertDropdownsModes)EditorPrefs.GetInt(ArticyDropdownsKey) : ConverterPrefs.ConvertDropdownsModes.Ints;
            converterPrefs.ConvertSlotsAs = EditorPrefs.HasKey(ArticySlotsKey) ? (ConverterPrefs.ConvertSlotsModes)EditorPrefs.GetInt(ArticySlotsKey) : ConverterPrefs.ConvertSlotsModes.DisplayName;
            converterPrefs.DirectConversationLinksToEntry1 = EditorPrefs.GetBool(ArticyDirectConversationLinksToEntry1, false);
            converterPrefs.FlowFragmentScript = EditorPrefs.GetString(ArticyFlowFragmentScriptKey, ConverterPrefs.DefaultFlowFragmentScript);
            converterPrefs.VoiceOverProperty = EditorPrefs.GetString(ArticyVoiceOverPropertyKey, ConverterPrefs.DefaultVoiceOverProperty);
            converterPrefs.LocalizationXlsx = EditorPrefs.GetString(ArticyLocalizationXlsKey);
            return converterPrefs;
        }

        public static void Save(ConverterPrefs converterPrefs)
        {
            EditorPrefs.SetString(ArticyProjectFilenameKey, converterPrefs.ProjectFilename);
            EditorPrefs.SetString(ArticyPortraitFolderKey, converterPrefs.PortraitFolder);
            EditorPrefs.SetBool(ArticyStageDirectionsAreSequencesKey, converterPrefs.StageDirectionsAreSequences);
            EditorPrefs.SetInt(ArticyFlowFragmentModeKey, (int)converterPrefs.FlowFragmentMode);
            EditorPrefs.SetString(ArticyOutputFolderKey, converterPrefs.OutputFolder);
            EditorPrefs.SetBool(ArticyOverwriteKey, converterPrefs.Overwrite);
            EditorPrefs.SetString(ArticyConversionSettingsKey, converterPrefs.ConversionSettings.ToXml());
            EditorPrefs.SetInt(ArticyEncodingKey, (int)converterPrefs.EncodingType);
            EditorPrefs.SetInt(ArticyRecursionKey, (int)converterPrefs.RecursionMode);
            EditorPrefs.SetInt(ArticyDropdownsKey, (int)converterPrefs.ConvertDropdownsAs);
            EditorPrefs.SetInt(ArticySlotsKey, (int)converterPrefs.ConvertSlotsAs);
            EditorPrefs.SetBool(ArticyDirectConversationLinksToEntry1, converterPrefs.DirectConversationLinksToEntry1);
            EditorPrefs.SetString(ArticyFlowFragmentScriptKey, converterPrefs.FlowFragmentScript);
            EditorPrefs.SetString(ArticyVoiceOverPropertyKey, converterPrefs.VoiceOverProperty);
            EditorPrefs.SetString(ArticyLocalizationXlsKey, converterPrefs.LocalizationXlsx);
        }

        public static void DeleteEditorPrefs()
        {
            EditorPrefs.DeleteKey(ArticyProjectFilenameKey);
            EditorPrefs.DeleteKey(ArticyPortraitFolderKey);
            EditorPrefs.DeleteKey(ArticyStageDirectionsAreSequencesKey);
            EditorPrefs.DeleteKey(ArticyFlowFragmentModeKey);
            EditorPrefs.DeleteKey(ArticyOutputFolderKey);
            EditorPrefs.DeleteKey(ArticyOverwriteKey);
            EditorPrefs.DeleteKey(ArticyConversionSettingsKey);
            EditorPrefs.DeleteKey(ArticyEncodingKey);
            EditorPrefs.DeleteKey(ArticyRecursionKey);
            EditorPrefs.DeleteKey(ArticyDropdownsKey);
            EditorPrefs.DeleteKey(ArticySlotsKey);
            EditorPrefs.DeleteKey(ArticyDirectConversationLinksToEntry1);
            EditorPrefs.DeleteKey(ArticyFlowFragmentScriptKey);
            EditorPrefs.DeleteKey(ArticyVoiceOverPropertyKey);
            EditorPrefs.DeleteKey(ArticyLocalizationXlsKey);
        }

    }

}