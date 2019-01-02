//这个是基类

using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditorMonoBehaviour
{
    static EditorMonoBehaviour()
    {
        //HierarchyProperty
        //var type = Type.GetType("UnityEditor.EditorAssemblies");
        //var method = type.GetMethod("SubclassesOf", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Type) }, null);
        //var e = method.Invoke(null, new object[] { typeof(EditorMonoBehaviour) }) as IEnumerable;
        //foreach (Type editorMonoBehaviourClass in e)
        //{
        //    method = editorMonoBehaviourClass.BaseType.GetMethod("OnEditorMonoBehaviour", BindingFlags.NonPublic | BindingFlags.Instance);
        //    if (method != null)
        //    {
        //        method.Invoke(System.Activator.CreateInstance(editorMonoBehaviourClass), new object[0]);
        //    }
        //}
        //EditorApplication.update += UpdateCall;
        //EditorApplication.hierarchyWindowChanged += Change;
        //EditorApplication.modifierKeysChanged += KeysChanged;
        //SceneView.onSceneGUIDelegate += OnSceneGUI;
        //EditorApplication.hierarchyWindowItemOnGUI += hierarchyOnGUI;
    }

    private static void hierarchyOnGUI(int instanceid, Rect selectionrect)
    {
        //Debug.Log(instanceid);
        //Debug.Log(selectionrect.position);
    }

    private static void OnSceneGUI(SceneView sceneview)
    {
        //Event e = Event.current;
        //Debug.Log("delta:" + e.delta);
        //Debug.Log("mousePosition:" + e.mousePosition);
        //Debug.Log("isMouse:" + e.isMouse);
        //Debug.Log("character:" + e.character);
        //Debug.Log("isScrollWheel:" + e.isScrollWheel);
        //Debug.Log("type:" + e.type);
        //Debug.Log("GetEventCount:" + Event.GetEventCount());
    }

    internal static void UpdateCall()
    {
        //Debug.Log("UpdateCall");
        Debug.Log("mouseOverWindow:" + EditorWindow.mouseOverWindow);
        Debug.Log("focusedWindow:" + EditorWindow.focusedWindow);
        //SceneHierarchyWindow sceneHierarchy = SceneHierarchyWindow.lastInteractedHierarchyWindow;
    }

    private static void KeysChanged()
    {
        Debug.Log("KeysChanged");
    }

    private static void Change()
    {
        Debug.Log("hierarchyWindowChanged");
    }

    private void OnEditorMonoBehaviour()
    {
        EditorApplication.update += Update;
        EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        EditorApplication.projectWindowChanged += OnProjectWindowChanged;
        EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
        EditorApplication.modifierKeysChanged += OnModifierKeysChanged;

        // globalEventHandler
        EditorApplication.CallbackFunction function = () => OnGlobalEventHandler(Event.current);
        FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
        EditorApplication.CallbackFunction functions = (EditorApplication.CallbackFunction)info.GetValue(null);
        functions += function;
        info.SetValue(null, (object)functions);

        EditorApplication.searchChanged += OnSearchChanged;

        EditorApplication.playmodeStateChanged += () =>
        {
            if (EditorApplication.isPaused)
            {
                OnPlaymodeStateChanged(PlayModeState.Paused);
            }
            if (EditorApplication.isPlaying)
            {
                OnPlaymodeStateChanged(PlayModeState.Playing);
            }
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                OnPlaymodeStateChanged(PlayModeState.PlayingOrWillChangePlaymode);
            }
        };
    }

    public virtual void Update()
    {
    }

    public virtual void OnHierarchyWindowChanged()
    {
    }

    public virtual void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
    }

    public virtual void OnProjectWindowChanged()
    {
    }

    public virtual void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
    {
    }

    public virtual void OnModifierKeysChanged()
    {
    }

    public virtual void OnGlobalEventHandler(Event e)
    {
    }

    public virtual void OnSearchChanged()
    {
    }

    public virtual void OnPlaymodeStateChanged(PlayModeState playModeState)
    {
    }

    public enum PlayModeState
    {
        Playing,
        Paused,
        Stop,
        PlayingOrWillChangePlaymode
    }
}