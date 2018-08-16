using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.UnityGUI {
	
	/// <summary>
	/// Applies a flash effect to a GUI control, alternating between visible and invisible.
	/// </summary>
	[AddComponentMenu("Dialogue System/UI/Unity GUI/Controls/Effects/Flash Effect (Unity GUI)")]
	public class FlashEffect : GUIEffect {
		
		public float interval = 0.5f;
		
		private GUIControl control = null;
		
		/// <summary>
		/// Plays the flash effect.
		/// </summary>
		public override IEnumerator Play() {
			control = GetComponent<GUIControl>();
			if (control == null) yield break;
			control.visible = true;
			while (true) {
                yield return StartCoroutine(DialogueTime.WaitForSeconds(interval)); // new WaitForSeconds(interval);
				control.visible = !control.visible;
			}
		}
			
	}

}
