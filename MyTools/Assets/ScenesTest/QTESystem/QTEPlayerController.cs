using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEPlayerController : MonoBehaviour
{
    public float speed = 2;

    private void Start()
    {
    }

    private void Update()
    {
        //if (QTEManager.Singleton.isNewQTE == true) return;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);
        }
        //if (Input.GetKeyDown("a"))
        //    Debug.Log("AAAAAAAAAAAAAAAA");
        //if (!Input.GetKeyDown(QTEKeyCode.B.ToString().ToLower()))
        //    Debug.Log("按下了：" + QTEKeyCode.B);
    }
}