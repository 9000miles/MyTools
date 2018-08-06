using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(LineRenderer))]
public class LineRendererTest : MonoBehaviour
{
    private LineRenderer line;
    private bool isMousePressed;
    private List<Vector3> pointsList;
    private Vector3 mousePos;

    //private Vector3 m_DownCamPos;
    //private Vector3 m_mouseDownStartPos;
    //主相机节点下
    // Structure for line points
    private struct myLine
    {
        public Vector3 StartPoint;
        public Vector3 EndPoint;
    };

    //	-----------------------------------
    private void Awake()
    {
        _Init();
    }

    private void _Init()
    {
        if (m_init) return;
        m_init = true;
        // Create line renderer component and set its property
        //line = this.GetComponent<LineRenderer>();
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Particles/Additive"));
        line.SetVertexCount(0);
        line.SetWidth(0.1f, 0.1f);
        line.SetColors(Color.green, Color.green);
        line.useWorldSpace = true;

        isMousePressed = false;
        pointsList = new List<Vector3>();
        //line.sortingLayerName = "Ignore Raycast";
        line.sortingOrder = 999;
        //		renderer.material.SetTextureOffset(
        //m_mainCam = this.GetComponent<Camera>();
    }

    private bool m_init = false;

    //Camera m_mainCam;
    //	-----------------------------------
    private void Update()
    {
        // If mouse button down, remove old line and set its color to green
        if (Input.GetMouseButtonDown(1))
        {
            isMousePressed = true;
            line.SetVertexCount(0);
            pointsList.RemoveRange(0, pointsList.Count);
            line.SetColors(Color.green, Color.green);
            //m_DownCamPos = Camera.main.transform.position;
            //m_mouseDownStartPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 1f));
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isMousePressed = false;
        }

        //if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
        //    Vector3 crelpos = Camera.main.transform.position;
        //    for (int i = 0; i < pointsList.Count; i++) {
        //        line.SetPosition(i, crelpos + pointsList[i]);
        //        //line.SetPosition(pointsList.Count - 1, (Vector3)pointsList[pointsList.Count - 1]);
        //    }
        //}
        // Drawing line when mouse is moving(presses)
        if (isMousePressed)
        {
            Vector3 crelpos = Camera.main.transform.position;
            //将鼠标点击的屏幕坐标转换为世界坐标，然后存储到position中
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 1f));
            Vector3 offsetpos = mousePos - crelpos;
            //Vector3 hitp = Vector3.zero;
            //bool ok = GetRayCastZero2DPlanePoint(Camera.main, Camera.main.transform.TransformPoint(0f, 0f, Camera.main.nearClipPlane + 1f).z, out hitp);
            //mousePos = hitp;
            if (!pointsList.Contains(offsetpos))
            {
                pointsList.Add(offsetpos);
                line.SetVertexCount(pointsList.Count);

                for (int i = 0; i < pointsList.Count; i++)
                {
                    line.SetPosition(i, crelpos + pointsList[i]);
                    //line.SetPosition(pointsList.Count - 1, (Vector3)pointsList[pointsList.Count - 1]);
                }
                if (isLineCollide())
                {
                    isMousePressed = false;
                    line.SetColors(Color.red, Color.red);
                }
            }
        }
    }

    //	-----------------------------------
    //  Following method checks is currentLine(line drawn by last two points) collided with line
    //	-----------------------------------
    private bool isLineCollide()
    {
        if (pointsList.Count < 2)
            return false;
        int TotalLines = pointsList.Count - 1;
        myLine[] lines = new myLine[TotalLines];
        if (TotalLines > 1)
        {
            for (int i = 0; i < TotalLines; i++)
            {
                lines[i].StartPoint = (Vector3)pointsList[i];
                lines[i].EndPoint = (Vector3)pointsList[i + 1];
            }
        }
        for (int i = 0; i < TotalLines - 1; i++)
        {
            myLine currentLine;
            currentLine.StartPoint = (Vector3)pointsList[pointsList.Count - 2];
            currentLine.EndPoint = (Vector3)pointsList[pointsList.Count - 1];
            if (isLinesIntersect(lines[i], currentLine))
                return true;
        }
        return false;
    }

    //	-----------------------------------
    //	Following method checks whether given two points are same or not
    //	-----------------------------------
    private bool checkPoints(Vector3 pointA, Vector3 pointB)
    {
        return (pointA.x == pointB.x && pointA.y == pointB.y);
    }

    //	-----------------------------------
    //	Following method checks whether given two line intersect or not
    //	-----------------------------------
    private bool isLinesIntersect(myLine L1, myLine L2)
    {
        if (checkPoints(L1.StartPoint, L2.StartPoint) ||
            checkPoints(L1.StartPoint, L2.EndPoint) ||
            checkPoints(L1.EndPoint, L2.StartPoint) ||
            checkPoints(L1.EndPoint, L2.EndPoint))
            return false;

        return ((Mathf.Max(L1.StartPoint.x, L1.EndPoint.x) >= Mathf.Min(L2.StartPoint.x, L2.EndPoint.x)) &&
               (Mathf.Max(L2.StartPoint.x, L2.EndPoint.x) >= Mathf.Min(L1.StartPoint.x, L1.EndPoint.x)) &&
               (Mathf.Max(L1.StartPoint.y, L1.EndPoint.y) >= Mathf.Min(L2.StartPoint.y, L2.EndPoint.y)) &&
               (Mathf.Max(L2.StartPoint.y, L2.EndPoint.y) >= Mathf.Min(L1.StartPoint.y, L1.EndPoint.y))
               );
    }
}