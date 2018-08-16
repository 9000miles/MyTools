using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This component allows you specify a camera angle to use when a Camera()
    /// sequencer command uses the reserved keyword 'angle'.
    /// </summary>
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER
    [HelpURL("http://pixelcrushers.com/dialogue_system/manual/html/default_camera_angle.html")]
#endif
    [AddComponentMenu("Dialogue System/Actor/Default Camera Angle")]
    public class DefaultCameraAngle : MonoBehaviour
    {

        /// <summary>
        /// The default camera angle for the object.
        /// </summary>
        public string cameraAngle = "Closeup";

    }

}
