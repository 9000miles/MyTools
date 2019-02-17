using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TransformPositionHelper))]
public class AnimatorIKDebugEditor : Editor
{
    private SetPositionData setPosition;
    private Transform Target;
    private Vector2 scrollPos;

    private void OnEnable()
    {
        setPosition = ResourceMgr.Singleton.Load<SetPositionData>("ScriptableObjectData/PlayerPositionData", false);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(false), GUILayout.Height(100));
        for (int i = 0; i < setPosition.positionDatas.Length; i++)
        {
            setPosition.positionDatas[i].isFoldout = EditorGUILayout.Foldout(setPosition.positionDatas[i].isFoldout, setPosition.positionDatas[i].userName, true);
            if (setPosition.positionDatas[i].isFoldout)
            {
                for (int k = 0; k < setPosition.positionDatas[i].data.Length; k++)
                {
                    if (GUILayout.Button("移动到 -- " + setPosition.positionDatas[i].data[k].placename + " -- 位置"))
                    {
                        Target = (target as TransformPositionHelper).transform;
                        string posStr = setPosition.positionDatas[i].data[k].position;
                        posStr = posStr.Replace("(", "").Replace(" ", "").Replace(")", "");
                        string[] vector = posStr.Split(',');
                        Vector3 position = new Vector3(float.Parse(vector[0]), float.Parse(vector[1]), float.Parse(vector[2]));
                        Target.position = position;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }
}