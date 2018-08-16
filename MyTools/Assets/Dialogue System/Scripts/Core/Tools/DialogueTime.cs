using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// A static wrapper class for the built-in Time class. This class allows the user to specify
    /// whether the dialogue system functions in realtime, gameplay time, or a custom time. If the 
    /// game is paused during conversations by setting <c>Time.timeScale = 0</c>, then the Dialogue 
    /// System should use realtime or it will also be paused. However, if you want the Dialogue 
    /// System to observe the timeScale, then you can use gameplay time (for example, if you want 
    /// the Sequencer to observe timeScale). If you want to manage time on your own, set the mode
    /// to Custom and manually set DialogueTime.time every frame.
    /// </summary>
    public static class DialogueTime
    {

        /// <summary>
        /// Dialogue System time mode.
        /// </summary>
        public enum TimeMode
        {

            /// <summary>
            /// Ignore Time.timeScale. Internally, use Time.realtimeSinceStartup.
            /// </summary>
            Realtime,

            /// <summary>
            /// Observe Time.timeScale. Internally, use Time.time.
            /// </summary>
            Gameplay,

            /// <summary>
            /// Your code must manually manage the time.
            /// </summary>
            Custom
        }

        /// <summary>
        /// Gets or sets the time mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public static TimeMode Mode { get; set; }

        /// <summary>
        /// Gets the time based on the current Mode.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public static float time
        {
            get
            {
                switch (Mode)
                {
                    default:
                    case TimeMode.Realtime:
                        return (m_isPaused ? realtimeWhenPaused : Time.realtimeSinceStartup) - totalRealtimePaused;
                    case TimeMode.Gameplay:
                        return Time.time;
                    case TimeMode.Custom:
                        return m_customTime;
                }
            }
            set
            {
                m_customTime = value;
            }
        }

        public static bool IsPaused
        {
            get
            {
                switch (Mode)
                {
                    default:
                    case TimeMode.Realtime:
                    case TimeMode.Custom:
                        return m_isPaused;
                    case TimeMode.Gameplay:
                        return Tools.ApproximatelyZero(Time.timeScale);
                }
            }
            set
            {
                switch (Mode)
                {
                    case TimeMode.Realtime:
                        if (!m_isPaused && value)
                        {
                            // Pausing, so record realtime at time of pause:
                            realtimeWhenPaused = Time.realtimeSinceStartup;
                        }
                        else if (m_isPaused && !value)
                        {
                            // Unpausing, so add to totalRealtimePaused:
                            totalRealtimePaused += Time.realtimeSinceStartup - realtimeWhenPaused;
                        }
                        break;
                    case TimeMode.Gameplay:
                        Time.timeScale = m_isPaused ? 0 : 1;
                        break;
                    case TimeMode.Custom:
                        break;
                }
                m_isPaused = value;
            }
        }

        /// <summary>
        /// This version of WaitForSeconds respects DialogueTime.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static IEnumerator WaitForSeconds(float seconds)
        {
            float start = DialogueTime.time;
            while (DialogueTime.time < start + seconds)
            {
                yield return null;
            }
        }

        private static float m_customTime = 0;

        private static bool m_isPaused = false;

        private static float realtimeWhenPaused = 0;

        private static float totalRealtimePaused = 0;

        /// <summary>
        /// Initializes the <see cref="PixelCrushers.DialogueSystem.DialogueTime"/> class.
        /// </summary>
        static DialogueTime()
        {
            Mode = TimeMode.Realtime;
        }

    }

}