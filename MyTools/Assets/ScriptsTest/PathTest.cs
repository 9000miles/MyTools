using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        print(Application.dataPath);
        print(Application.persistentDataPath);
        print(Application.streamingAssetsPath);
        print(Application.temporaryCachePath);
    }
}