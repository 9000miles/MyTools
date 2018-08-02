using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    ///<summary>
    /// 脚本单例类
    ///</summary>
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        private static T singleton;

        //按需加载
        public static T Singleton
        {
            get
            {
                if (singleton == null)
                {
                    //在场景中查找该类型实例
                    singleton = FindObjectOfType<T>();
                    if (singleton == null)
                    {
                        //创建该类型组件对象
                        singleton = new GameObject("Singleto of " + typeof(T).ToString()).AddComponent<T>();
                    }
                    singleton.Init();//通知实现类初始化
                }
                return singleton;
            }
        }

        protected void Awake()
        {
            //如果当前组件在场景中默认存在  则通过as为字段赋值
            if (singleton == null)
            {
                singleton = this as T;
                Init();//通知实现类初始化
            }
        }

        public virtual void Init()
        {
        }
    }
}