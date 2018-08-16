using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem {

	/// <summary>
	/// Keeps track of an active conversation. As conversations finish, another
	/// active conversation record can be promoted to the "current" conversation.
	/// </summary>
	public class ActiveConversationRecord {

		public Transform Actor { get; set; }

		public Transform Conversant { get; set; }

		public ConversationController ConversationController { get; set; }

		public ConversationModel ConversationModel { get { return (ConversationController != null) ? ConversationController.ConversationModel : null; } }

		public ConversationView ConversationView { get { return (ConversationController != null) ? ConversationController.ConversationView : null; } }
		
		public bool IsConversationActive { get { return (ConversationController != null) && ConversationController.IsActive; } }

		public IDialogueUI originalDialogueUI;
		public DisplaySettings originalDisplaySettings;
		public bool isOverrideUIPrefab;

	}

}
