using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using Common;
using UnityEngine.Playables;

namespace MarsPC
{
    public class DialogueManager : MonoBehaviour
    {
        private DialogueSystemController dialogueController;
        private DialogueDatabase database;
        private Conversation conversation;
        private DialogueEntry currentEntry;
        private DialogueEntry lastEntry;
        public ConversationTrigger trigger;
        private Link nextLink;

        private void Start()
        {
            dialogueController = GetComponent<DialogueSystemController>();
        }

        private void GetDialogueData()
        {
            //Transform conversantTF = dialogueController.CurrentConversant;
            //if (conversantTF == null) return;
            //ConversationTrigger trigger = trigger.GetComponent<ConversationTrigger>();
            database = dialogueController.initialDatabase;
            conversation = database.GetConversation(trigger.conversation);
            foreach (var item in conversation.dialogueEntries)
            {
                item.onExecute.AddListener(() =>
                {
                    currentEntry = database.GetDialogueEntry(conversation.id, item.id);
                    if (lastEntry != currentEntry && nextLink != null)
                    {
                        nextLink.priority = ConditionPriority.Normal;
                        nextLink = null;
                    }
                    GotoNextDialogueEntry(EQTEResult.Succed);
                    lastEntry = currentEntry;
                });
            }
        }

        private void GotoNextDialogueEntry(EQTEResult result)
        {
            if (currentEntry == null) return;
            List<Link> links = currentEntry.outgoingLinks;
            DialogueEntry[] entries = links.ToArray().Select(t => database.GetDialogueEntry(t));
            DialogueEntry nextEntry = null;
            for (int i = 0; i < entries.Length; i++)
            {
                Field qteResultField = entries[i].fields.Find(t => t.title == "EQTEResult" && t.value == result.ToString());
                if (qteResultField != null)
                {
                    nextEntry = entries[i];
                }
            }
            if (nextEntry != null)
            {
                nextLink = links.Find(t => t.destinationDialogueID == nextEntry.id);
                nextLink.priority = ConditionPriority.High;
            }
            //切换完毕之后需要恢复为原状
        }

        private void Update()
        {
            GetDialogueData();
            //GotoNextDialogueEntry(EQTEResult.Succed);
            //PlayTimeLine();
        }

        private void PlayTimeLine()
        {
            //if (currentEntry == null) return;
            //string sequeceStr = currentEntry.Sequence;
            //string timelineSequece = sequeceStr.Split(';').Find(t => t.StartsWith("PlayTimeline"));//PlayTimeline(QTETimelineTest);
            //if (timelineSequece == "") return;
            //string timelineName = timelineSequece.Substring(timelineSequece.IndexOf('(') + 1).Replace(")", "");
            //PlayableDirector director = GameObject.Find(timelineName).GetComponent<PlayableDirector>();
            //director.Play();
        }
    }
}