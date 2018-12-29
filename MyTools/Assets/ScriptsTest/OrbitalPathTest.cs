using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalPathTest : MonoBehaviour
{
    public float length;
    public float curLength;
    public bool isUnits;
    public float value;
    public float percent;
    public CinemachinePathBase.PositionUnits units;
    public CinemachineSmoothPath path;
    public Transform point;
    public Transform other;
    public int startSegment;
    public int searchRadius;
    public int stepsPerSegment;
    public CinemachineSmoothPath otherPath;

    // Use this for initialization
    private void Start()
    {
        otherPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[path.m_Waypoints.Length];
        path.m_Waypoints.CopyTo(otherPath.m_Waypoints, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        length = path.PathLength;
        value = path.FindClosestPoint(point.position, startSegment, searchRadius, stepsPerSegment);
        transform.position = path.EvaluatePosition(value);
        transform.rotation = path.EvaluateOrientation(value);
        Debug.DrawRay(transform.position, path.EvaluateTangent(value));
        if (isUnits)
        {
            other.position = path.EvaluatePositionAtUnit(value, units);
            other.rotation = path.EvaluateOrientationAtUnit(value, units);
            Debug.DrawRay(other.position, path.EvaluateTangentAtUnit(value, units));
        }
        else
        {
            other.position = path.EvaluatePosition(value);
            other.rotation = path.EvaluateOrientation(value);
            Debug.DrawRay(other.position, path.EvaluateTangent(value));
        }
        //otherPath.m_Waypoints[otherPath.m_Waypoints.Length - 1].position = transform.position;
        //curLength = otherPath.PathLength;
        //percent = value / (path.m_Waypoints.Length - 1f);
        //curLength = percent * length;
    }
}