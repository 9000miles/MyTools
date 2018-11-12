using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
namespace MarsPC
{
    public class RagdollTest : MonoBehaviour
    {
        RagdollUtility ragdollUtility;
        bool isDead;
        CapsuleCollider _capsuleCollider;
        Rigidbody _rigidbody;

        public Transform root;
        #region public variables

        public bool removePhysicsAfterDie;
        [Tooltip("SHOOTER: Keep false to use detection hit on each children collider, don't forget to change the layer to BodyPart from hips to all childrens. MELEE: Keep true to only hit the main Capsule Collider.")]
        public bool disableColliders = false;
        public AudioSource collisionSource;
        public AudioClip collisionClip;
        [Header("Add Tags for Weapons or Itens here:")]
        public List<string> ignoreTags = new List<string>() { "Weapon", "Ignore Ragdoll", "Untagged" };
        public AnimatorStateInfo stateInfo;

        #endregion

        #region private variables
        Animator animator;
        [HideInInspector]
        public Transform characterChest, characterHips;
        [System.NonSerialized]
        public bool isActive;
        bool inStabilize, updateBehaviour;

        //bool ragdolled
        //{
        //    get
        //    {
        //        return state != RagdollState.animated;
        //    }
        //    set
        //    {
        //        if (value == true)
        //        {
        //            if (state == RagdollState.animated)
        //            {
        //                //Transition from animated to ragdolled
        //                SetKinematic(false); //allow the ragdoll RigidBodies to react to the environment
        //                SetCollider(false);
        //                animator.enabled = false; //disable animation
        //                state = RagdollState.ragdolled;
        //            }
        //        }
        //        else
        //        {
        //            characterHips.parent = hipsParent;
        //            isActive = false;
        //            if (state == RagdollState.ragdolled)
        //            {
        //                SetKinematic(true); //disable gravity etc.
        //                SetCollider(true);
        //                ragdollingEndTime = Time.time; //store the state change time

        //                animator.enabled = true; //enable animation
        //                state = RagdollState.blendToAnim;

        //                //Store the ragdolled position for blending
        //                foreach (BodyPart b in bodyParts)
        //                {
        //                    b.storedRotation = b.transform.rotation;
        //                    b.storedPosition = b.transform.position;
        //                }

        //                //Remember some key positions
        //                ragdolledFeetPosition = 0.5f * (animator.GetBoneTransform(HumanBodyBones.LeftToes).position + animator.GetBoneTransform(HumanBodyBones.RightToes).position);
        //                ragdolledHeadPosition = animator.GetBoneTransform(HumanBodyBones.Head).position;
        //                ragdolledHipPosition = animator.GetBoneTransform(HumanBodyBones.Hips).position;

        //                //Initiate the get up animation
        //                //hip hips forward vector pointing upwards, initiate the get up from back animation
        //                if (animator.GetBoneTransform(HumanBodyBones.Hips).forward.y > 0)
        //                    animator.Play("StandUp@FromBack");
        //                else
        //                    animator.Play("StandUp@FromBelly");
        //            }
        //        }
        //    }
        //}

        //Possible states of the ragdoll
        enum RagdollState
        {
            animated,    //Mecanim is fully in control
            ragdolled,   //Mecanim turned off, physics controls the ragdoll
            blendToAnim  //Mecanim in control, but LateUpdate() is used to partially blend in the last ragdolled pose
        }

        //The current state
        RagdollState state = RagdollState.animated;
        //How long do we blend when transitioning from ragdolled to animated
        float ragdollToMecanimBlendTime = 0.5f;
        float mecanimToGetUpTransitionTime = 0.05f;
        //A helper variable to store the time when we transitioned from ragdolled to blendToAnim state
        float ragdollingEndTime = -100;
        //Additional vectores for storing the pose the ragdoll ended up in.
        Vector3 ragdolledHipPosition, ragdolledHeadPosition, ragdolledFeetPosition;
        //Declare a list of body parts, initialized in Start()
        List<BodyPart> bodyParts = new List<BodyPart>();
        // used to reset parent of hips
        Transform hipsParent;
        //used to controll damage frequency
        bool inApplyDamage;
        private GameObject _ragdollContainer;
        class BodyPart
        {
            public Transform transform;
            public Vector3 storedPosition;
            public Quaternion storedRotation;
        }
        #endregion

        void Start()
        {
            root = transform;
            pelvis = GetComponent<FullBodyBipedIK>().references.pelvis.GetComponent<Rigidbody>();
            ragdollUtility = GetComponent<RagdollUtility>();
            isDead = false;
            animator = GetComponent<Animator>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();

            // store the Animator component

            // find character chest and hips
            characterChest = animator.GetBoneTransform(HumanBodyBones.Chest);
            characterHips = animator.GetBoneTransform(HumanBodyBones.Hips);
            //hipsParent = characterHips.parent;
            // set all RigidBodies to kinematic so that they can be controlled with Mecanim
            // and there will be no glitches when transitioning to a ragdoll
            //SetKinematic(true);
            //SetCollider(true);
            _ragdollContainer = new GameObject("RagdollContainer " + gameObject.name);
            //_ragdollContainer.hideFlags = HideFlags.HideInHierarchy;
            // find all the transforms in the character, assuming that this script is attached to the root
            Component[] components = GetComponentsInChildren(typeof(Transform));

            // for each of the transforms, create a BodyPart instance and store the transform 
            foreach (Component c in components)
            {
                if (!ignoreTags.Contains(c.tag))
                {
                    BodyPart bodyPart = new BodyPart();
                    bodyPart.transform = c as Transform;
                    if (c.GetComponent<Rigidbody>() != null)
                        c.tag = gameObject.tag;
                    bodyParts.Add(bodyPart);
                }
            }
        }
        public Rigidbody pelvis;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.O))
            {
                if (isDead == false)
                {
                    //ragdolled = true;
                    ragdollUtility.EnableRagdoll();
                    //ActivateRagdoll();
                    //_capsuleCollider.isTrigger = true;
                    //_rigidbody.useGravity = false;
                    //_rigidbody.isKinematic = true;
                    //InputManager.Singleton.ActionController<ControllerBase>();
                }
                else
                {
                    Vector3 toPelvis = pelvis.position - root.position;
                    root.position += toPelvis;
                    pelvis.transform.position -= toPelvis;

                    ragdollUtility.DisableRagdoll();


                    //ragdolled = false;
                    //ragdollUtility.DisableRagdoll();
                    //SetKinematic(false);
                    //_rigidbody.useGravity = true;
                    //_rigidbody.isKinematic = false;
                    //_capsuleCollider.isTrigger = false;
                    //InputManager.Singleton.ActionController<ControllerNormal>();
                }
                isDead = !isDead;
            }
        }
        private void FixedUpdate()
        {
            if(isDead )
            {
                Vector3 toPelvis = pelvis.position - root.position;
                //root.position = pelvis.position;
                //pelvis.transform.position -= toPelvis;

            }
        }

        //void LateUpdate()
        //{
        //    if (animator == null) return;
        //    if (!updateBehaviour && animator.updateMode == AnimatorUpdateMode.AnimatePhysics) return;
        //    updateBehaviour = false;
        //    RagdollBehaviour();
        //}

        //void FixedUpdate()
        //{
        //    updateBehaviour = true;
        //    if (!isActive) return;

        //    if (characterHips.parent != _ragdollContainer.transform) characterHips.SetParent(_ragdollContainer.transform);// = null;
        //    if (ragdolled && !inStabilize)
        //    {
        //        ragdolled = false;
        //        StartCoroutine(ResetPlayer(1f));
        //    }
        //    else if (animator != null && !animator.isActiveAndEnabled && ragdolled || (animator == null && ragdolled))
        //        transform.position = characterHips.position;

        //}

        /// <summary>
        /// Reset the inApplyDamage variable. Set to false;
        /// </summary>
        //void ResetDamage()
        //{
        //    inApplyDamage = false;
        //}

        //// active ragdoll - call this method to turn the ragdoll on      
        //public void ActivateRagdoll()
        //{
        //    if (isActive)
        //        return;

        //    inApplyDamage = true;
        //    isActive = true;

        //    if (transform.parent != null && !transform.parent.gameObject.isStatic) transform.parent = null;


        //    // turn ragdoll on
        //    inStabilize = true;
        //    ragdolled = true;

        //    // start to check if the ragdoll is stable
        //    StartCoroutine(RagdollStabilizer(2f));

        //    if (!isDead)
        //        characterHips.SetParent(_ragdollContainer.transform);// = null;
        //    Invoke("ResetDamage", 0.2f);
        //}

        //// ragdoll stabilizer - wait until the ragdoll became stable based on the chest velocity.magnitude
        //IEnumerator RagdollStabilizer(float delay)
        //{

        //    float rdStabilize = Mathf.Infinity;
        //    yield return new WaitForSeconds(delay);
        //    while (rdStabilize > (isDead ? 0.0001f : 0.1f))
        //    {
        //        if (animator != null && !animator.isActiveAndEnabled)
        //        {
        //            rdStabilize = characterChest.GetComponent<Rigidbody>().velocity.magnitude;

        //        }
        //        else
        //            break;
        //        yield return new WaitForEndOfFrame();
        //    }

        //    if (isDead)
        //    {
        //        //Destroy(iChar as Component);
        //        yield return new WaitForEndOfFrame();
        //        DestroyComponents();
        //    }
        //    inStabilize = false;
        //}

        //// reset player - restore control to the character	
        //IEnumerator ResetPlayer(float waitTime)
        //{
        //    yield return new WaitForSeconds(waitTime);
        //    //Debug.Log("Ragdoll OFF");        

        //}

        //// ragdoll blend - code based on the script by Perttu Hämäläinen with modifications to work with this Controller        
        //void RagdollBehaviour()
        //{
        //    if (!isDead) return;

        //    //Blending from ragdoll back to animated
        //    if (state == RagdollState.blendToAnim)
        //    {
        //        if (Time.time <= ragdollingEndTime + mecanimToGetUpTransitionTime)
        //        {
        //            //If we are waiting for Mecanim to start playing the get up animations, update the root of the mecanim
        //            //character to the best match with the ragdoll
        //            Vector3 animatedToRagdolled = ragdolledHipPosition - animator.GetBoneTransform(HumanBodyBones.Hips).position;
        //            Vector3 newRootPosition = transform.position + animatedToRagdolled;

        //            //Now cast a ray from the computed position downwards and find the highest hit that does not belong to the character 
        //            RaycastHit[] hits = Physics.RaycastAll(new Ray(newRootPosition + Vector3.up, Vector3.down));
        //            //newRootPosition.y = 0;

        //            foreach (RaycastHit hit in hits)
        //            {
        //                if (!hit.transform.IsChildOf(transform))
        //                {
        //                    newRootPosition.y = Mathf.Max(newRootPosition.y, hit.point.y);
        //                }
        //            }
        //            transform.position = newRootPosition;

        //            //Get body orientation in ground plane for both the ragdolled pose and the animated get up pose
        //            Vector3 ragdolledDirection = ragdolledHeadPosition - ragdolledFeetPosition;
        //            ragdolledDirection.y = 0;

        //            Vector3 meanFeetPosition = 0.5f * (animator.GetBoneTransform(HumanBodyBones.LeftFoot).position + animator.GetBoneTransform(HumanBodyBones.RightFoot).position);
        //            Vector3 animatedDirection = animator.GetBoneTransform(HumanBodyBones.Head).position - meanFeetPosition;
        //            animatedDirection.y = 0;

        //            //Try to match the rotations. Note that we can only rotate around Y axis, as the animated characted must stay upright,
        //            //hence setting the y components of the vectors to zero. 
        //            transform.rotation *= Quaternion.FromToRotation(animatedDirection.normalized, ragdolledDirection.normalized);
        //        }
        //        //compute the ragdoll blend amount in the range 0...1
        //        float ragdollBlendAmount = 1.0f - (Time.time - ragdollingEndTime - mecanimToGetUpTransitionTime) / ragdollToMecanimBlendTime;
        //        ragdollBlendAmount = Mathf.Clamp01(ragdollBlendAmount);

        //        //In LateUpdate(), Mecanim has already updated the body pose according to the animations. 
        //        //To enable smooth transitioning from a ragdoll to animation, we lerp the position of the hips 
        //        //and slerp all the rotations towards the ones stored when ending the ragdolling
        //        foreach (BodyPart b in bodyParts)
        //        {
        //            if (b.transform != transform)
        //            { //this if is to prevent us from modifying the root of the character, only the actual body parts
        //              //position is only interpolated for the hips
        //                if (b.transform == animator.GetBoneTransform(HumanBodyBones.Hips))
        //                    b.transform.position = Vector3.Lerp(b.transform.position, b.storedPosition, ragdollBlendAmount);
        //                //rotation is interpolated for all body parts
        //                b.transform.rotation = Quaternion.Slerp(b.transform.rotation, b.storedRotation, ragdollBlendAmount);
        //            }
        //        }

        //        //if the ragdoll blend amount has decreased to zero, move to animated state
        //        if (ragdollBlendAmount == 0)
        //        {
        //            state = RagdollState.animated;
        //            return;
        //        }
        //    }
        //}

        //// set all rigidbodies to kinematic
        //void SetKinematic(bool newValue)
        //{
        //    var _hips = characterHips.GetComponent<Rigidbody>();
        //    _hips.isKinematic = newValue;
        //    Component[] components = _hips.transform.GetComponentsInChildren(typeof(Rigidbody));

        //    foreach (Component c in components)
        //    {
        //        if (!ignoreTags.Contains(c.transform.tag))
        //            (c as Rigidbody).isKinematic = newValue;
        //    }
        //}

        //// set all colliders to trigger
        //void SetCollider(bool newValue)
        //{
        //    if (!disableColliders) return;

        //    var _hips = characterHips.GetComponent<Collider>();
        //    _hips.enabled = !newValue;
        //    Component[] components = _hips.transform.GetComponentsInChildren(typeof(Collider));

        //    foreach (Component c in components)
        //    {
        //        if (!ignoreTags.Contains(c.transform.tag))
        //            if (!c.transform.Equals(transform)) (c as Collider).enabled = !newValue;
        //    }
        //}

        //// destroy the components if the character is dead
        //void DestroyComponents()
        //{
        //    if (removePhysicsAfterDie)
        //    {
        //        var comps = GetComponentsInChildren<MonoBehaviour>();
        //        for (int i = 0; i < comps.Length; i++)
        //        {
        //            if (comps[i].transform != transform)
        //                Destroy(comps[i]);
        //        }
        //        var joints = GetComponentsInChildren<CharacterJoint>();
        //        if (joints != null)
        //        {
        //            foreach (CharacterJoint comp in joints)
        //                if (!ignoreTags.Contains(comp.gameObject.tag) && comp.transform != transform)
        //                    Destroy(comp);
        //        }

        //        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        //        if (rigidbodies != null)
        //        {
        //            foreach (Rigidbody comp in rigidbodies)
        //                if (!ignoreTags.Contains(comp.gameObject.tag) && comp.transform != transform)
        //                    Destroy(comp);
        //        }

        //        var colliders = GetComponentsInChildren<Collider>();
        //        if (colliders != null)
        //        {
        //            foreach (Collider comp in colliders)
        //                if (!ignoreTags.Contains(comp.gameObject.tag) && comp.transform != transform)
        //                    Destroy(comp);
        //        }
        //    }
        //}
    }
}