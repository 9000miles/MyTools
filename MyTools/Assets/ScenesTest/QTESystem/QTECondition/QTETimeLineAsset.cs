using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class QTETimeLineAsset : PlayableAsset
{
    public QTEInfo info;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        QTETimeLinePlayable qtePlayable = new QTETimeLinePlayable();
        qtePlayable.playableDirector = owner.GetComponent<PlayableDirector>();
        qtePlayable.qteInfo = info;
        qtePlayable.timeLineAsset = this;
        return ScriptPlayable<QTETimeLinePlayable>.Create(graph, qtePlayable);
    }
}