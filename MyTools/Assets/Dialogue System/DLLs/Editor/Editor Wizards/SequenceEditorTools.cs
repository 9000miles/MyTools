using UnityEngine;
using UnityEditor;
using System;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This class provides a custom drawer for Sequence fields.
    /// </summary>
    public static class SequenceEditorTools
    {

        private enum MenuResult
        {
            None, DefaultSequence, Delay, DefaultCameraAngle, UpdateTracker, Continue, ContinueTrue, ContinueFalse
        }

        private static MenuResult menuResult = MenuResult.None;

        private enum AudioDragDropCommand { AudioWait, Audio, SALSA }

        private static AudioDragDropCommand audioDragDropCommand = AudioDragDropCommand.AudioWait;

        public static string DrawLayout(GUIContent guiContent, string sequence, ref Rect rect)
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

            switch (Event.current.type)
            {
                case EventType.Repaint:
                    rect = GUILayoutUtility.GetLastRect();
                    break;
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        if (Event.current.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            foreach (var obj in DragAndDrop.objectReferences)
                            {
                                if (obj is AudioClip)
                                {
                                    var clip = obj as AudioClip;
                                    var path = AssetDatabase.GetAssetPath(clip);
                                    if (path.Contains("Resources"))
                                    {
                                        sequence = AddCommandToSequence(sequence, GetCurrentAudioCommand() + "(" + GetResourceName(path) + ")");
                                        GUI.changed = true;
                                    }
                                    else
                                    {
                                        EditorUtility.DisplayDialog("Not in Resources Folder", "Audio clips must be located in the hierarchy of a Resources folder or an AssetBundle.", "OK");
                                    }
                                }
                            }
                        }
                    }
                    break;
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
            menu.AddItem(new GUIContent("Include Dialogue Manager's Default Sequence"), false, SetMenuResult, MenuResult.DefaultSequence);
            menu.AddItem(new GUIContent("Delay for subtitle length"), false, SetMenuResult, MenuResult.Delay);
            menu.AddItem(new GUIContent("Cut to speaker's default camera angle"), false, SetMenuResult, MenuResult.DefaultCameraAngle);
            menu.AddItem(new GUIContent("Update quest tracker"), false, SetMenuResult, MenuResult.UpdateTracker);
            menu.AddItem(new GUIContent("Audio Drag-n-Drop/Help..."), false, ShowSequenceEditorAudioHelp, null);
            menu.AddItem(new GUIContent("Audio Drag-n-Drop/Use AudioWait()"), audioDragDropCommand == AudioDragDropCommand.AudioWait, SetAudioDragDropCommand, AudioDragDropCommand.AudioWait);
            menu.AddItem(new GUIContent("Audio Drag-n-Drop/Use Audio()"), audioDragDropCommand == AudioDragDropCommand.Audio, SetAudioDragDropCommand, AudioDragDropCommand.Audio);
            menu.AddItem(new GUIContent("Audio Drag-n-Drop/Use SALSA() (3rd party)"), audioDragDropCommand == AudioDragDropCommand.SALSA, SetAudioDragDropCommand, AudioDragDropCommand.SALSA);
            menu.AddItem(new GUIContent("Continue/Simulate continue button click"), false, SetMenuResult, MenuResult.Continue);
            menu.AddItem(new GUIContent("Continue/Enable continue button"), false, SetMenuResult, MenuResult.ContinueTrue);
            menu.AddItem(new GUIContent("Continue/Disable continue button"), false, SetMenuResult, MenuResult.ContinueFalse);
            menu.ShowAsContext();
        }

        private static void OpenURL(object url)
        {
            Application.OpenURL(url as string);
        }

        private static void ShowSequenceEditorAudioHelp(object data)
        {
            EditorUtility.DisplayDialog("Audio Drag and Drop Help", "Select an item in this Audio submenu to specify which command to use when dragging an audio clip onto the Sequence field. Audio clips must be in a Resources folder. Audio commands can use AssetBundles, but not with this drag-and-drop feature.", "OK");
        }

        private static void SetAudioDragDropCommand(object data)
        {
            audioDragDropCommand = (AudioDragDropCommand)data;
        }

        private static string GetCurrentAudioCommand()
        {
            switch (audioDragDropCommand)
            {
                case AudioDragDropCommand.Audio:
                    return "Audio";
                case AudioDragDropCommand.SALSA:
                    return "SALSA";
                default:
                    return "AudioWait";
            }
        }

        private static void SetMenuResult(object data)
        {
            menuResult = (MenuResult)data;
        }

        private static string ApplyMenuResult(MenuResult menuResult, string sequence)
        {
            GUI.changed = true;
            var newCommand = GetMenuResultCommand(menuResult);
            if (string.IsNullOrEmpty(newCommand))
            {
                return sequence;
            }
            else
            {
                return AddCommandToSequence(sequence, newCommand);
            }
        }

        private static string GetMenuResultCommand(MenuResult menuResult)
        {
            switch (menuResult)
            {
                case MenuResult.DefaultSequence:
                    return "{{default}}";
                case MenuResult.Delay:
                    return "Delay({{end}})";
                case MenuResult.DefaultCameraAngle:
                    return "Camera(default)";
                case MenuResult.UpdateTracker:
                    return "UpdateTracker()";
                case MenuResult.Continue:
                    return "Continue()";
                case MenuResult.ContinueTrue:
                    return "SetContinueMode(true)";
                case MenuResult.ContinueFalse:
                    return "SetContinueMode(false)";
                default:
                    return string.Empty;
            }
        }

        private static string AddCommandToSequence(string sequence, string newCommand)
        {
            return sequence + (string.IsNullOrEmpty(sequence) ? string.Empty : ";\n") + newCommand;
        }

        private static string GetResourceName(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            var index = path.IndexOf("Resources/");
            if (index == -1) return string.Empty;
            var s = path.Substring(index + "Resources/".Length);
            index = s.LastIndexOf(".");
            if (index != -1) s = s.Substring(0, index);
            return s;
        }

    }

}
