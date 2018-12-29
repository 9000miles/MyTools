using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class RVControlBase
{
    public string UID { private set; get; }//用于识别的唯一ID, 就是名字路径,如果在集合中则还需加上编号
    public readonly int depth = 0;

    public string NameLabel { private set; get; }
    protected object data = null;

    //层缩进长度,其实值为 Indent * depth
    public static readonly int Indent_class = 5;

    public static readonly int Indent_field = 5;
    public float IndentPlus = 0;

    protected RVVisibility rvVisibility;
    protected Rect nameLabelRect;
    private int nowStayFrame = 0;
    private int maxStayFrame = 8;

    protected RVSettingData settingData;
    protected RVCStatus rvcStatus;
    public RVControlBase Parent { private set; get; }

    protected RuntimeViewer rv { private set; get; }

    public RVControlBase(RuntimeViewer rv, string UID, string nameLabel, object data, int depth, RVVisibility rvVisibility,
        RVControlBase parent)
    {
        this.rv = rv;
        this.Parent = parent;
        this.UID = UID;
        this.NameLabel = nameLabel;
        this.data = data;
        this.depth = depth;
        this.rvVisibility = rvVisibility;
    }

    public virtual void OnGUIUpdate(bool isRealtimeUpdate, RVSettingData settingData, RVCStatus rvcStatus)
    {
        this.settingData = settingData;
        this.rvcStatus = rvcStatus;
    }

    //execute after 'OnGUIUpdate' use to update tip !
    public virtual void OnGUIUpdate_Late()
    {
        if (IsNormalizedRect(nameLabelRect) == true)
            return;

        if (IsInRect(Event.current.mousePosition, nameLabelRect))
        {
            if (nowStayFrame < maxStayFrame)
            {
                nowStayFrame++;
            }
            else
            {
                nameLabelRect.x = EditorGUIUtility.singleLineHeight;
                nameLabelRect.y += EditorGUIUtility.singleLineHeight + 4;
                string text = GetTipString();
                //EditorGUI.TextArea(GetRectByText(nameLabelRect, text), text);
            }
        }
        else
        {
            nowStayFrame = 0;
        }
    }

    public virtual void OnDestroy()
    {
    }

    public override bool Equals(object obj)
    {
        if (obj is RVControlBase == false)
            return false;
        return this.UID.Equals(((RVControlBase)obj).UID);
    }

    public override int GetHashCode()
    {
        return this.UID.GetHashCode();
    }

    public void UpdateData(RVControlBase newData)
    {
        this.data = newData.data;
        this.rvVisibility = newData.rvVisibility;
    }

    protected Rect GetRectByText(Rect rect, string str)
    {
        if (str == null || str == "")
            return rect;

        string[] lines = str.Split(new char[] { '\n' });
        string longestLine = lines[0];

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length > longestLine.Length)
                longestLine = lines[i];
        }

        rect.width = longestLine.Length * this.settingData.LowercaseLength;
        rect.height = EditorGUIUtility.singleLineHeight * lines.Length;

        return rect;
    }

    protected virtual string GetTipString()
    {
        string text = "";
        if (this.data == null)
            text += "ToString() : null";
        else
            text += "ToString() : " + this.data.ToString();

        text += "\ntype>> ";

        if (this.rvVisibility != null && this.rvVisibility.ValueType != null)
            text += this.rvVisibility.ValueType.ToString();

        text += "\npath>> " + this.GetNamePath();
        return text;
    }

    private bool IsNormalizedRect(Rect rect)
    {
        if (rect.x == 0 && rect.y == 0 && rect.width == 1 && rect.height == 1)
            return true;
        return false;
    }

    private bool IsInRect(Vector2 mousePos, Rect rect)
    {
        if (mousePos.y < rect.y)
            return false;
        if (mousePos.y > rect.y + rect.height)
            return false;
        if (mousePos.x < rect.x)
            return false;
        return true;
    }

    protected string GetNamePath()
    {
        string str = "";
        if (this.Parent != null)
            str = this.Parent.GetNamePath() + " - " + RVHelper.GetClearNameString(this.NameLabel);
        else
            str = this.NameLabel;
        str = str.Replace("┌ ", "").Replace("  ┐", "").Replace("└", "").Replace("┘", "");
        return str;
    }
}