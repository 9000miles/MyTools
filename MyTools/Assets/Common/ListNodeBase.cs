using System.Collections;
using System.Collections.Generic;

/// <summary>
///选择逻辑节点父类(Unity) By:XiongJunYu
///使用细节:
///1子类使用前需要在子类中调用Add()添加元素
///2添加当改变时的OnChange事件,OnChange事件中必须判断last不为null  !!!
///3并使用StartSelect(currentIndex);设置一个初始选择
///4重写Name()和SetActive()以明确对名称和设置激活的定义
/// </summary>
public abstract class ListNodeBase<T> : UnityEngine.MonoBehaviour
{
    public int currentIndex = 0;
    public LogicLoopType loopType;

    public bool otherAcitve = false;

    public delegate void EventHandler(T last, T current);

    public event EventHandler OnChangeEvent;

    private int dirction = 1;
    private T lastElement;

    /// <summary>容器 </summary>
    public List<T> contentList = new List<T>();

    public T last { get { return lastElement; } }

    public T current { get { return contentList[currentIndex]; } }

    /// <summary>到上一个索引</summary>

    public virtual void Last()
    {
        lastElement = current;

        if (loopType == LogicLoopType.circle)
        {
            dirction = 1;
            if (currentIndex == 0)
            {
                currentIndex = contentList.Count - 1;
            }
            else
            {
                currentIndex -= dirction;
            }
        }
        else
        {
            if (loopType == LogicLoopType.pingpong)
            {
                currentIndex -= dirction;
                if (currentIndex == -1 || currentIndex == contentList.Count)
                {
                    dirction *= -1;
                    currentIndex -= 2 * dirction;
                }
            }
        }

        ChangeActive();
        if (OnChangeEvent != null) OnChangeEvent(last, current);
    }

    /// <summary>到下一个索引 </summary>
    public virtual void Next()
    {
        lastElement = current;

        if (loopType == LogicLoopType.circle)
        {
            dirction = 1;
            if (currentIndex == contentList.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex += dirction;
            }
        }
        else
        {
            if (loopType == LogicLoopType.pingpong)
            {
                currentIndex += dirction;
                if (currentIndex == -1 || currentIndex == contentList.Count)
                {
                    dirction *= -1;
                    currentIndex += 2 * dirction;
                }
            }
        }

        ChangeActive();
        if (OnChangeEvent != null) OnChangeEvent(last, current);
    }

    /// <summary>选择元素</summary>
    public virtual void Select(int index)
    {
        if (index == currentIndex) return;
        lastElement = current;

        currentIndex = index;
        ChangeActive();
        if (OnChangeEvent != null) OnChangeEvent(last, current);
    }

    protected virtual void StartSelect(int index)
    {
        //if (index == currentIndex) return;
        //lastElement = current;

        currentIndex = index;
        ChangeActive();
        if (OnChangeEvent != null) OnChangeEvent(last, current);
    }

    public virtual void SelectByname(string s)
    {
        if (s == Name(current)) return;
        int index = contentList.FindIndex(0, p => Name(p) == s);
        Select(index);
    }

    /// <summary>获取名字</summary>
    protected abstract string Name(T t);

    /// <summary>当选择改变 </summary>

    /// <summary> 设置为激活</summary>
    protected abstract void SetActive(T t, bool active);

    private void ChangeActive()
    {
        SetActive(current, true);
        if (otherAcitve) return;

        for (int i = 0; i < contentList.Count; i++)
        {
            if (i == currentIndex)
            {
                continue;
            }
            SetActive(contentList[i], false);
        }
    }

    /// <summary>添加元素</summary>
    public virtual void Add(T element)
    {
        contentList.Add(element);
    }

    public virtual void AddRange<Tenum>(Tenum elements) where Tenum : IEnumerable
    {
        foreach (T item in elements)
        {
            Add(item);
        }
    }

    /// <summary> 移除元素</summary>
    public virtual void Remove(int index)
    {
        contentList.RemoveAt(index);
    }
}

[System.Serializable]
public enum LogicLoopType
{
    circle,
    pingpong
}