using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手柄输入事件封装 By：XiongJunYu
/// 挂在手柄上
/// </summary>
public class ControllerEvent : MonoBehaviour
{
    public static List<My_ControllerEventTrigger> EventTriggers = new List<My_ControllerEventTrigger>();

    /// <summary> Update中 菜单键键按下 </summary>
    public event Action<GameObject> OnApplicationMenuDown_update;

    /// <summary> Update中 触摸板左键按下 </summary>
    public event Action<GameObject> OnTouchPadLeftDown_update;

    /// <summary> Update中 触摸板右键按下 </summary>
    public event Action<GameObject> OnTouchPadRightDown_update;

    /// <summary> Update中 触摸板上键按下 </summary>
    public event Action<GameObject> OnTouchPadUpDown_update;

    /// <summary> Update中 触摸板下键按下 </summary>
    public event Action<GameObject> OnTouchPadDownDown_update;

    /// <summary> Update中 触摸板按住事件 </summary>
    public event Action<GameObject> OnTouchPadPress_update;

    /// <summary>Update中 触摸板（按住后）抬起事件 </summary>
    public event Action<GameObject> OnTouchPadRiseUp_update;

    /// <summary> Update中 扳机按住事件 </summary>
    public event Action<GameObject> OnTriggerPress_update;

    /// <summary> Update中 扳机（按住后）抬起事件 </summary>
    public event Action<GameObject> OnTriggerRiseUp_update;

    private SteamVR_TrackedObject trackedObj;
    private FixedJoint joint;

    private void Awake()
    {
        //在EventTriggers注册这个手柄
        EventTriggers.Add(this);
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    private void Update()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        #region 按下

        //菜单键按下
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            if (OnApplicationMenuDown_update != null)
            {
                OnApplicationMenuDown_update(this.gameObject);
            }
        }
        //触摸板键按下
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Vector2 axis = device.GetAxis();

            float angle = Vector2.Angle(axis, Vector2.right);

            if (InRange(angle, -45f, 45f))
            {
                if (OnTouchPadRightDown_update != null)
                {
                    OnTouchPadRightDown_update(this.gameObject);
                }
            }
            else if (InRange(angle, 45f, 135f))
            {
                if (axis.y > 0)
                {
                    // print("上");
                    if (OnTouchPadUpDown_update != null)
                    {
                        OnTouchPadUpDown_update(this.gameObject);
                    }
                }
                else
                {
                    if (OnTouchPadDownDown_update != null)
                    {
                        OnTouchPadDownDown_update(this.gameObject);
                    }
                }
            }
            else
            {
                //print("左");
                if (OnTouchPadLeftDown_update != null)
                {
                    OnTouchPadLeftDown_update(this.gameObject);
                }
            }
        }

        #endregion 按下

        #region 按住-抬起

        //触摸板按住

        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (OnTouchPadPress_update != null)
            {
                OnTouchPadPress_update(this.gameObject);
            }
        }
        //触摸板（按住后）抬起
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (OnTouchPadRiseUp_update != null)
            {
                OnTouchPadRiseUp_update(this.gameObject);
            }
        }

        //扳机按住按住
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (OnTriggerPress_update != null)
            {
                OnTriggerPress_update(this.gameObject);
            }
        }
        //扳机（按住后）抬起
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (OnTriggerRiseUp_update != null)
            {
                OnTriggerRiseUp_update(this.gameObject);
            }
        }

        #endregion 按住-抬起
    }

    #region 私有方法

    /// <summary> 浮点数是否在范围内 </summary>
    private bool InRange(float f, float min, float max)
    {
        if (f > min && f < max)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion 私有方法
}