using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// Overrides the dialogue UI for conversations involving the game object. To use this
    /// component, add it to a game object. When the game object is a conversant, the conversation
    /// will use the dialogue UI on this component instead of the UI on the DialogueManager.
    /// </summary>
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER
    [HelpURL("http://pixelcrushers.com/dialogue_system/manual/html/override_dialogue_u_i.html")]
#endif
    [AddComponentMenu("Dialogue System/UI/Override/Override Dialogue UI")]
    public class OverrideDialogueUI : OverrideUIBase
    {

        /// <summary>
        /// The dialogue UI to use for the game object this component is attached to.
        /// </summary>
        [Tooltip("Use this dialogue UI for this GameObject.")]
        public GameObject ui;

    }

}
