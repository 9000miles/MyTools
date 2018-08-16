using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This class provides a custom drawer for Sequence fields.
    /// </summary>
    public static class SequenceEditorTools
    {

        private enum MenuResult
        {
            None, Delay, DefaultCameraAngle, UpdateTracker, Continue, ContinueTrue, ContinueFalse
        }

        private static MenuResult menuResult = MenuResult.None;

        public static string DrawLayout(GUIContent guiContent, string sequence)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(guiContent);
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(26)))
            {
                DrawContextMenu(sequence);
            }
            EditorGUILayout.EndHorizontal();
            if (menuResult != MenuResult.None)
            {
                sequence = ApplyMenuResult(menuResult, sequence);
                menuResult = MenuResult.None;
            }

            EditorWindowTools.StartIndentedSection();
            var newSequence = EditorGUILayout.TextArea(sequence);
            if (!string.Equals(newSequence, sequence))
            {
                sequence = newSequence;
                GUI.changed = true;
            }
            EditorWindowTools.EndIndentedSection();

            return sequence;
        }

        private static void DrawContextMenu(string sequence)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Help/Overview..."), false, OpenURL, "http://www.pixelcrushers.com/dialogue_system/manual/html/sequences.html");
            menu.AddItem(new GUIContent("Help/Command Reference..."), false, OpenURL, "http://www.pixelcrushers.com/dialogue_system/manual/html/sequencer_commands.html");
            menu.AddSeparator("");
            menu.AddDisabledItem(new GUIContent("Shortcuts:"));
            menu.AddItem(new GUIContent("Delay for subtitle length"), false, SetMenuResult, MenuResult.Delay);
            menu.AddItem(new GUIContent("Cut to speaker's default camera angle"), false, SetMenuResult, MenuResult.DefaultCameraAngle);
            menu.AddItem(new GUIContent("Update quest tracker"), false, SetMenuResult, MenuResult.UpdateTracker);
            menu.AddItem(new GUIContent("Continue/Simulate continue button click"), false, SetMenuResult, MenuResult.Continue);
            menu.AddItem(new GUIContent("Continue/Enable continue button"), false, SetMenuResult, MenuResult.ContinueTrue);
            menu.AddItem(new GUIContent("Continue/Disable continue button"), false, SetMenuResult, MenuResult.ContinueFalse);
            menu.ShowAsContext();
        }

        private static void OpenURL(object url)
        {
            Application.OpenURL(url as string);
        }

        private static void SetMenuResult(object data)
        {
            menuResult = (MenuResult)data;
        }

        private static string ApplyMenuResult(MenuResult menuResult, string sequence)
        {
            GUI.changed = true;
            return sequence + (string.IsNullOrEmpty(sequence) ? string.Empty : ";\n") + GetMenuResultCommand(menuResult);
        }

        private static string GetMenuResultCommand(MenuResult menuResult)
        {
            switch (menuResult)
            {
                case MenuResult.Delay:
                    return "Delay({{end}})";
                case MenuResult.DefaultCameraAngle:
                    return "Camera(default)";
                case MenuResult.UpdateTracker:
                    return "UpdateTracker()"; //---Was: return "SendMessage(SendUpdateTracker,,Dialogue Manager)";
                case MenuResult.Continue:
                    return "Continue()"; //---Was: return "SendMessage(OnContinue,,Dialogue Manager,broadcast)";
                case MenuResult.ContinueTrue:
                    return "SetContinueMode(true)";
                case MenuResult.ContinueFalse:
                    return "SetContinueMode(false)";
                default:
                    return string.Empty;
            }
        }

    }

}
