using PixelCrushers.DialogueSystem.SequencerCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

//public class TimelineSequece : SequencerCommand
//{
//    //是否循环
//    private bool mIsLoop = false;

//    private bool noWait = false;
//    private bool isEnd = false;//播放完整个timeline
//    private PlayableDirector playableDirector = null;
//    private PlayableDirector mLoopPlayable;
//    private List<PlayableDirector> playableLists;

//    private void Start()
//    {
//        var mode = GetParameter(0).ToLower();
//        //不写默认 为Speaker
//        var subject = GetSubject(1, Sequencer.Speaker);
//        var isloop = string.Equals(GetParameter(2), "isloop", System.StringComparison.OrdinalIgnoreCase) ||
//            string.Equals(GetParameter(3), "isloop", System.StringComparison.OrdinalIgnoreCase) ||
//            string.Equals(GetParameter(4), "isLoop", System.StringComparison.OrdinalIgnoreCase);

//        var noWait = string.Equals(GetParameter(2), "nowait", System.StringComparison.OrdinalIgnoreCase) ||
//            string.Equals(GetParameter(3), "nowait", System.StringComparison.OrdinalIgnoreCase) ||
//            string.Equals(GetParameter(4), "nowait", System.StringComparison.Ordinal);

//        var isEnd = string.Equals(GetParameter(2), "isEnd", System.StringComparison.OrdinalIgnoreCase) ||
//            string.Equals(GetParameter(3), "isEnd", System.StringComparison.OrdinalIgnoreCase) ||
//            string.Equals(GetParameter(4), "isEnd", System.StringComparison.OrdinalIgnoreCase);

//        playableDirector = (subject != null) ? subject.GetComponent<PlayableDirector>() : null;
//    }
//}