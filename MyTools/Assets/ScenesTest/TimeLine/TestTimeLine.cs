using Invector;
using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TestTimeLine : vObjectDamage
{
    private PlayableDirector playableDirector;
    private Transform Player;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        Player = FindObjectOfType<vThirdPersonController>().transform;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //Debug.Log("duration  " + playableDirector.duration);
        //Debug.Log("initialTime  " + playableDirector.initialTime);
        //Debug.Log("time  " + playableDirector.time);
        if (Input.GetKeyDown(KeyCode.O))
        {
            ApplyDamage(Player.transform, transform.position);
        }
    }
}