//using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
}