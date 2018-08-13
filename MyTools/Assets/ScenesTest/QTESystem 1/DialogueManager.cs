using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using Common;
using UnityEngine.Playables;

public class DialogueManager : MonoBehaviour
{
    private DialogueSystemController dialogueController;
    private DialogueDatabase database;
    private Conversation conversation;
    private DialogueEntry currentEntry;

    private void Start()
    {
        dialogueController = GetComponent<DialogueSystemController>();
        GetDialogueData();
    }

    private void GetDialogueData()
    {
        Transform conversantTF = dialogueController.CurrentConversant;
        if (conversantTF == null) return;
        ConversationTrigger trigger = conversantTF.GetComponent<ConversationTrigger>();
        database = dialogueController.initialDatabase;
        conversation = database.GetConversation(trigger.conversation);
        foreach (var item in conversation.dialogueEntries)
        {
            item.onExecute.AddListener(() =>
            {
                currentEntry = database.GetDialogueEntry(conversation.id, item.id);
            });
        }
    }

    private void Update()
    {
        //PlayTimeLine();
    }

    private void PlayTimeLine()
    {
        if (currentEntry == null) return;
        string sequeceStr = currentEntry.Sequence;
        string timelineSequece = sequeceStr.Split(';').Find(t => t.StartsWith("PlayTimeline"));//PlayTimeline(QTETimelineTest);
        if (timelineSequece == "") return;
        string timelineName = timelineSequece.Substring(timelineSequece.IndexOf('(') + 1).Replace(")", "");
        PlayableDirector director = GameObject.Find(timelineName).GetComponent<PlayableDirector>();
        director.Play();
    }
}