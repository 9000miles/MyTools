using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CurveTest))]
public class CuraveEditor : Editor
{
    private enum PreviewState
    {
        Play,
        Pause,
        Stop,
    }

    private PreviewState _previewState = PreviewState.Stop;
    private float _moveTotalTime = 0;
    private float _lastTime = 0;

    private const float AnchorSize = 0.3f;
    private const float DeltaTime = 0.005f;
    private readonly Handles.CapFunction sphereCap = Handles.SphereHandleCap;
    private CurveTest _bomb { get { return target as CurveTest; } }

    private void PreviewInScene()
    {
        float currentTime = (float)EditorApplication.timeSinceStartup;
        float deltaTime = currentTime - _lastTime;
        _lastTime = currentTime;

        _moveTotalTime += deltaTime;
        if (_bomb.transform.position.y >= _bomb.originalPosition.y)
        {
            _bomb.transform.position = _bomb.GetPosition(_moveTotalTime);
        }
    }

    private void OnSceneGUI()
    {
        if (EditorApplication.isPlaying) return;

        switch (_previewState)
        {
            case PreviewState.Play:
                PreviewInScene();
                break;
            case PreviewState.Pause:
                break;
            case PreviewState.Stop:
                DrawHandle();
                _bomb.UpdateOriginalPosition();
                break;
        }

        DrawCurve();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (EditorApplication.isPlaying) return;

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play"))
        {
            _previewState = PreviewState.Play;
            _lastTime = (float)EditorApplication.timeSinceStartup;
        }
        if (GUILayout.Button("Pause"))
        {
            _previewState = PreviewState.Pause;
        }
        if (GUILayout.Button("Stop"))
        {
            _previewState = PreviewState.Stop;
            _moveTotalTime = 0;
            _bomb.transform.position = _bomb.originalPosition;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawHandle()
    {
        Handles.color = Color.blue;
        var originalPos = _bomb.transform.position;
        float handleSize = HandleUtility.GetHandleSize(originalPos);
        float constSize = handleSize * AnchorSize;

        _bomb.transform.position = Handles.FreeMoveHandle(originalPos, Quaternion.identity, constSize, Vector3.one, sphereCap);

        float maxHeighTime = _bomb.GetTotalTime() / 2;
        Vector3 maxHeightPos = _bomb.GetPosition(maxHeighTime);

        handleSize = HandleUtility.GetHandleSize(maxHeightPos);
        constSize = handleSize * AnchorSize;
        maxHeightPos = Handles.FreeMoveHandle(maxHeightPos, Quaternion.identity, constSize, Vector3.one, sphereCap);

        Vector3 maxLengthPos = _bomb.GetPosition(maxHeighTime * 2);
        handleSize = HandleUtility.GetHandleSize(maxLengthPos);
        constSize = handleSize * AnchorSize;
        maxLengthPos = Handles.FreeMoveHandle(maxLengthPos, Quaternion.identity, constSize, Vector3.one, sphereCap);

        float yAxisDeltaDistance = maxHeightPos.y - _bomb.originalPosition.y;
        float zAxisDeltaDistance = maxLengthPos.z - _bomb.originalPosition.z;
        if (yAxisDeltaDistance > 0 && zAxisDeltaDistance > 0)
        {
            _bomb.initialAngle = Mathf.Atan(4 * yAxisDeltaDistance / zAxisDeltaDistance) / Mathf.Deg2Rad;
            _bomb.initialSpeed = Mathf.Sqrt(_bomb.GetGravity() * zAxisDeltaDistance / Mathf.Sin(2 * _bomb.initialAngle * Mathf.Deg2Rad));
        }
    }

    private void DrawCurve()
    {
        float totalTime = 0;
        while (_bomb.GetPosition(totalTime).y >= _bomb.originalPosition.y)
        {
            Vector3 pos1 = _bomb.GetPosition(totalTime);
            totalTime += DeltaTime;
            Vector3 pos2 = _bomb.GetPosition(totalTime);

            Handles.color = Color.red;
            Handles.DrawLine(pos1, pos2);
        }
    }
}