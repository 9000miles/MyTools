//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//namespace MarsPC
//{
//    [CustomEditor(typeof(QTETimeLineAsset))]
//    public class QTETimelineAssetInspector : Editor
//    {
//        //private QTETimeLineAsset qteAsset;
//        private SerializedProperty singleKeyContinue;
//        private SerializedProperty singleKeyPhythm;
//        private SerializedProperty doubleKeyRepeat;
//        private SerializedProperty linearClick;
//        private SerializedProperty linearDirection;
//        private SerializedProperty scrollBarClick;
//        private SerializedProperty powerGauge;
//        private SerializedProperty mouseGestures;
//        private SerializedProperty focusPoint;

//        private void Awake()
//        {
//            qteAsset = target as QTETimeLineAsset;
//        }

//        private void OnEnable()
//        {
//            singleKeyContinue = serializedObject.FindProperty("info.singleKeyContinue");
//            singleKeyPhythm = serializedObject.FindProperty("info.singleKeyPhythm");
//            doubleKeyRepeat = serializedObject.FindProperty("info.doubleKeyRepeat");
//            linearClick = serializedObject.FindProperty("info.linearClick");
//            linearDirection = serializedObject.FindProperty("info.linearDirection");
//            scrollBarClick = serializedObject.FindProperty("info.scrollBarClick");
//            powerGauge = serializedObject.FindProperty("info.powerGauge");
//            mouseGestures = serializedObject.FindProperty("info.mouseGestures");
//            focusPoint = serializedObject.FindProperty("info.focusPoint");
//        }

//        public override void OnInspectorGUI()
//        {
//            serializedObject.Update();
//            qteAsset.info.isAutomaticActive = EditorGUILayout.Toggle("Is Automatic Active", qteAsset.info.isAutomaticActive);
//            qteAsset.info.ID = EditorGUILayout.IntField("ID", qteAsset.info.ID);
//            qteAsset.info.duration = EditorGUILayout.FloatField("Duration", qteAsset.info.duration);
//            qteAsset.info.description = EditorGUILayout.TextField("Description", qteAsset.info.description);

//            qteAsset.info.type = (EQTEType)EditorGUILayout.EnumPopup("Type", qteAsset.info.type);
//            switch (qteAsset.info.type)
//            {
//                case EQTEType.None:
//                    break;

//                case EQTEType.SingleKeyContinue:
//                    EditorGUILayout.PropertyField(singleKeyContinue, true);
//                    break;

//                case EQTEType.SingleKeyPhythm:
//                    EditorGUILayout.PropertyField(singleKeyPhythm, true);
//                    break;

//                case EQTEType.DoubleKeyRepeat:
//                    EditorGUILayout.PropertyField(doubleKeyRepeat, true);
//                    break;

//                case EQTEType.LinearClick:
//                    EditorGUILayout.PropertyField(linearClick, true);
//                    break;

//                case EQTEType.LinearDirection:
//                    EditorGUILayout.PropertyField(linearDirection, true);
//                    break;

//                case EQTEType.ScrollBarClick:
//                    EditorGUILayout.PropertyField(scrollBarClick, true);
//                    break;

//                case EQTEType.PowerGauge:
//                    EditorGUILayout.PropertyField(powerGauge, true);
//                    break;

//                case EQTEType.MouseGestures:
//                    EditorGUILayout.PropertyField(mouseGestures, true);
//                    break;

//                case EQTEType.FocusPoint:
//                    EditorGUILayout.PropertyField(focusPoint, true);
//                    break;

//                default:
//                    break;
//            }
//            serializedObject.ApplyModifiedProperties();
//        }
//    }
//}