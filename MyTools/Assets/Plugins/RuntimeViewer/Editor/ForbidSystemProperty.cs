using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ForbidSystemProperty
{
    //only forbid property when object is component
    static List<string> componentPropertys = null;
    public static List<string> ComponentPropertys
    {
        get
        {
            if (componentPropertys == null)
            {
                Init();
            }

            return componentPropertys;
        }
    }

    static List<string> gameObjectPropertys = null;
    public static List<string> GameObjectPropertys
    {
        get
        {
            if (gameObjectPropertys == null)
            {
                Init();
            }

            return gameObjectPropertys;
        }
    }

    static void Init()
    {
        componentPropertys = new List<string>();
        componentPropertys.Add("useGUILayout");
        componentPropertys.Add("isActiveAndEnabled");
        componentPropertys.Add("rigidbody");
        componentPropertys.Add("rigidbody2D");
        componentPropertys.Add("camera");
        componentPropertys.Add("light");
        componentPropertys.Add("animation");
        componentPropertys.Add("constantForce");
        componentPropertys.Add("audio");
        componentPropertys.Add("guiText");
        componentPropertys.Add("guiElement");
        componentPropertys.Add("guiTexture");
        componentPropertys.Add("hingeJoint");
        componentPropertys.Add("particleEmitter");
        componentPropertys.Add("particleSystem");
        componentPropertys.Add("collider");
        componentPropertys.Add("collider2D");
        componentPropertys.Add("hideFlags");
        componentPropertys.Add("worldToLocalMatrix");
        componentPropertys.Add("localToWorldMatrix");
        componentPropertys.Add("hasChanged");

        gameObjectPropertys = new List<string>();
        gameObjectPropertys.Add("guiText");
        gameObjectPropertys.Add("guiElement");
        gameObjectPropertys.Add("guiTexture");
        gameObjectPropertys.Add("collider");
        gameObjectPropertys.Add("collider2D");
        gameObjectPropertys.Add("hingeJoint");
        gameObjectPropertys.Add("particleEmitter");
        gameObjectPropertys.Add("UnityEngine");
        gameObjectPropertys.Add("hideFlags");
        gameObjectPropertys.Add("isStatic");
        gameObjectPropertys.Add("rigidbody");
        gameObjectPropertys.Add("rigidbody2D");
        gameObjectPropertys.Add("camera");
        gameObjectPropertys.Add("light");
        gameObjectPropertys.Add("constantForce");
        gameObjectPropertys.Add("audio");
    }
}
