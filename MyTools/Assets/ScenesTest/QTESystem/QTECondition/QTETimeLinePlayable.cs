//using PixelCrushers.DialogueSystem;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Playables;
//using Common;
//using UnityEngine.Timeline;

//namespace MarsPC
//{
//    public class QTETimeLinePlayable : PlayableBehaviour
//    {
//        public PlayableDirector playableDirector;
//        public QTEInfo qteInfo;
//        private QTEManager qteManager;
//        private DialogueSystemController dialogueController;
//        private DialogueDatabase database;
//        private Conversation conversation;
//        private DialogueEntry currentEntry;
//        private List<TimelineClip> qteAssetList = new List<TimelineClip>();

//        public override void OnGraphStart(Playable playable)
//        {
//            base.OnGraphStart(playable);
//            qteManager = GameObject.FindObjectOfType<QTEManager>();
//            dialogueController = GameObject.FindObjectOfType<DialogueSystemController>();

//            TimelineAsset timelineAsset = playableDirector.playableAsset as TimelineAsset;
//            IEnumerable<TrackAsset> putTracks = timelineAsset.GetOutputTracks();
//            foreach (var item in putTracks)
//            {
//                IEnumerator clipsEnumerator = item.GetClips().GetEnumerator();
//                while (clipsEnumerator.MoveNext())
//                {
//                    TimelineClip qteClip = clipsEnumerator.Current as TimelineClip;
//                    if (qteClip.asset as QTETimeLineAsset)
//                    {
//                        qteClip.duration = (qteClip.asset as QTETimeLineAsset).info.duration;
//                    }
//                }
//            }
//        }

//        public override void OnBehaviourPlay(Playable playable, FrameData info)
//        {
//            base.OnBehaviourPlay(playable, info);
//            //qteManager.ManualExcuteQTE(qteInfo);
//        }

//        public override void OnBehaviourPause(Playable playable, FrameData info)
//        {
//            base.OnBehaviourPause(playable, info);
//        }

//        public override void PrepareFrame(Playable playable, FrameData info)
//        {
//            base.PrepareFrame(playable, info);
//            GetDialogueData();
//            switch (qteInfo.result)
//            {
//                case EQTEResult.None:
//                    break;

//                case EQTEResult.Failure:
//                    currentEntry.conditionPriority = ConditionPriority.Low;
//                    break;

//                case EQTEResult.Succed:
//                    currentEntry.conditionPriority = ConditionPriority.High;
//                    break;

//                default:
//                    break;
//            }
//            //PlayTimeLine();
//        }

//        private void PlayTimeLine()
//        {
//            //string sequeceStr = currentEntry.Sequence;
//            //string timelineSequece = sequeceStr.Split(';').Find(t => t.StartsWith("PlayTimeline"));//PlayTimeline(QTETimelineTest);
//            //if (timelineSequece == "") return;
//            //string timelineName = timelineSequece.Substring(timelineSequece.IndexOf('(') + 1).Replace(")", "");
//            //PlayableDirector director = GameObject.Find(timelineName).GetComponent<PlayableDirector>();
//            //director.Play();
//        }

//        private void GetDialogueData()
//        {
//            Transform conversantTF = dialogueController.CurrentConversant;
//            if (conversantTF == null) return;
//            ConversationTrigger trigger = conversantTF.GetComponent<ConversationTrigger>();
//            database = dialogueController.initialDatabase;
//            conversation = database.GetConversation(trigger.conversation);
//            foreach (var item in conversation.dialogueEntries)
//            {
//                item.onExecute.AddListener(() =>
//                {
//                    currentEntry = database.GetDialogueEntry(conversation.id, item.id);
//                });
//            }
//        }
//    }
//}