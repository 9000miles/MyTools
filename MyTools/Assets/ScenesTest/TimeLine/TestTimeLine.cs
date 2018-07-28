using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TestTimeLine : MonoBehaviour
{
    private PlayableDirector playableDirector;

    // Use this for initialization
    private void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Debug.Log("duration  " + playableDirector.duration);
        //Debug.Log("initialTime  " + playableDirector.initialTime);
        //Debug.Log("time  " + playableDirector.time);
    }
}