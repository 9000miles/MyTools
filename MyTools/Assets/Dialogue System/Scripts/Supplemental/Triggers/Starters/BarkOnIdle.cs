using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// The bark on idle component can be used to make an NPC bark on timed intervals.
    /// Barks don't occur while a conversation is active.
    /// </summary>
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER
    [HelpURL("http://pixelcrushers.com/dialogue_system/manual/html/bark_on_idle.html")]
#endif
    [AddComponentMenu("Dialogue System/Trigger/Bark On Idle")]
    public class BarkOnIdle : BarkStarter
    {

        /// <summary>
        /// The minimum seconds between barks.
        /// </summary>
        [Tooltip("Minimum seconds between barks.")]
        public float minSeconds = 5f;

        /// <summary>
        /// The maximum seconds between barks.
        /// </summary>
        [Tooltip("Maximum seconds between barks.")]
        public float maxSeconds = 10f;

        /// <summary>
        /// The target to bark at. Leave unassigned to just bark into the air.
        /// </summary>
        [Tooltip("Target to whom bark is addressed. Leave unassigned to just bark into the air.")]
        public Transform target;

        private bool started = false;

        void Start()
        {
            started = true;
            StartBarkLoop();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartBarkLoop();
        }

        /// <summary>
        /// Starts the bark loop. Normally this is started in the Start() method. If you need to
        /// restart it for some reason, call this method.
        /// </summary>
        public void StartBarkLoop()
        {
            if (!started) return;
            StopAllCoroutines();
            StartCoroutine(BarkLoop());
        }

        private IEnumerator BarkLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minSeconds, maxSeconds));
                if (enabled && (!DialogueManager.IsConversationActive || allowDuringConversations) && !DialogueTime.IsPaused)
                {
                    TryBark(target);
                }
            }
        }

    }

}
