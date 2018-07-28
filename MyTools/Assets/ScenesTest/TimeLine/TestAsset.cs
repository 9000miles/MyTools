using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TestAsset : PlayableAsset
{
    public string text = "";
    private IEnumerable<PlayableBinding> playableBindings;
    private TestBehavior testBehavior;
    private Playable mPlayable;

    public TestBehavior template = new TestBehavior();
    public ExposedReference<Transform> listener;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        playableBindings = outputs;
        //Debug.Log(duration);
        //Debug.Log(owner.name);
        mPlayable = Playable.Create(graph);
        testBehavior = new TestBehavior();

        var playable = ScriptPlayable<TestBehavior>.Create(graph, template);
        TestBehavior clone = playable.GetBehaviour();
        clone.listener = listener.Resolve(graph.GetResolver());
        //TimelineAsset timelineAsset= testBehavior as TimelineAsset;
        return playable;
    }
}