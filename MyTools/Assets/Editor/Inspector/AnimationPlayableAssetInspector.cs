using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CustomEditor(typeof(AnimationPlayableAsset))]
public class AnimationPlayableAssetInspector : Editor
{
    private bool isSel;

    private void Awake()
    {
        Debug.Log("jojfojwoeof");
    }

    private void OnSceneGUI()
    {
        Debug.Log("nonowneogoew");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        isSel = EditorGUILayout.Toggle(isSel, "519444444444444");
        if (isSel)
        {
            Debug.Log("hohfowe");
        }
    }

    [MenuItem("CONTEXT/TestAsset/ojfoweof")]
    public static void Muew()
    {
        Debug.Log(888888888888888888);
    }
}