using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public KeyCode code;
	// Use this for initialization
	void Start () {
		
	}

    //// Update is called once per frame
    //void Update () {

    //}
    public bool isButtonKeyDown
    {
        get {

            if (Input.GetKeyDown(code))
            {
                return true;
            }
            return false;
        }
    }
}
