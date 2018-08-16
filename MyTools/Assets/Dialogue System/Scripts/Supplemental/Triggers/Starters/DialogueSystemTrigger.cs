using UnityEngine;
using System;
using System.Collections;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// The Dialogue System trigger is a general-purpose trigger that can execute most
    /// Dialogue System functions such as starting conversations, barks, alerts, 
    /// sequences, and Lua code.
    /// </summary>
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER
    [HelpURL("http://pixelcrushers.com/dialogue_system/manual/html/dialogue_system_trigger.html")]
#endif
    [AddComponentMenu("Dialogue System/Trigger/Dialogue System Trigger")]
    public class DialogueSystemTrigger : DialogueEventStarter
    {

        /// <summary>
        /// The trigger that starts the conversation.
        /// </summary>
        [DialogueTriggerEvent]
        public DialogueTriggerEvent trigger = DialogueTriggerEvent.OnUse;

        /// <summary>
        /// The conditions under which the trigger will fire.
        /// </summary>
        public Condition condition;

        /// <summary>
        /// Only start if no other conversation is active.
        /// </summary>
        [Tooltip("Only trigger if no other conversation is already active.")]
        public bool exclusive = false;

        // //////////////////////////////////////////////////////////////////////////////////
        // Conversation:

        /// <summary>
        /// The title of the conversation to start.
        /// </summary>
        [ConversationPopup(false)]
        public string conversation = string.Empty;

        /// <summary>
        /// The conversant of the conversation. If not set, this game object. The actor is usually
        /// the entity that caused the trigger (for example, the player that hits the "Use" button
        /// on the conversant, thereby triggering OnUse).
        /// </summary>
        [Tooltip("The other actor (e.g., NPC). If unassigned, this GameObject.")]
        public Transform conversationConversant;

        /// <summary>
        /// The actor to converse with. If not set, the game object that triggered the event.
        /// </summary>
        [Tooltip("The primary actor (e.g., player). If unassigned, the GameObject that triggered the conversation.")]
        public Transform conversationActor;

        /// <summary>
        /// If this is <c>true</c> and no valid entries currently link from the start entry,
        /// don't start the conversation.
        /// </summary>
        [Tooltip("Only trigger if at least one entry's Conditions are currently true.")]
        public bool skipIfNoValidEntries;

        /// <summary>
        /// Set <c>true</c> to stop the conversation if the actor leaves the trigger area.
        /// </summary>
        [Tooltip("")]
        public bool stopConversationOnTriggerExit = false;

        // //////////////////////////////////////////////////////////////////////////////////
        // Bark:

        /// <summary>
        /// The title of the bark conversation.
        /// </summary>
        [ConversationPopup(false)]
        public string barkConversation = string.Empty;

        /// <summary>
        /// The barker.
        /// </summary>
        public Transform barker;

        /// <summary>
        /// The target of the bark.
        /// </summary>
        [Tooltip("Target of the bark. Receives OnBark events.")]
        public Transform barkTarget;

        /// <summary>
        /// Specifies the order to run through the list of barks.
        /// 
        /// - Random: Choose a random bark from the conversation.
        /// - Sequential: Choose the barks in order from first to last, looping at the end.
        /// </summary>
        public BarkOrder barkOrder = BarkOrder.Random;

        /// <summary>
        /// Are barks allowed during conversations?
        /// </summary>
        public bool allowBarksDuringConversations = false;

        /// <summary>
        /// Skip bark if no valid entries.
        /// </summary>
        [Tooltip("Only trigger if at least one entry's Conditions are currently true.")]
        public bool skipBarkIfNoValidEntries;

        /// <summary>
        /// If ticked, bark info is cached during the first bark. This can reduce stutter
        /// when barking on slower mobile devices, but barks are not reevaluated each time
        /// as the state changes, barks use no em formatting codes, and sequences are not
        /// played with barks.
        /// </summary>
        [Tooltip("Cache all lines during first bark. This can reduce stutter when barking on slower mobile devices, but barks' conditions are not reevaluated each time as the state changes, barks use no em formatting codes, and sequences are not played with barks.")]
        public bool cacheBarkLines = false;

        // //////////////////////////////////////////////////////////////////////////////////
        // Sequence:

        [TextArea(1,10)]
        public string sequence = string.Empty;

        /// <summary>
        /// The speaker to use for the sequence (or null if no speaker is needed). Sequence
        /// commands can reference 'speaker' and 'listener', so you may need to define them
        /// in this component.
        /// </summary>
        public Transform sequenceSpeaker;

        /// <summary>
        /// The listener to use for the sequence (or null if no listener is needed). Sequence
        /// commands can reference 'speaker' and 'listener', so you may need to define them
        /// in this component.
        /// </summary>
        public Transform sequenceListener;

        public bool waitOneFrameOnStartOrEnable = true;

        // //////////////////////////////////////////////////////////////////////////////////
        // Quest:

        /// <summary>
        /// If `true`, set the quest state.
        /// </summary>
        public bool setQuestState = true;

        /// <summary>
        /// The name of the quest.
        /// </summary>
        [QuestPopup]
        public string questName;

        /// <summary>
        /// The new state of the quest when triggered.
        /// </summary>
        [QuestState]
        public QuestState questState;

        /// <summary>
        /// If `true`, set the quest entry state.
        /// </summary>
        public bool setQuestEntryState = false;

        /// <summary>
        /// The quest entry number whose state to change.
        /// </summary>
        public int questEntryNumber = 1;

        /// <summary>
        /// The new state of the quest entry when triggered.
        /// </summary>
        [QuestState]
        public QuestState questEntryState;

        // //////////////////////////////////////////////////////////////////////////////////
        // Lua:

        /// <summary>
        /// The lua code to run.
        /// </summary>
        public string luaCode = string.Empty;

        // //////////////////////////////////////////////////////////////////////////////////
        // Alert:

        /// <summary>
        /// An optional gameplay alert message. Leave blank for no message.
        /// </summary>
        public string alertMessage;

        /// <summary>
        /// An optional localized text table to use for the alert message.
        /// </summary>
        public LocalizedTextTable localizedTextTable;

        public float alertDuration = 0;

        // //////////////////////////////////////////////////////////////////////////////////
        // Send Message:

        [Serializable]
        public class SendMessageAction
        {
            public GameObject gameObject = null;
            public string message = "OnUse";
            public string parameter = string.Empty;
        }

        /// <summary>
        /// Targets and messages to send when the trigger fires.
        /// </summary>
        public SendMessageAction[] sendMessages = new SendMessageAction[0];

        [HideInInspector]
        public bool useConversationTitlePicker = true;

        [HideInInspector]
        public bool useBarkTitlePicker = true;

        [HideInInspector]
        public bool useQuestNamePicker = true;

        [HideInInspector]
        public DialogueDatabase selectedDatabase = null;

        /// <summary>
        /// Gets the sequencer used by the current bark, if a bark is playing.
        /// If a bark is not playing, this is undefined. To check if a bark is
        /// playing, check the bark UI's IsPlaying property.
        /// </summary>
        /// <value>The sequencer.</value>
        public Sequencer sequencer { get; private set; }

        private BarkHistory barkHistory;

        private ConversationState cachedState = null;

        private IBarkUI barkUI = null;

        private bool tryingToStart = false;

        public virtual void Awake()
        {
            barkHistory = new BarkHistory(barkOrder);
            sequencer = null;
        }

        public void OnBarkEnd(Transform actor)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnBarkEnd)) TryStart(actor);
        }

        public void OnConversationEnd(Transform actor)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnConversationEnd)) TryStart(actor);
        }

        public void OnSequenceEnd(Transform actor)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnSequenceEnd)) TryStart(actor);
        }

        public void OnUse(Transform actor)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnUse)) TryStart(actor);
        }

        public void OnUse(string message)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnUse)) TryStart(null);
        }

        public void OnUse()
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnUse)) TryStart(null);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnTriggerEnter)) TryStart(other.transform);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnTriggerEnter)) TryStart(other.transform);
        }

        public void OnTriggerExit(Collider other)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnTriggerExit)) TryStart(other.transform);
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnTriggerExit)) TryStart(other.transform);
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnCollisionEnter)) TryStart(collision.collider.transform);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnTriggerEnter)) TryStart(collision.collider.transform);
        }

        public void OnCollisionExit(Collision collision)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnTriggerExit)) TryStart(collision.collider.transform);
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            if (enabled && (trigger == DialogueTriggerEvent.OnTriggerExit)) TryStart(collision.collider.transform);
        }

        public void Start()
        {
            // Waits one frame to allow all other components to finish their Start() methods.
            if (trigger == DialogueTriggerEvent.OnStart) StartCoroutine(StartAfterOneFrame());
        }

        private bool listenForOnDestroy = false;

        public void OnEnable()
        {
            PersistentDataManager.RegisterPersistentData(gameObject);
            listenForOnDestroy = true;
            // Waits one frame to allow all other components to finish their OnEnable() methods.
            if (trigger == DialogueTriggerEvent.OnEnable) StartCoroutine(StartAfterOneFrame());
        }

        public void OnDisable()
        {
            PersistentDataManager.UnregisterPersistentData(gameObject);
            if (listenForOnDestroy && trigger == DialogueTriggerEvent.OnDisable) TryStart(null);
        }

        public void OnLevelWillBeUnloaded()
        {
            listenForOnDestroy = false;
        }

        public void OnApplicationQuit()
        {
            listenForOnDestroy = false;
        }

        public void OnDestroy()
        {
            if (listenForOnDestroy && trigger == DialogueTriggerEvent.OnDestroy) TryStart(null);
        }

        private IEnumerator StartAfterOneFrame()
        {
            yield return null;
            TryStart(null);
        }

        public void TryStart(Transform actor)
        {
            TryStart(actor, actor);
        }

        /// <summary>
        /// Sets the quest status if the condition is true.
        /// </summary>
        public void TryStart(Transform actor, Transform interactor)
        {
            if (tryingToStart) return;
            tryingToStart = true;
            try
            {
                if (((condition == null) || condition.IsTrue(interactor)))
                {
                    Fire(actor);
                }
            }
            finally
            {
                tryingToStart = false;
            }
        }

        public void Fire(Transform actor)
        {
            if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Dialogue System Trigger is firing.", new System.Object[] { DialogueDebug.Prefix }), this);

            // Quest states:
            if (!string.IsNullOrEmpty(questName))
            {
                if (setQuestState) QuestLog.SetQuestState(questName, questState);
                if (setQuestEntryState) QuestLog.SetQuestEntryState(questName, questEntryNumber, questEntryState);
            }

            // Lua:
            if (!string.IsNullOrEmpty(luaCode))
            {
                Lua.Run(luaCode, DialogueDebug.LogInfo);
            }

            // Sequence:
            if (!string.IsNullOrEmpty(sequence))
            {
                DialogueManager.PlaySequence(sequence, Tools.Select(sequenceSpeaker, transform), Tools.Select(sequenceListener, actor));
            }

            // Alert:
            if (!string.IsNullOrEmpty(alertMessage))
            {
                string localizedAlertMessage = alertMessage;
                if ((localizedTextTable != null) && localizedTextTable.ContainsField(alertMessage))
                {
                    localizedAlertMessage = localizedTextTable[alertMessage];
                }
                if (Mathf.Approximately(0, alertDuration))
                {
                    DialogueManager.ShowAlert(localizedAlertMessage);
                }
                else
                {
                    DialogueManager.ShowAlert(localizedAlertMessage, alertDuration);
                }
            }

            // Send Messages:
            foreach (var sma in sendMessages)
            {
                if (sma.gameObject != null && !string.IsNullOrEmpty(sma.message))
                {
                    sma.gameObject.SendMessage(sma.message, sma.parameter, SendMessageOptions.DontRequireReceiver);
                }
            }

            // Bark:
            if (!string.IsNullOrEmpty(barkConversation))
            {
                if (DialogueManager.IsConversationActive && !allowBarksDuringConversations)
                {
                    if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Bark triggered on {1}, but a conversation is already active.", new System.Object[] { DialogueDebug.Prefix, name }), GetBarker());
                }
                else if (cacheBarkLines)
                {
                    BarkCachedLine(GetBarker(), Tools.Select(barkTarget, actor));
                }
                else
                {
                    DialogueManager.Bark(barkConversation, GetBarker(), Tools.Select(barkTarget, actor), barkHistory);
                    sequencer = BarkController.LastSequencer;
                }
            }

            // Conversation:
            if (!string.IsNullOrEmpty(conversation))
            {
                bool skipConversation = skipIfNoValidEntries && !DialogueManager.ConversationHasValidEntry(conversation, Tools.Select(conversationActor, actor), Tools.Select(conversationConversant, this.transform));
                if (exclusive && DialogueManager.IsConversationActive) skipConversation = true;
                if (skipConversation)
                {
                    if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Conversation triggered on {1}, but skipping because no entries are currently valid.", new System.Object[] { DialogueDebug.Prefix, name }));
                }
                else
                {
                    DialogueManager.StartConversation(conversation, Tools.Select(conversationActor, actor), Tools.Select(conversationConversant, this.transform));
                }
            }

            DialogueManager.SendUpdateTracker();

            // Once?
            DestroyIfOnce();
        }

        private Transform GetBarker()
        {
            return Tools.Select(barker, this.transform);
        }

        private string GetBarkerName()
        {
            return OverrideActorName.GetActorName(GetBarker());
        }

        private void BarkCachedLine(Transform speaker, Transform listener)
        {
            if (barkUI == null) barkUI = speaker.GetComponentInChildren(typeof(IBarkUI)) as IBarkUI;
            if (cachedState == null) PopulateCache(speaker, listener);
            BarkNextCachedLine(speaker, listener);
        }

        private void PopulateCache(Transform speaker, Transform listener)
        {
            if (string.IsNullOrEmpty(barkConversation) && DialogueDebug.LogWarnings) Debug.Log(string.Format("{0}: Bark (speaker={1}, listener={2}): conversation title is blank", new System.Object[] { DialogueDebug.Prefix, speaker, listener }), speaker);
            ConversationModel conversationModel = new ConversationModel(DialogueManager.MasterDatabase, barkConversation, speaker, listener, DialogueManager.AllowLuaExceptions, DialogueManager.IsDialogueEntryValid);
            cachedState = conversationModel.FirstState;
            if ((cachedState == null) && DialogueDebug.LogWarnings) Debug.Log(string.Format("{0}: Bark (speaker={1}, listener={2}): '{3}' has no START entry", new System.Object[] { DialogueDebug.Prefix, speaker, listener, barkConversation }), speaker);
            if (!cachedState.HasAnyResponses && DialogueDebug.LogWarnings) Debug.Log(string.Format("{0}: Bark (speaker={1}, listener={2}): '{3}' has no valid bark lines", new System.Object[] { DialogueDebug.Prefix, speaker, listener, barkConversation }), speaker);
        }

        private void BarkNextCachedLine(Transform speaker, Transform listener)
        {
            if ((barkUI != null) && (cachedState != null) && cachedState.HasAnyResponses)
            {
                Response[] responses = cachedState.HasNPCResponse ? cachedState.npcResponses : cachedState.pcResponses;
                int index = (barkHistory ?? new BarkHistory(BarkOrder.Random)).GetNextIndex(responses.Length);
                DialogueEntry barkEntry = responses[index].destinationEntry;
                if ((barkEntry == null) && DialogueDebug.LogWarnings) Debug.Log(string.Format("{0}: Bark (speaker={1}, listener={2}): '{3}' bark entry is null", new System.Object[] { DialogueDebug.Prefix, speaker, listener, conversation }), speaker);
                if (barkEntry != null)
                {
                    Subtitle subtitle = new Subtitle(cachedState.subtitle.listenerInfo, cachedState.subtitle.speakerInfo, new FormattedText(barkEntry.DialogueText), string.Empty, string.Empty, barkEntry);
                    if (DialogueDebug.LogInfo) Debug.Log(string.Format("{0}: Bark (speaker={1}, listener={2}): '{3}'", new System.Object[] { DialogueDebug.Prefix, speaker, listener, subtitle.formattedText.text }), speaker);
                    StartCoroutine(BarkController.Bark(subtitle, speaker, listener, barkUI));
                }
            }
        }

        /// <summary>
        /// Resets the bark history to the beginning of the list of bark lines.
        /// </summary>
        public void ResetBarkHistory()
        {
            barkHistory.Reset();
        }

        /// <summary>
        /// Listens for the OnRecordPersistentData message and records the current bark index.
        /// </summary>
        public void OnRecordPersistentData()
        {
            if (enabled && !string.IsNullOrEmpty(barkConversation))
            {
                DialogueLua.SetActorField(GetBarkerName(), "Bark_Index", barkHistory.index);
            }
        }

        /// <summary>
        /// Listens for the OnApplyPersistentData message and retrieves the current bark index.
        /// </summary>
        public void OnApplyPersistentData()
        {
            if (enabled && !string.IsNullOrEmpty(barkConversation))
            {
                if (barkHistory == null) barkHistory = new BarkHistory(barkOrder);
                barkHistory.index = DialogueLua.GetActorField(GetBarkerName(), "Bark_Index").AsInt;
            }
        }

    }

}
