using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MarsPC
{
    public class AnimatorStateEvent
    {
        private float _delayTime;
        /// <summary>
        /// 延时时间，整个时间为1，具体参见AnimationState.normalizedTime
        /// </summary>
        public float DelayTime
        {
            get { return _delayTime; }
            set { _delayTime = Mathf.Clamp01(value); }
        }

        /// <summary>
        /// 动画状态完整路径
        /// </summary>
        public string FullName;

        /// <summary>
        /// 进入该状态时调用
        /// </summary>
        public Action OnEnterCall;

        /// <summary>
        /// 播放该状态时一直调用
        /// </summary>
        public Action OnUpdateCall;

        /// <summary>
        /// 离开该状态调用
        /// </summary>
        public Action OnExitCall;

        /// <summary>
        /// 进入该状态之后延时调用，延时时间为delayNormalizedTime，整个时间为1，具体参见AnimationState.normalizedTime
        /// </summary>
        public Action OnDelayCall;
    }

    public class AniamtorStateEventDelayCall
    {
        /// <summary>
        /// 是否已经延时调用了，不需要外部设置
        /// </summary>
        public bool delayCalled;

        /// <summary>
        /// 延时调用函数
        /// </summary>
        public Action delayCall;
    }
}