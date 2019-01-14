using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TestAsset : PlayableAsset
{
    [SerializeField, TextArea(2, 5)]
    public string text = "";

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        ScriptPlayable<TestBehavior> playable = ScriptPlayable<TestBehavior>.Create(graph);
        TestBehavior clone = playable.GetBehaviour();
        return playable;
    }

    [ContextMenuItem("add testName", "ContextMenuFunc2")]
    public string testName = "";

    //[ContextMenu("Test Function")]
    private void ContextMenuFunc2()
    {
        Debug.Log(888888888888888888);
    }
}