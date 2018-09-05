using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// The persistent active data component works with the PersistentDataManager to set a target 
    /// game object active or inactive when loading a game (or when applying persistent data
    /// between level changes).
    /// </summary>
    /// <remarks>
    /// Inactive game objects don't receive messages. Don't add this component to an inactive game 
    /// object. Instead, add it to a "manager" object and set the target to the object that you 
    /// want to activate or deactivate.
    /// </remarks>
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER
    [HelpURL("http://pixelcrushers.com/dialogue_system/manual/html/persistent_active_data.html")]
#endif
    [AddComponentMenu("Dialogue System/Save System/Persistent Active Data")]
    public class PersistentActiveData : MonoBehaviour
    {

        /// <summary>
        /// The target game object.
        /// </summary>
        [Tooltip("The GameObject to set active or inactive based on the Condition below.")]
        public GameObject target;

        /// <summary>
        /// If this condition is <c>true</c>, the target game object is activated; otherwise it's deactivated.
        /// </summary>
        [Tooltip("If true, Target is activated; otherwise deactivated.")]
        public Condition condition;

        /// <summary>
        /// When the script starts, check the condition and set the target GameObject active/inactive.
        /// </summary>
        [Tooltip("When script starts, check condition & set target GameObject active/inactive.")]
        public bool checkOnStart;

        protected virtual void Start()
        {
            if (checkOnStart) Check();
        }

        protected virtual void OnEnable()
        {
            PersistentDataManager.RegisterPersistentData(gameObject);
        }

        protected virtual void OnDisable()
        {
            PersistentDataManager.UnregisterPersistentData(gameObject);
        }

        /// <summary>
        /// Listens for an OnApplyPersistentData message from the PersistentDataManager, and sets a target
        /// game object accordingly.
        /// </summary>
        public void OnApplyPersistentData()
        {
            Check();
        }

        public virtual void Check()
        { 
            if (enabled) target.SetActive(condition.IsTrue(null));
        }

    }

}