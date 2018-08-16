using UnityEngine;

namespace PixelCrushers.DialogueSystem {

    /// <summary>
    /// Overrides the display settings for conversations involving the game object. To use this
    /// component, add it to a game object. When the game object is a conversant, the conversation
    /// will use the display settings on this component instead of the settings on the 
    /// DialogueManager.
    /// </summary>
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER
    [HelpURL("http://pixelcrushers.com/dialogue_system/manual/html/override_display_settings.html")]
#endif
    [AddComponentMenu("Dialogue System/UI/Override/Override Display Settings")]
	public class OverrideDisplaySettings : OverrideUIBase {

		/// <summary>
		/// The display settings to use for the game object this component is attached to.
		/// </summary>
		[Tooltip("Use these display settings for this GameObject.")]
		public DisplaySettings displaySettings;
		
	}

}
