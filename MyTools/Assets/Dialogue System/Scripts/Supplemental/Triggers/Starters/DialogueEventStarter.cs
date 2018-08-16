using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem {
	
	/// <summary>
	/// This is the base class for all dialogue event trigger components.
	/// </summary>
	public class DialogueEventStarter : MonoBehaviour {

        /// <summary>
        /// Set <c>true</c> if this event should only happen once.
        /// </summary>
        [Tooltip("Only trigger once for this instance of the scene, then destroy this component. NOTE: This is not persistent across scene changes or saved games. It only applies to the current instance of this scene. To make something only happen once for the player's playthrough (including scene changes and saved games), use persistent data components.")]
        public bool once = false;
		
		protected void DestroyIfOnce() {
			if (once) Destroy(this);
		}
		
	}

}
