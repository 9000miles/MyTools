using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MarsPC
{
    public class AnimatorStateEventInfo
    {
        /// <summary>
        /// 是否在状态内，不需要外部设置
        /// </summary>
        public bool isInState;

        /// <summary>
        /// 该动画状态对应的动画片段
        /// </summary>
        public AnimationClip clip;

        /// <summary>
        /// 动画事件
        /// </summary>
        public AnimatorStateEvent stateEvent;

        /// <summary>
        /// 延时调用集合，可添加多个时间对应的事件函数
        /// </summary>
        public Dictionary<float, List<AniamtorStateEventDelayCall>> delayCallDic = new Dictionary<float, List<AniamtorStateEventDelayCall>>();
    }

    public enum EAnimatorStateCallType
    {
        EnterCall,
        UpdateCall,
        ExitCall,
    }

    public enum EAnimatorStateChangeCallType
    {
        Append,
        Remove,
        Cover,
    }
}