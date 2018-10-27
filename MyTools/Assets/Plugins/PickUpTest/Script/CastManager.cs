using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastManager : MonoBehaviour
{
    public Transform target;
    public float searchRadius;
    public string layMaskName;
    public float angleScope = 30f;
    public float minDistance = 10f;
    public float minAngle = 120f;
    public float accuracy = 4;
    public float m_distance = 10f;
    public int lineNum = 4;
    public float innerRace = 5f;//近距离
    [HideInInspector]
    public GameObject hitObject = null;

    public System.Action<GameObject> actionA;//A是边缘的探测
    public System.Action<GameObject> actionB;//B是内圈探测
    public System.Action exitArea;//离开区域时调用只调用一次

    private static CastManager _instance = null;
    private Vector3[] corners;
    private float rotatePerSceond = 90f;
    private int acrossAngle;
    private int verticalAngle;
    private bool isInto = false;
    public static CastManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("CastManager");
                _instance = go.AddComponent<CastManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        var rect = GetCorners(m_distance, 2.5f);
        acrossAngle = Mathf.CeilToInt(Vector3.Angle(rect[0], rect[1]));
        verticalAngle = Mathf.CeilToInt(Vector3.Angle(rect[0], rect[2]));

        Debug.Log("AcrossAngle:" + acrossAngle + "---verticalAngle:" + verticalAngle);
    }

    void Update()
    {
        corners = GetCorners(m_distance, 2.5f);
        Debug.DrawLine(corners[0], corners[1], Color.red);
        Debug.DrawLine(corners[1], corners[3], Color.red);
        Debug.DrawLine(corners[3], corners[2], Color.red);
        Debug.DrawLine(corners[2], corners[0], Color.red);

        for (int i = 0; i < corners.Length; i++)
        {
            Debug.DrawLine(target.position, corners[i], Color.green);
        }

        if (lookRect() && !isInto)
        {
            if (hitObject != null)
            isInto = true;
        }
        else if (!lookRect() && isInto && hitObject)
        {
            if (isCrossBorder(hitObject.transform.position))
            {
                isInto = false;
                if (exitArea != null)
                    exitArea();//离开区域
                hitObject = null;
            }
        }
    }
    /// <summary>
    /// 相交球检测
    /// </summary>
    public void CheackCollider()
    {
        Collider[] colliders = Physics.OverlapSphere(target.position, searchRadius, 1 << LayerMask.NameToLayer(layMaskName));
        if (colliders.Length <= 0) return;
        for (int i = 0; i < colliders.Length; i++)
        {
            Debug.Log(colliders[i].gameObject.name);
        }
    }
    /// <summary>
    /// 扇形位置检测
    /// </summary>
    public void RayCastTest()
    {
        //检测扇形角度
        Vector3 newPos = target.transform.forward * 10f;
        float distance = Vector3.Distance(target.transform.position, newPos);

        Quaternion right = target.transform.rotation * Quaternion.AngleAxis(30, Vector3.up);
        Quaternion left = target.transform.rotation * Quaternion.AngleAxis(30, Vector3.down);
        Vector3 n = target.transform.position + (Vector3.forward * distance);
        Vector3 leftPoint = left * n;
        Vector3 rightPoint = right * n;

        Debug.DrawLine(target.transform.position, leftPoint, Color.red);
        Debug.DrawLine(target.transform.position, rightPoint, Color.red);

    }
    /// <summary>
    /// 扇形范围检测是否进入
    /// </summary>
    /// <param name="_distance"></param>
    /// <param name="_avatarPos"></param>
    /// <param name="_enemyPos"></param>
    /// <returns></returns>
    private bool CheackScope(Vector3 _avatarPos, Vector3 _enemyPos, float _tempAngle)
    {
        Vector3 srcVect = _enemyPos - _avatarPos;
        Vector3 fowardPos = target.transform.forward * 1 + _avatarPos;
        Vector3 fowardVect = fowardPos - _avatarPos;
        fowardVect.y = 0;
        float angle = Vector3.Angle(srcVect, fowardVect);
        if (angle < _tempAngle / 2)
        {
            Debug.Log("进入了检测范围");
            return true;
        }
        return false;
    }
    public float rotatePerSecond = 90f;
    public Color debugColor = Color.green;

    /// <summary>
    /// 射线检测
    /// </summary>
    /// <returns></returns>
    private bool lookRect()
    {
        float angle = acrossAngle;
        float subAngleH = angle / accuracy;
        float subAngleV = verticalAngle / lineNum;

        for (int j = 0; j < lineNum; j++)
        {
            Vector3 Angle2Pos = target.rotation * Quaternion.Euler(Vector3.right * -subAngleV * j) * Vector3.forward * m_distance;
            for (int accuracyNum = 0; accuracyNum < accuracy; accuracyNum++)
            {
                if (lookAround(Quaternion.Euler(0, -angle / 2 + accuracyNum * subAngleH + Mathf.Repeat(rotatePerSceond * Time.time, subAngleH), 0), Angle2Pos) &&
                    !hitObject.GetComponent<CheackItem>().isFinish)
                    return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否越界
    /// </summary>
    /// <returns></returns>
    private bool isCrossBorder(Vector3 _enemyPos)
    {
        return CheackScope(target.transform.position, _enemyPos, acrossAngle) == true ? false : true;
    }
    private void InnerRace(float _innerRace, Transform _targetPoint)
    {
        float distance = Vector3.Distance(_targetPoint.position, target.position);
        var action = distance > innerRace ? actionA : actionB;
        if (action != null) action(_targetPoint.gameObject);
    }
    private bool lookAround(Quaternion eulerAnge, Vector3 _direction)
    {
        RaycastHit hit;
        Debug.DrawRay(target.position, eulerAnge * (_direction.normalized * m_distance), Color.red);
        if (Physics.Raycast(target.position, eulerAnge * (_direction.normalized), out hit, m_distance) &&
            hit.collider.CompareTag("Player"))
        {
            GameObject tempHitObj = hit.collider.gameObject;
            hitObject = tempHitObj;
            InnerRace(innerRace, hitObject.transform);
            //Debug.Log("Success");
            return true;
        }

        return false;
    }
    public float fieldOfView = 30f;//视角
    private Vector3[] GetCorners(float _distance, float _officeY)
    {
        Vector3[] corners = new Vector3[4];
        Vector3 mTarget = new Vector3(target.position.x, target.position.y, target.position.z);
        float halfOv = (fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = Camera.main.aspect;
        float height = m_distance * Mathf.Tan(halfOv);//得到正切值
        float weight = height * aspect;

        //UpperLeft
        corners[0] = mTarget - target.right * weight;
        corners[0] += target.up * height;
        corners[0] += target.forward * m_distance;
        //UpperRight
        corners[1] = mTarget + (target.right * weight);
        corners[1] += target.up * height;
        corners[1] += target.forward * m_distance;
        //LowerLeft
        corners[2] = mTarget - (target.right * weight);
        corners[2] -= target.up * height;
        corners[2] += target.forward * m_distance;
        //LowerRight
        corners[3] = mTarget + (target.right * weight);
        corners[3] -= target.up * height;
        corners[3] += target.forward * m_distance;
        float distanceX = Mathf.Abs(corners[1].x - corners[0].x);
        float distanceY = Mathf.Abs(corners[0].y - corners[2].y);
        return corners;
    }
}
