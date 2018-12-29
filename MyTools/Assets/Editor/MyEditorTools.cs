using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MarsPC
{
    public class MyEditorTools
    {
        public static float deltaVaule = 0.2f;

        [MenuItem("MyTools/Get Object Count")]
        private static void GetObjectCount()
        {
            GameObject[] select = Selection.gameObjects;
            int count = 0;
            foreach (var item in select)
            {
                count += item.GetComponentsInChildren<Transform>().Length;
                //count += GetObjcetCount(item);
            }
            Debug.Log(string.Format("-------选中{0}个物体，共有{1}个物体-------", select.Length, count));
        }

        #region 更改Raycast Target属性功能

        [MenuItem("MyTools/Change Selection Raycast Target", true, 1)]
        [MenuItem("MyTools/Change Selection Raycast Target On", true, 2)]
        [MenuItem("MyTools/Change Selection Raycast Target Off", true, 3)]
        private static bool IsSelection()
        {
            return Selection.gameObjects.Length > 0;
        }

        /// <summary>
        /// 反转所有的RaycastTarget
        /// </summary>
        [MenuItem("MyTools/Change Selection Raycast Target", false, 1)]
        private static void ChangeRaycastTargetValue()
        {
            GameObject[] gos = Selection.gameObjects;
            for (int i = 0; i < gos.Length; i++)
                gos[i].GetComponent<Graphic>().raycastTarget = !gos[i].GetComponent<Graphic>().raycastTarget;
            Debug.Log("Reverse RaycastTarget Value Done");
        }

        /// <summary>
        /// 打开所有的RaycastTarget
        /// </summary>
        [MenuItem("MyTools/Change Selection Raycast Target On", false, 2)]
        private static void ChangeRaycastTargetValueOn()
        {
            GameObject[] gos = Selection.gameObjects;
            for (int i = 0; i < gos.Length; i++)
                ChangeChildRaycastTargetValue(gos[i], true);
            Debug.Log("On GameObjects RaycastTarget Value Done");
        }

        /// <summary>
        /// 关闭所有的RaycastTarget
        /// </summary>
        [MenuItem("MyTools/Change Selection Raycast Target Off", false, 3)]
        private static void ChangeRaycastTargetValueOff()
        {
            GameObject[] gos = Selection.gameObjects;
            for (int i = 0; i < gos.Length; i++)
                ChangeChildRaycastTargetValue(gos[i], false);
            Debug.Log("Off GameObjects RaycastTarget Value Done");
        }

        /// <summary>
        /// 改变子级物体的RaycastTarget的值
        /// </summary>
        /// <param name="go">物体</param>
        /// <param name="isOn">是否打开</param>
        private static void ChangeChildRaycastTargetValue(GameObject go, bool isOn)
        {
            if (go.GetComponent<Graphic>() != null)
            {
                go.GetComponent<Graphic>().raycastTarget = isOn;//设置当前物体

                for (int i = 0; i < go.transform.childCount; i++)//递归子级物体
                    ChangeChildRaycastTargetValue(go.transform.GetChild(i).gameObject, isOn);
            }
        }

        #endregion 更改Raycast Target属性功能

        #region 打开功能窗口

        /// <summary>
        /// 手动递增改名
        /// </summary>
        [MenuItem("MyTools/Change Name [W]")]
        private static void ChangeNameOneByOne()
        {
            ChangeNameWindow window = new ChangeNameWindow();
            window.Show();
        }

        /// <summary>
        /// 排列物体
        /// </summary>
        [MenuItem("MyTools/Placing GameObject [W]")]
        private static void PlacingObject()
        {
            PlacingObjectWindow window = new PlacingObjectWindow();
            window.Show();
        }

        /// <summary>
        /// 打开选择物体窗口
        /// </summary>
        [MenuItem("MyTools/Open Selete History [W]")]
        private static void OpenSelectionWindow()
        {
            SelectedGameObjectWindow window = new SelectedGameObjectWindow();
            window.Show();
        }

        //查找指定物体 是否在选中的物体中，如果在则选中此物体（如果有多个物体，则全部展开），否则提示没有。
        /// <summary>
        /// 查找指定物体，是否在选中的物体中
        /// </summary>
        [MenuItem("MyTools/Find Object [W]")]
        private static void FindObject()
        {
            FindObjectWindow window = new FindObjectWindow();
            window.Show();
        }

        //[MenuItem("MyTools/Change Delta Vaule [W]")]
        //private static void ChangeDeltaVaule()
        //{
        //    //ChangeDeltaVaule window = new ChangeDeltaVaule();
        //    //window.Show();
        //}

        #endregion 打开功能窗口

        /// <summary>
        /// 获取选中的物体数量
        /// </summary>

        #region 常用快捷键

        /// <summary>
        /// 改变物体激活状态
        /// </summary>
        [MenuItem("MyTools/Change GameObject Active State %w")]
        private static void ToggleActiveState()
        {
            GameObject[] select = Selection.gameObjects;
            foreach (GameObject item in select)
            {
                item.SetActive(!item.activeSelf);
            }
        }

        private static int GetObjcetCount(GameObject go)
        {
            int count = 0;
            if (go.transform.childCount > 0)
            {
                Transform[] tfs = go.transform.GetComponentsInChildren<Transform>();
                foreach (var item in tfs)
                {
                    if (item != go.transform)
                        count += GetObjcetCount(item.gameObject);
                }
            }
            else
                count = 1;
            return count;
        }

        /// <summary>
        /// 选中Player
        /// </summary>
        [MenuItem("MyTools/Selected Player &Q")]
        private static void SelectedPlayer()
        {
            GameObject player;
            player = GameObject.Find("Player");
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            if (player != null)
            {
                Selection.activeGameObject = player;
            }
        }

        #endregion 常用快捷键

        #region 上下移动物体在Hierarchy面板中的顺序

        /// <summary>
        /// Alt+Up向上移动GameObject的层级
        /// </summary>
        [MenuItem("MyTools/Move Up GameObject In Hierarchy &UP")]
        private static void MoveUpGameObjectInHierarchy()
        {
            GameObject[] gos = Selection.gameObjects;
            gos.OrderAscending((t) => t.transform.GetSiblingIndex());
            foreach (var item in gos)
            {
                item.transform.SetSiblingIndex(item.transform.GetSiblingIndex() - 1);
            }
        }

        /// <summary>
        /// Alt+Down向下移动GameObject的层级
        /// </summary>
        [MenuItem("MyTools/Move Down GameObject In Hierarchy &DOWN")]
        private static void MoveDownGameObjectInTheHierarchy()
        {
            GameObject[] gos = Selection.gameObjects;
            gos.OrderDescending((t) => t.transform.GetSiblingIndex());
            foreach (var item in gos)
            {
                item.transform.SetSiblingIndex(item.transform.GetSiblingIndex() + 1);
            }
        }

        #endregion 上下移动物体在Hierarchy面板中的顺序

        #region 在同级中选择上下物体快捷键

        [MenuItem("MyTools/Select Up GameObject %UP")]
        public static void SelectUpGameObject()
        {
            GameObject go = Selection.activeGameObject;
            int index = go.transform.GetSiblingIndex();
            if (index > 0)
                Selection.activeGameObject = go.transform.parent.GetChild(index - 1).gameObject;
        }

        [MenuItem("MyTools/Select Down GameObject %DOWN")]
        public static void SelectDownGameObject()
        {
            GameObject go = Selection.activeGameObject;
            int index = go.transform.GetSiblingIndex();
            if (index < go.transform.parent.childCount - 1)
                Selection.activeGameObject = go.transform.parent.GetChild(index + 1).gameObject;
        }

        #endregion 在同级中选择上下物体快捷键

        #region 微移物体快捷键

        [MenuItem("MyTools/Up Movement %&UP")]
        private static void SmallChangeTransformUp()
        {
            GameObject go = Selection.activeGameObject;
            Vector3 pos = go.transform.localPosition;
            pos.y += deltaVaule;
            go.transform.localPosition = pos;
        }

        [MenuItem("MyTools/Down Movement %&DOWN")]
        private static void SmallChangeTransformDown()
        {
            GameObject go = Selection.activeGameObject;
            Vector3 pos = go.transform.localPosition;
            pos.y -= deltaVaule;
            go.transform.localPosition = pos;
        }

        [MenuItem("MyTools/Left Movement %&LEFT")]
        private static void SmallChangeTransformLeft()
        {
            GameObject go = Selection.activeGameObject;
            Vector3 pos = go.transform.localPosition;
            pos.x += deltaVaule;
            go.transform.localPosition = pos;
        }

        [MenuItem("MyTools/Right Movement %&RIGHT")]
        private static void SmallChangeTransformRight()
        {
            GameObject go = Selection.activeGameObject;
            Vector3 pos = go.transform.localPosition;
            pos.x -= deltaVaule;
            go.transform.localPosition = pos;
        }

        [MenuItem("MyTools/Front Movement %&#UP")]
        private static void SmallChangeTransformLeftFront()
        {
            GameObject go = Selection.activeGameObject;
            Vector3 pos = go.transform.localPosition;
            pos.z -= deltaVaule;
            go.transform.localPosition = pos;
        }

        [MenuItem("MyTools/Back Movement %&#DOWN")]
        private static void SmallChangeTransformBack()
        {
            GameObject go = Selection.activeGameObject;
            Vector3 pos = go.transform.localPosition;
            pos.z += deltaVaule;
            go.transform.localPosition = pos;
        }

        #endregion 微移物体快捷键

        #region TransformHelper

        [MenuItem("CONTEXT/Transform/Copy LocalPosition", false, 200)]
        private static void CopyLocalPosition(MenuCommand command)
        {
            Transform tf = (Transform)command.context;
            string str = "(" + tf.localPosition.x.ToString() + ", " + tf.localPosition.y.ToString() + ", " + tf.localPosition.z.ToString() + ")";
            EditorGUIUtility.systemCopyBuffer = str;
        }

        [MenuItem("CONTEXT/Transform/Copy LocalRotation", false, 201)]
        private static void CopyLocalRotation(MenuCommand command)
        {
            Transform tf = (Transform)command.context;
            string str = "(" + tf.localRotation.eulerAngles.x.ToString() + ", " + tf.localRotation.eulerAngles.y.ToString() + ", " + tf.localRotation.eulerAngles.z.ToString() + ")";
            EditorGUIUtility.systemCopyBuffer = str;
        }

        [MenuItem("CONTEXT/Transform/Copy LocalScale", false, 202)]
        private static void CopyLocalScale(MenuCommand command)
        {
            Transform tf = (Transform)command.context;
            string str = "(" + tf.localScale.x.ToString() + ", " + tf.localScale.y.ToString() + ", " + tf.localScale.z.ToString() + ")";
            EditorGUIUtility.systemCopyBuffer = str;
        }

        [MenuItem("CONTEXT/Transform/Paste LocalPosition", false, 250)]
        private static void PasteLocalPosition(MenuCommand command)
        {
            Transform tf = (Transform)command.context;
            Vector3 vector = tf.ConverVector3(EditorGUIUtility.systemCopyBuffer);
            if (vector != Vector3.zero)
            {
                Undo.RecordObject(tf, "Paste LocalPosition");
                tf.localPosition = vector;
            }
        }

        [MenuItem("CONTEXT/Transform/Paste LocalRotation", false, 251)]
        private static void PasteLocalRotation(MenuCommand command)
        {
            Transform tf = (Transform)command.context;
            Vector3 vector = tf.ConverVector3(EditorGUIUtility.systemCopyBuffer);
            if (vector != Vector3.zero)
            {
                Undo.RecordObject(tf, "PasteLocalRotation");
                tf.localRotation = Quaternion.Euler(vector);
            }
        }

        [MenuItem("CONTEXT/Transform/Paste LocalScale", false, 252)]
        private static void PasteLocalScale(MenuCommand command)
        {
            Transform tf = (Transform)command.context;
            Vector3 vector = tf.ConverVector3(EditorGUIUtility.systemCopyBuffer);
            if (vector != Vector3.zero)
            {
                Undo.RecordObject(tf, "Paste LocalScale");
                tf.localScale = vector;
            }
        }

        #endregion TransformHelper

        //#region 测试方法

        ///// <summary>
        ///// 测试方法
        ///// </summary>
        //[MenuItem("MyTools/WalkPosturePathManager Create Function")]
        //private static void WalkPosturePathManagerTestFunction()
        //{
        //    AutoWalkManager manager = new AutoWalkManager();
        //    manager.FindAllPath();
        //    manager.CreateAllPathPoint();
        //}

        ///// <summary>
        ///// 测试方法
        ///// </summary>
        //[MenuItem("MyTools/WalkPosturePathManager Clear Function")]
        //private static void WalkPosturePathManagerClearFunction()
        //{
        //    List<Transform> pathList = new List<Transform>();
        //    Transform mainManager = GameObject.Find("WalkPostureManager").transform;
        //    if (mainManager == null) return;

        //    foreach (var item in mainManager.transform.GetComponentsInChildren<Transform>())
        //    {
        //        if (item != mainManager && item.parent != mainManager)
        //        {
        //            pathList.Add(item);
        //        }
        //    }

        //    GameObjectHelper.DestroyChildren(pathList.ToArray());
        //}

        //#endregion 测试方法
    }
}