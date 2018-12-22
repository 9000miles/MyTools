using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CapsuleRaycastTest : MonoBehaviour
{
    private CapsuleCollider collider;
    private RaycastHit hitInfo;

    public bool isGet;
    public bool isLineCast;
    public Transform showBox;
    public Transform point1;
    public Transform point2;
    public Transform point;
    public Transform head;

    public RaycastHit[] raycastHits;
    public List<Transform> rayCastList = new List<Transform>();

    public Vector3 center;
    public Vector3 halfExtents;
    public Vector3 direction;
    public Quaternion orientation;
    public float maxDistance;

    private int index;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        rayCastList.Clear();
        //isGet = Physics.BoxCast(center, halfExtents, direction);
        isGet = Physics.CapsuleCast(point1.position, point2.position, maxDistance, direction);
        raycastHits = Physics.CapsuleCastAll(point1.position, point2.position, maxDistance, direction);
        rayCastList.AddRange(raycastHits.Select(t => t.transform));

        isLineCast = Physics.Linecast(point1.position, point2.position);

        //if (index < raycastHits.Length)
        //{
        //    point.position = raycastHits[index].barycentricCoordinate;
        //}
        //else
        //{
        //    index = 0;
        //}
        point.position = Physics.ClosestPoint(head.position, showBox.GetComponent<Collider>(), showBox.position, showBox.rotation);
        //showBox.localScale = halfExtents * 2;
        //showBox.localPosition = center;
        //showBox.localRotation = orientation;
    }
}