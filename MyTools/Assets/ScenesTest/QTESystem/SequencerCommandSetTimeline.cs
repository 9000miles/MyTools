//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Playables;
//using UnityEngine.Timeline;

//namespace PixelCrushers.DialogueSystem.SequencerCommands
//{
//    public class SequencerCommandSetTimeline : SequencerCommand
//    {
//        //是否循环
//        private bool mIsLoop = false;

//        private bool noWait = false;
//        private bool isEnd = false;//播放完整个timeline
//        private PlayableDirector playableDirector = null;
//        private PlayableDirector mLoopPlayable;
//        private List<PlayableDirector> playableLists;

//        public IEnumerator Start()
//        {
//            SetInit();
//            var mode = GetParameter(0).ToLower();
//            //不写默认 为Speaker
//            var subject = GetSubject(1, Sequencer.Speaker);
//            var isloop = string.Equals(GetParameter(2), "isloop", System.StringComparison.OrdinalIgnoreCase) ||
//                string.Equals(GetParameter(3), "isloop", System.StringComparison.OrdinalIgnoreCase) ||
//                string.Equals(GetParameter(4), "isLoop", System.StringComparison.OrdinalIgnoreCase)
//                ;
//            var noWait = string.Equals(GetParameter(2), "nowait", System.StringComparison.OrdinalIgnoreCase) ||
//                string.Equals(GetParameter(3), "nowait", System.StringComparison.OrdinalIgnoreCase) ||
//                string.Equals(GetParameter(4), "nowait", System.StringComparison.Ordinal);
//            var isEnd = string.Equals(GetParameter(2), "isEnd", System.StringComparison.OrdinalIgnoreCase) ||
//                string.Equals(GetParameter(3), "isEnd", System.StringComparison.OrdinalIgnoreCase) ||
//                string.Equals(GetParameter(4), "isEnd", System.StringComparison.OrdinalIgnoreCase);
//            playableDirector = (subject != null) ? subject.GetComponent<PlayableDirector>() : null;
//            //if (isEnd)
//            //    GetBeforeTimeline();
//            //else
//            //    StopAllPlayable();//开始播放关闭所有Timeline
//            if (isloop) mIsLoop = true;
//            else mIsLoop = false;
//            if (playableDirector != null)
//            {
//                var isModeValid = (mode == "play" || mode == "pause" || mode == "resume" || mode == "stop");
//                if (!isModeValid)
//                {
//                    if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: Timeline(" + GetParameters() +
//                   "): Invalid mode '" + mode + "'. Expected 'play', 'pause', 'resume', or 'stop'.");
//                }
//                else
//                {
//                    if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: Sequencer: Timeline(" + GetParameters() + ")");
//                    switch (mode)
//                    {
//                        case "play":
//                            SetPlayableLoop(playableDirector, mIsLoop);//设置循环
//                            if (isEnd && beforePlayable != null)
//                            {
//                                var beforePlayableTime = (beforePlayable.duration) - beforePlayable.time;
//                                bool isSkip = beforePlayableTime <= 0.1f ? true : false;
//                                if (!isSkip)
//                                {
//                                    var myTime = DialogueTime.time + beforePlayableTime;
//                                    var endTime = noWait ? 0 : myTime + playableDirector.playableAsset.duration;
//                                    bool isInverst = false;
//                                    while (DialogueTime.time < endTime)
//                                    {
//                                        if (!isInverst && beforePlayable.duration - beforePlayable.time <= 0.1f)
//                                        {
//                                            isInverst = true;
//                                            StopAllPlayable(isInverst);
//                                            OnPlayDirect();
//                                        }
//                                        yield return null;
//                                    }
//                                    isEnd = false;
//                                }
//                                else
//                                {
//                                    var endTime = noWait ? 0 : DialogueTime.time + playableDirector.playableAsset.duration;
//                                    StopAllPlayable();
//                                    OnPlayDirect();
//                                    while (DialogueTime.time < endTime)
//                                    {
//                                        yield return null;
//                                    }
//                                }
//                            }
//                            else if (!isEnd && beforePlayable == null)
//                            {
//                                StopAllPlayable();
//                                OnPlayDirect();
//                                var endTime = noWait ? 0 : DialogueTime.time + playableDirector.playableAsset.duration;
//                                while (DialogueTime.time < endTime)
//                                {
//                                    yield return null;
//                                }
//                            }
//                            break;

//                        case "pause":
//                            playableDirector.Pause();

//                            break;

//                        case "resume":
//                            playableDirector.Resume();
//                            var reusumEndTime = noWait ? 0 : DialogueTime.time + playableDirector.playableAsset.duration;
//                            while (DialogueTime.time < reusumEndTime)
//                            {
//                                yield return null;
//                            }
//                            break;

//                        case "stop":
//                            playableDirector.Stop();
//                            break;

//                        default:
//                            isModeValid = false;
//                            break;
//                    }
//                }
//            }
//            Stop();//结束本条对话
//        }

//        private void SetInit()
//        {
//            PlayableDirector[] playables = GameObject.FindObjectsOfType<PlayableDirector>();
//            playableLists = new List<PlayableDirector>(playables);
//        }

//        private void SetPlayableLoop(PlayableDirector _playable, bool _isLoop)
//        {
//            if (_isLoop)
//            {
//                mLoopPlayable = _playable;
//                _playable.stopped += (PlayableDirector tempPlayable) =>
//                  {
//                      if (CheackTimelinePlay(tempPlayable))
//                          tempPlayable.Pause();
//                      else tempPlayable.Play();
//                  };
//                return;
//            }
//        }

//        private bool CheackTimelinePlay(PlayableDirector _currectPlayable)
//        {
//            var enumPlayable = playableLists.GetEnumerator();
//            while (enumPlayable.MoveNext())
//            {
//                var playable = enumPlayable.Current;
//                if (string.Compare(playable.name, _currectPlayable.name) != 0 && playable.state == PlayState.Playing)
//                {
//                    //Debug.Log("正在播放的TImeline:" + playable);
//                    return true;
//                }
//            }
//            return false;
//        }

//        private void StopAllPlayable(bool isPause = false)
//        {
//            if (playableDirector.playOnAwake) return;
//            var enumPlayable = playableLists.GetEnumerator();
//            //if (mLoopPlayable != null)
//            //Debug.Log("mLoop:" + mLoopPlayable.name);
//            while (enumPlayable.MoveNext())
//            {
//                var TempPlayable = enumPlayable.Current;
//                if ((TempPlayable.state == PlayState.Playing || TempPlayable.state == PlayState.Paused))
//                {
//                    if (isPause || (mLoopPlayable != null && mLoopPlayable.name == TempPlayable.name)) TempPlayable.Pause();
//                    else if (mLoopPlayable == null) TempPlayable.Stop();
//                }
//            }
//        }

//        private void OnPlayDirect()
//        {
//            if (!playableDirector || playableDirector.playOnAwake) return;
//            //if (playableDirector.time == playableDirector.initialTime)
//            playableDirector.Play();
//            //else playableDirector.Resume();
//        }

//        private PlayableDirector beforePlayable;

//        //等待上一个TimeLine时间

//        //public void OnDestroy()
//        //{
//        //    if (playableDirector != null)
//        //        playableDirector.Stop();
//        //}
//        //如果返回为false则会不执行次条目
//    }
//}