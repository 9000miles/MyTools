using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class DeadTest : MonoBehaviour
{
    RagdollUtility ragdollUtility;
    bool ifDead;
    Transform characterChest, characterHips;
    Animator animator;
    // Use this for initialization
    void Start()
    {
        ragdollUtility = GetComponent<RagdollUtility>();
        ifDead = false;
        characterHips = animator.GetBoneTransform(HumanBodyBones.Hips);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.O))
        {
            if (ifDead == false)
            {
                Debug.Log("characterHips            "+characterHips);
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
