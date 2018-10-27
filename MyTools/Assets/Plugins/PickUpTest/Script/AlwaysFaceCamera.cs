using UnityEngine;
    /// <summary>
    /// Component that keeps its game object always facing the main camera.
    /// </summary>
    [AddComponentMenu("Dialogue System/Actor/Always Face Camera")]
    public class AlwaysFaceCamera : MonoBehaviour
    {
        public bool yAxisOnly = false;

        private Transform myTransform = null;

        private void Awake()
        {
            myTransform = transform;
        }

        private void Update()
        {
            if ((myTransform != null) && (UnityEngine.Camera.main != null))
            {
                if (yAxisOnly)
                {
                    myTransform.LookAt(new Vector3(UnityEngine.Camera.main.transform.position.x, myTransform.position.y, UnityEngine.Camera.main.transform.position.z));
                }
                else
                {
                    myTransform.LookAt(UnityEngine.Camera.main.transform);
                }
            }
        }
    }
