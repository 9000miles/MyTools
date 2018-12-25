using System.Collections;
using System.Collections.Generic;
using Common;
using MarsPC;
using UnityEngine;

public class TouchWallTest : MonoBehaviour
{
    public bool isGetWall;
    public Transform point;
    public Transform edgePoint;
    public float radius = 0.5f;
    public float dis = 0.2f;
    public Transform hand;
    public LayerMask touchLayers; // The layers to touch
    private RaycastHit hit = new RaycastHit();

    private void Start()
    {
    }

    private void Update()
    {
        //SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance, int layerMask)
        //RaycastHit[] hits=Physics.SphereCastAll( )
        Collider[] colliders = Physics.OverlapSphere(hand.position, radius, touchLayers);
        isGetWall = colliders != null && colliders.Length > 0 ? true : false;
        if (isGetWall)
        {
            Collider minPoint = colliders.GetMin(t => Vector3.Distance(t.ClosestPoint(hand.position), hand.position));
            point.position = minPoint.ClosestPoint(hand.position);
            edgePoint.position = minPoint.ClosestPointOnBounds(hand.position);
            RaycastHit hit;
            Physics.Raycast(hand.position, minPoint.transform.position - hand.position, out hit);
            Debug.DrawRay(point.position, hit.normal);
        }
        //isGetWall = Physics.SphereCast(hand.position, radius, Vector3.zero, out hit, dis, touchLayers);
        //if (isGetWall)
        //{
        //    point.position = hit.point;
        //}
    }
}