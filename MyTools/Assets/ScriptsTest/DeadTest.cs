using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class DeadTest : MonoBehaviour
{
    RagdollUtility ragdollUtility;
    bool ifDead;
    // Use this for initialization
    void Start()
    {
        ragdollUtility = GetComponent<RagdollUtility>();
        ifDead = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.O))
        {
            if (ifDead == false)
            {
                ragdollUtility.EnableRagdoll();
            }
            else
            {
                ragdollUtility.DisableRagdoll();
            }
            ifDead = !ifDead;
        }
    }
}
