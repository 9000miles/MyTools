using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TestBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;

    public override void OnBehaviourDelay(Playable playable, FrameData info)
    {
        base.OnBehaviourDelay(playable, info);
        Debug.Log("OnBehaviourDelay");
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)//每个片段结束播放，就会调用该函数
    {
        base.OnBehaviourPause(playable, info);
        //Debug.Log("OnBehaviourPause");
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)//每个片段开始播放，就会调用该函数
    {
        base.OnBehaviourPlay(playable, info);
        playableDirector = GameObject.Find("TimeLine").GetComponent<PlayableDirector>();
        //Debug.Log("OnBehaviourPlay");
    }

    public override void OnGraphStart(Playable playable)//TimeLine编辑之后，会调用
    {
        base.OnGraphStart(playable);
        //Debug.Log("OnGraphStart");
    }

    public override void OnGraphStop(Playable playable)//TimeLine编辑之后，会调用
    {
        base.OnGraphStop(playable);
        //Debug.Log("OnGraphStop");
    }

    public override void PrepareData(Playable playable, FrameData info)
    {
        base.PrepareData(playable, info);
        Debug.Log("PrepareData");
    }

    public override void PrepareFrame(Playable playable, FrameData info)//每帧调用
    {
        base.PrepareFrame(playable, info);
        //Debug.Log("PrepareFrame" + "            " + playableDirector.time);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)//每帧调用
    {
        base.ProcessFrame(playable, info, playerData);
        //Debug.Log("ProcessFrame" + "            " + playableDirector.time);
    }
}