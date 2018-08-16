using UnityEngine;
using System;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// Mediates between a ConversationModel (data) and ConversationView (user interface) to run a
    /// conversation.
    /// </summary>
    public class ConversationController
    {

        /// <summary>
        /// The data model of the conversation.
        /// </summary>
        private ConversationModel model = null;

        /// <summary>
        /// The view (user interface) of the current state of the conversation.
        /// </summary>
        private ConversationView view = null;

        /// <summary>
        /// The current state of the conversation.
        /// </summary>
        private ConversationState state = null;

        /// <summary>
        /// Indicates whether the ConversationController is currently running a conversation.
        /// </summary>
        /// <value>
        /// <c>true</c> if a conversation is active; <c>false</c> if the conversation is done.
        /// </value>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets the actor info for this conversation.
        /// </summary>
        /// <value>
        /// The actor info.
        /// </value>
        public CharacterInfo ActorInfo { get { return (model != null) ? model.ActorInfo : null; } }

        public ConversationModel ConversationModel { get { return model; } }

        public ConversationView ConversationView { get { return view; } }

        /// <summary>
        /// Gets or sets the IsDialogueEntryValid delegate.
        /// </summary>
        public IsDialogueEntryValidDelegate IsDialogueEntryValid
        {
            get { return model.IsDialogueEntryValid; }
            set { model.IsDialogueEntryValid = value; }
        }

        /// <summary>
        /// Gets the conversant info for this conversation.
        /// </summary>
        /// <value>
        /// The conversant info.
        /// </value>
        public CharacterInfo ConversantInfo { get { return (model != null) ? model.ConversantInfo : null; } }

        private bool alwaysForceResponseMenu = false;

        private int currentConversationID;

        public delegate void EndConversationDelegate(ConversationController ConversationController);

        private EndConversationDelegate endConversationHandler = null;

        /// <summary>
        /// Initializes a new ConversationController and starts the conversation in the model.
        /// Also sends OnConversationStart messages to the participants.
        /// </summary>
        /// <param name='model'>
        /// Data model of the conversation.
        /// </param>
        /// <param name='view'>
        /// View to use to provide a user interface for the conversation.
        /// </param>
        /// <param name='endConversationHandler'>
        /// Handler to call to inform when the conversation is done.
        /// </param>
        public ConversationController(ConversationModel model, ConversationView view, bool alwaysForceResponseMenu, EndConversationDelegate endConversationHandler)
        {
            IsActive = true;
            this.model = model;
            this.view = view;
            this.alwaysForceResponseMenu = alwaysForceResponseMenu;
            this.endConversationHandler = endConversationHandler;
            model.InformParticipants("OnConversationStart");
            view.FinishedSubtitleHandler += OnFinishedSubtitle;
            view.SelectedResponseHandler += OnSelectedResponse;
            currentConversationID = model.GetConversationID(model.FirstState);
            SetConversationOverride(model.FirstState);
            GotoState(model.FirstState);
        }

        /// <summary>
        /// Closes the currently-running conversation, which also sends OnConversationEnd messages
        /// to the participants.
        /// </summary>
        public void Close()
        {
            if (IsActive)
            {
                IsActive = false;
                if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Ending conversation.", new System.Object[] { DialogueDebug.Prefix }));
                view.displaySettings.conversationOverrideSettings = null;
                view.FinishedSubtitleHandler -= OnFinishedSubtitle;
                view.SelectedResponseHandler -= OnSelectedResponse;
                view.Close();
                model.InformParticipants("OnConversationEnd", true);
                if (endConversationHandler != null) endConversationHandler(this);
                DialogueManager.Instance.CurrentConversationState = null;
            }
        }

        /// <summary>
        /// Goes to a conversation state. If the state is <c>null</c>, the conversation ends.
        /// </summary>
        /// <param name='state'>
        /// State.
        /// </param>
        public void GotoState(ConversationState state)
        {
            this.state = state;
            DialogueManager.Instance.CurrentConversationState = state;
            if (state != null)
            {
                // Check for change of conversation:
                var newConversationID = model.GetConversationID(state);
                if (newConversationID != currentConversationID)
                {
                    currentConversationID = newConversationID;
                    model.InformParticipants("OnLinkedConversationStart", true);
                    SetConversationOverride(state);
                }
                // Use view to show current state:
                if (state.IsGroup)
                {
                    view.ShowLastNPCSubtitle();
                }
                else
                {
                    var isPCAutoResponseNext = state.HasPCAutoResponse;
                    var isPCResponseMenuNext = state.HasPCResponses && !state.HasNPCResponse;
                    if (isPCAutoResponseNext && !view.displaySettings.GetAlwaysForceResponseMenu()) isPCResponseMenuNext = false;
                    view.StartSubtitle(state.subtitle, isPCResponseMenuNext, isPCAutoResponseNext);
                }
            }
            else
            {
                Close();
            }
        }

        private void SetConversationOverride(ConversationState state)
        {
            view.displaySettings.conversationOverrideSettings = model.GetConversationOverrideSettings(state);
        }

        /// <summary>
        /// Handles the finished subtitle event. If the current conversation state has an NPC 
        /// response, the conversation proceeds to that response. Otherwise, if the current
        /// state has PC responses, then the response menu is shown (or if it has a single
        /// auto-response, the conversation proceeds directly to that response). If there are no
        /// responses, the conversation ends.
        /// </summary>
        /// <param name='sender'>
        /// Sender.
        /// </param>
        /// <param name='e'>
        /// Event args.
        /// </param>
        private void OnFinishedSubtitle(object sender, EventArgs e)
        {
            if (state.HasNPCResponse)
            {
                GotoState(model.GetState(state.FirstNPCResponse.destinationEntry));
            }
            else if (state.HasPCResponses)
            {
                if (state.HasPCAutoResponse && (!alwaysForceResponseMenu || state.pcResponses[0].destinationEntry.isGroup))
                {
                    GotoState(model.GetState(state.PCAutoResponse.destinationEntry));
                }
                else
                {
                    view.StartResponses(state.subtitle, state.pcResponses);
                }
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        /// Handles the selected response event by proceeding to the state associated with the
        /// selected response.
        /// </summary>
        /// <param name='sender'>
        /// Sender.
        /// </param>
        /// <param name='e'>
        /// Selected response event args.
        /// </param>
        private void OnSelectedResponse(object sender, SelectedResponseEventArgs e)
        {
            GotoState(model.GetState(e.DestinationEntry));
        }

        /// <summary>
        /// Follows the first PC response in the current state.
        /// </summary>
        public void GotoFirstResponse()
        {
            if (state != null)
            {
                if (state.pcResponses.Length > 0)
                {
                    view.SelectResponse(new SelectedResponseEventArgs(state.pcResponses[0]));
                }
            }
        }

        /// <summary>
        /// Follows a random PC response in the current state.
        /// </summary>
        public void GotoRandomResponse()
        {
            if (state != null)
            {
                if (state.pcResponses.Length > 0)
                {
                    view.SelectResponse(new SelectedResponseEventArgs(state.pcResponses[UnityEngine.Random.Range(0, state.pcResponses.Length)]));
                }
            }
        }

        public void UpdateResponses()
        {
            if (state != null)
            {
                model.UpdateResponses(state);
                OnFinishedSubtitle(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the portrait texture to use in the UI for an actor.
        /// This is used when the SetPortrait() sequencer command changes an actor's image.
        /// </summary>
        /// <param name="actorName">Actor name.</param>
        /// <param name="portraitTexture">Portrait texture.</param>
        public void SetActorPortraitTexture(string actorName, Texture2D portraitTexture)
        {
            model.SetActorPortraitTexture(actorName, portraitTexture);
            view.SetActorPortraitTexture(actorName, portraitTexture);
        }

    }

}
