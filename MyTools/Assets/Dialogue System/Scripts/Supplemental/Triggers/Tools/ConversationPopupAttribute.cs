using UnityEngine;

namespace PixelCrushers.DialogueSystem {
	
	public class ConversationPopupAttribute : PropertyAttribute {
		
		public bool showReferenceDatabase = false;
		
		public ConversationPopupAttribute(bool showReferenceDatabase = false) {
			this.showReferenceDatabase = showReferenceDatabase;
		}
	}
}
