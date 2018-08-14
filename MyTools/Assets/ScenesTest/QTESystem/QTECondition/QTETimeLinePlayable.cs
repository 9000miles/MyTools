using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Common;
using System;
using UnityEngine.Timeline;

public class QTETimeLinePlayable : PlayableBehaviour
{
    public PlayableDirector playableDirector;
    public QTEInfo qteInfo;
    private QTEManager qteManager;
    private DialogueSystemController dialogueController;
    private DialogueDatabase database;
    private Conversation conversation;
    private DialogueEntry currentEntry;
    public QTETimeLineAsset timeLineAsset;

    public override void OnGraphStart(Playable playable)
    {
        base.OnGraphStart(playable);
        qteManager = GameObject.FindObjectOfType<QTEManager>();
        dialogueController = GameObject.FindObjectOfType<DialogueSystemController>();
        PlayableTrack playableTrack = playableDirector.playableAsset.outputs as PlayableTrack;
        IEnumerable<TimelineClip> timelineClips = playableTrack.GetClips();
        foreach (var item in timelineClips)
        {
            if (item.GetType() is QTETimeLineAsset)
            {
                //item.duration =
            }
        }
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        qteManager.ManualExcuteQTE(qteInfo);
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        //Debug.Log(qteInfo.result);
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareFrame(playable, info);
        GetDialogueData();
        switch (qteInfo.result)
        {
            case QTEResult.None:
                break;

            case QTEResult.Failure:
                currentEntry.conditionPriority = ConditionPriority.Low;
                break;

            case QTEResult.Succed:
                currentEntry.conditionPriority = ConditionPriority.High;
                break;

            default:
                break;
        }
        //PlayTimeLine();
    }

    private void PlayTimeLine()
    {
        string sequeceStr = currentEntry.Sequence;
        string timelineSequece = Array.Find(sequeceStr.Split(';'), t => t.StartsWith("PlayTimeline"));//PlayTimeline(QTETimelineTest);
        if (timelineSequece == "") return;
        string timelineName = timelineSequece.Substring(timelineSequece.IndexOf('(') + 1).Replace(")", "");
        PlayableDirector director = GameObject.Find(timelineName).GetComponent<PlayableDirector>();
        director.Play();
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
}