using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using Invector.vCharacterController;

public class DeadTest : MonoBehaviour
{
    //RagdollUtility ragdollUtility;
    //bool ifDead;
    //Transform characterChest, characterHips;
    //Animator animator;
    // Use this for initialization
    public bool isJump;

    private void Start()
    {
        //ragdollUtility = GetComponent<RagdollUtility>();
        //ifDead = false;
        //characterHips = animator.GetBoneTransform(HumanBodyBones.Hips);
    }

    // Update is called once per frame
    private void Update()
    {
        this.isJump = GetComponent<vThirdPersonMotor>().isJumping;
        //if (Input.GetKeyUp(KeyCode.O))
        //{
        //    if (ifDead == false)
        //    {
        //        Debug.Log("characterHips            "+characterHips);
        //        ragdollUtility.EnableRagdoll();
        //    }
        //    else
        //    {
        //        ragdollUtility.DisableRagdoll();
        //    }
        //    ifDead = !ifDead;
        //}
    }
}