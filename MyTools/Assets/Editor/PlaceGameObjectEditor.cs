using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MarsPC
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PlaceGameObject))]
    public class PlaceGameObjectEditor : Editor
    {
        private PlaceGameObject mBehaviour { get { return (target as PlaceGameObject); } }
        private Transform mTF { get { return mBehaviour.transform; } }
        private Collider mCollider { get { return mTF.GetComponent<Collider>(); } }
        private float groundClearance { get { return (mBehaviour.groundClearance != 0 || mCollider == null) ? mBehaviour.groundClearance : mCollider.bounds.size.y / 2f; } }

        private void OnSceneGUI()
        {
            Event e = Event.current;
            if ((e.isMouse && e.button == 1 && e.type == EventType.MouseDown) && e.control)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                int layer = 1 << LayerMask.NameToLayer("Triggers");
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, int.MaxValue, ~layer, QueryTriggerInteraction.Ignore))
                {
                    mTF.position = hit.point + (mBehaviour.isNormalDirection ? hit.normal * groundClearance : new Vector3(0, groundClearance, 0));
                }
            }

            if (e.isKey && e.character == 'w' && e.clickCount == 0)
            {
                MyEditorTools.SelectUpGameObject();
            }

            if (e.isKey && e.character == 's' && e.clickCount == 0)
            {
                MyEditorTools.SelectDownGameObject();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox("按 W 或 S 键可在同父级子物体中选择下个物体，按 CTRL 和 鼠标右键 将该选中的物体标记到鼠标位置，" +
                                    "Ground Clearance为离地间隙，如果未设置则自动获取该物体身上的Collider组件，" +
                                    "根据Collider组件的大小设置离地间隙", MessageType.Info, true);
        }
    }
}