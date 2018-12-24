using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

public class RVText : RVControlBase
{
    bool isSelected = false;
    RVInput rvInput;

    public RVText(RuntimeViewer rv, string UID, string nameLabel, object data, int depth, RVVisibility rvVisibility, RVControlBase parent)
        : base(rv, UID, nameLabel, data, depth, rvVisibility, parent)
    {
    }

    public override void OnGUIUpdate(bool isRealtimeUpdate, RVSettingData settingData, RVCStatus rvcStatus)
    {
        base.OnGUIUpdate(isRealtimeUpdate, settingData, rvcStatus);

        string _valueStr = GetValueString(this.data);

        isSelected = rvcStatus.IsSelected(this.NameLabel);
        if (isSelected == false)
            isSelected = rvcStatus.IsSelected(_valueStr);

        GUIStyle guiStyle = SelectNameGUIStyle(settingData);
        GUIStyle value_guiStyle = SelectValueGUIStyle(settingData);

        EditorGUILayout.BeginHorizontal();

        //Indent
        EditorGUILayout.LabelField("     ", GUILayout.Width(depth * RVControlBase.Indent_field + IndentPlus));

        //Name
        string nameString = this.NameLabel + " : ";
        EditorGUILayout.LabelField(nameString, guiStyle, GUILayout.Width(guiStyle.CalcSize(new GUIContent(nameString)).x));
        nameLabelRect = GUILayoutUtility.GetLastRect();

        //right click Menu
        FieldMenu(GUILayoutUtility.GetLastRect(), settingData, this.NameLabel, _valueStr);

        //Value
        OnGUI_Value(settingData, _valueStr, value_guiStyle);

        EditorGUILayout.EndHorizontal();
    }

    void OnGUI_Value(RVSettingData settingData, string _valueStr, GUIStyle value_guiStyle)
    {
        //if (rvInput == null || this.rvVisibility.ValueType == null)
        if (rvInput == null)
        {
            if (this.data == null)
                EditorGUILayout.LabelField(_valueStr, settingData.Get_value_null());
            else
                EditorGUILayout.LabelField(_valueStr, value_guiStyle);
        }
        else
        {
            this.rvInput.OnGUI();
        }
    }

    public override void OnDestroy()
    {
    }

    GUIStyle SelectValueGUIStyle(RVSettingData settingData)
    {
        GUIStyle _nowStyle = settingData.Get_value_others();

        if (RVHelper.IsString(this.rvVisibility.ValueType) == true)
        {
            _nowStyle = settingData.Get_value_string();
        }
        else if (RVHelper.IsEnum(this.rvVisibility.ValueType) == true)
        {
            _nowStyle = settingData.Get_value_enum();
        }
        else if (RVHelper.IsNormalType(this.rvVisibility.ValueType) == true)
        {
            _nowStyle = settingData.Get_value_digital();
        }

        return _nowStyle;
    }

    GUIStyle SelectNameGUIStyle(RVSettingData settingData)
    {
        GUIStyle _nowStyle = settingData.Get_default();
        if (this.rvVisibility.RVType == RVVisibility.NameType.Field)
        {
            if (this.rvVisibility.IsPublic == true)
                _nowStyle = settingData.Get_name_public();
            else
                _nowStyle = settingData.Get_name_private();
        }
        else if (this.rvVisibility.RVType == RVVisibility.NameType.CollectionItem)
        {
            _nowStyle = settingData.Get_name_collection_item();
        }
        else if (this.rvVisibility.RVType == RVVisibility.NameType.Property)
        {
            _nowStyle = settingData.Get_name_property();
        }


        return _nowStyle;
    }

    string GetValueString(object data)
    {
        if (data == null)
            return "null";

        if (this.rvVisibility.ValueIsString() == true)
        {
            return "\"" + this.data.ToString() + "\"";
        }
        else if (typeof(Vector2).IsAssignableFrom(this.rvVisibility.ValueType) == true)
        {
            Vector2 v2 = (Vector2)System.Convert.ChangeType(data, typeof(Vector2));
            return "(" + v2.x.ToString() + ", " + v2.y.ToString() + ")";
        }
        else if (typeof(Vector3).IsAssignableFrom(this.rvVisibility.ValueType) == true)
        {
            Vector3 v3 = (Vector3)System.Convert.ChangeType(data, typeof(Vector3));
            return "(" + v3.x.ToString() + ", " + v3.y.ToString() + ", " + v3.z.ToString() + ")";
        }
        else if (typeof(Vector4).IsAssignableFrom(this.rvVisibility.ValueType) == true)
        {
            Vector4 v4 = (Vector4)System.Convert.ChangeType(data, typeof(Vector4));
            return "(" + v4.x.ToString() + ", " + v4.y.ToString() + ", " + v4.z.ToString() + ", " + v4.w.ToString() + ")";
        }
        else
        {
            return this.data.ToString();
        }
    }

    void FieldMenu(Rect rect, RVSettingData settingData, string name, string value)
    {
        if (rvInput != null)
            return;

        Event currentEvent = Event.current;
        Rect contextRect = new Rect(rect.x, rect.y, 1200, 16);
        if (isSelected == true)
            EditorGUI.DrawRect(contextRect, settingData.bgColor_selected);
        else
            EditorGUI.DrawRect(contextRect, new Color(0, 0, 0, 0));

        if (currentEvent.type == EventType.ContextClick)
        {
            Vector2 mousePos = currentEvent.mousePosition;
            if (contextRect.Contains(mousePos))
            {
                // create the menu, add items and show it
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Copy name"), false, delegate (object obj)
                {
                    EditorGUIUtility.systemCopyBuffer = obj.ToString();
                }, name);
                menu.AddItem(new GUIContent("Copy value"), false, delegate (object obj)
                {
                    EditorGUIUtility.systemCopyBuffer = obj.ToString();
                }, value);
                menu.AddItem(new GUIContent("Change value"), false, delegate (object obj)
                {
                    this.rvInput = new RVInput(this.rv, this.NameLabel, this.GetNamePath(), this.data, this.Parent, settingData, this.rvVisibility,
                        delegate ()
                        {
                            this.rvInput = null;
                        });
                }, value);
                menu.ShowAsContext();
                currentEvent.Use();
            }
        }
    }

    void DataEditorControl(RVSettingData settingData, string _valueStr, GUIStyle value_guiStyle)
    {
        if (this.rvVisibility.ValueIsString() == true)
        {
            string str = "";
            if (this.data != null)
                str = this.data.ToString();
            this.data = EditorGUILayout.TextField(str, GUILayout.MaxWidth(180), GUILayout.MinWidth(10));
        }
        else
        {
            EditorGUILayout.IntField(10, GUILayout.MaxWidth(180), GUILayout.MinWidth(10));
        }

        Rect rect = GUILayoutUtility.GetLastRect();
        rect.y -= 1;
        rect.x += rect.width + 5;
        rect.width = 43;
        rect.height = 17;
        if (GUILayout.Button("Apply", GUILayout.Width(43)))
        {
            this.rvInput = null;
            //this.isInChangeValue = false;
        }
        // EditorGUILayout.fie
        //else if

    }

    public static void RightClickMenu(Rect rect, float width, float height, RVSettingData settingData, string menuName, GenericMenu.MenuFunction2 onClick, string text, bool isSelected)
    {
        Event currentEvent = Event.current;
        Rect contextRect = new Rect(rect.x, rect.y, width, height);
        if (isSelected == true)
            EditorGUI.DrawRect(contextRect, settingData.bgColor_selected);
        else
            EditorGUI.DrawRect(contextRect, new Color(0, 0, 0, 0));

        if (currentEvent.type == EventType.ContextClick)
        { 
            Vector2 mousePos = currentEvent.mousePosition;
            if (contextRect.Contains(mousePos))
            {
                // Now create the menu, add items and show it
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(menuName), false, onClick, text);
                menu.ShowAsContext();
                currentEvent.Use();
            }
        }
    }
     
    public static void OnMenuClick_Copy(object obj)
    {
        EditorGUIUtility.systemCopyBuffer = obj.ToString();
    }
}

public class RVVisibility
{
    public enum NameType
    {
        None,
        Field,
        Property,
        CollectionItem,
        Class,
    }
    
    public NameType RVType = RVVisibility.NameType.None;

    public Type ValueType { get; private set; }
    FieldInfo fieldInfo;
    PropertyInfo propertyInfo;
    public CollectionItemInfo CollectionItemInfo { get; private set; }//NameType.CollectionItem
    public bool IsPublic { get { return this.fieldInfo.IsPublic; } }
    public bool IsPrivate { get { return this.fieldInfo.IsPrivate; } }
    public bool PropertyCanRead { get { return this.propertyInfo.CanRead; } }
    public bool PropertyCanWrite { get { return this.propertyInfo.CanWrite; } }

    /// <summary>
    /// この値の親クラス,CollectionItemの場合はParentDataはそのCollection
    /// </summary>
    public object ParentData { get; private set; }

    public RVVisibility(Type v)
    {
        this.ValueType = v;
    }

    public RVVisibility(NameType n, Type v)
    {
        this.RVType = n;
        this.ValueType = v;
    }

    public RVVisibility(CollectionItemInfo collectionItemInfo, Type v, object parentData)
    {
        this.CollectionItemInfo = collectionItemInfo;
        this.ParentData = parentData;
        this.RVType =  NameType.CollectionItem;
        this.ValueType = v;
    }

    public RVVisibility(FieldInfo fieldInfo, object parentData)
    {
        this.ParentData = parentData;
        this.fieldInfo = fieldInfo;
        this.RVType = RVVisibility.NameType.Field;
        this.ValueType = this.fieldInfo.FieldType;
    }

    public RVVisibility(PropertyInfo propertyInfo, object parentData)
    {
        this.ParentData = parentData;
        this.propertyInfo = propertyInfo;
        this.RVType = RVVisibility.NameType.Property;
        this.ValueType = this.propertyInfo.PropertyType;
    }

    public bool ValueIsString()
    {
        if (ValueType == null)
            return false;
        return RVHelper.IsString(ValueType);
    }

    //nullかないの数　int,floatなと
    public bool ValueIsNumeric()
    {
        if (ValueType == null)
            return false;

        return RVHelper.IsNumeric(ValueType);
    }

    public bool ValueIsFloat()
    {
        if (ValueType == null)
            return false;

        return ValueType == typeof(float);
    }

    public bool ValueIsDouble()
    {
        if (ValueType == null)
            return false;
        return ValueType == typeof(double);
    }

    public bool ValueIsLong()
    {
        if (ValueType == null)
            return false;
        return ValueType == typeof(long);
    }

    public bool ValueIsEnum()
    {
        if (ValueType == null)
            return false;
        return ValueType.IsEnum;
    }

    public bool ValueIsBool()
    {
        if (ValueType == null)
            return false;
        return ValueType == typeof(bool);
    }

    public bool ValueIsVector()
    {
        if (ValueType == null)
            return false;
        return typeof(Vector2).IsAssignableFrom(ValueType) == true ||
            typeof(Vector3).IsAssignableFrom(ValueType) == true ||
            typeof(Vector4).IsAssignableFrom(ValueType) == true;
    }

    public bool ValueIsColor()
    {
        if (ValueType == null)
            return false;
        return typeof(Color).IsAssignableFrom(ValueType) == true ||
            typeof(Color32).IsAssignableFrom(ValueType) == true;
    }

    public RVVisibility GetCopy()
    {
        return this.MemberwiseClone() as RVVisibility;
    }

    public void FieldInfoSetValue(object parent, object newItemValue)
    {
      //  Type _type = parent.GetType();
       // bool isStruct = _type.IsValueType && !_type.IsPrimitive;

        //if (isStruct == true)
        //{
        //    fieldInfo.SetValueDirect(__makeref(parent), newItemValue);
        //}
        //else
        //{
            fieldInfo.SetValue(parent, newItemValue);
    //    }
    }

    public void PropertyInfoSetValue(object parent, object newItemValue)
    {
        Type _type = parent.GetType();
        bool isStruct = _type.IsValueType && !_type.IsPrimitive;

        if (isStruct == true)
        {
            RVInput.ErrorLog("not support struct type...", null);
        }
        else
        {
            propertyInfo.SetValue(parent, newItemValue, null);
        }
    }
}


public class CollectionItemInfo
{
    public enum CIType
    {
        ICollectionItem, //listなとの
        IDictionaryItemKey,
        IDictionaryItemValue,
    }

    public CIType ItemType;

    //listなとの
    public int ICollectionItemIndex = -1;
    //dicなとの
    public object IDictionaryKey = null;

    public CollectionItemInfo(CIType t, object iDictionaryKey = null, int iCollectionItemIndex = -1)
    {
        ItemType = t;
        ICollectionItemIndex = iCollectionItemIndex;
        IDictionaryKey = iDictionaryKey;
    }

    public void SetValue(object obj, object value)
    {
        if (ItemType == CIType.ICollectionItem)
        {
            SetValue_Collection(obj, value);
        }
        else
        {
            SetValue_Dictionary(obj, value);
        }
    }

    void SetValue_Collection(object obj, object value)
    {
        if (obj is IList)
        {
            IList list = obj as IList;
            list[ICollectionItemIndex] = value;
        }
        else if (obj is Array)
        {
            Array list = obj as Array;
            list.SetValue(value, ICollectionItemIndex);
        }
        else
        {
            RVInput.ErrorLog("not support type '" + RVHelper.GetTypeName(obj.GetType()) + "' ...", null);
        }
    }

    void SetValue_Dictionary(object obj, object newValue)
    {
        if (obj is IDictionary == false)
        {
            RVInput.ErrorLog("not support type '" + RVHelper.GetTypeName(obj.GetType()) + "' ...", null);
            return;
        }

        IDictionary dic = obj as IDictionary;

        if (ItemType == CIType.IDictionaryItemKey)
        {
            if (dic.Contains(newValue))
            {
                RVInput.ErrorLog("An element with the same key already exists in the dictionary ...", null);
                return;
            }

            if (dic.Contains(IDictionaryKey))
            {
                object _value = dic[IDictionaryKey];
                dic.Remove(IDictionaryKey);
                dic.Add(newValue, _value);
            }
        }
        else if (ItemType == CIType.IDictionaryItemValue)
        {
            if (dic.Contains(IDictionaryKey))
            {
                dic[IDictionaryKey] = newValue;
            }
        }
        else
        {
            RVInput.ErrorLog("not support type '" + RVHelper.GetTypeName(obj.GetType()) + "' ...", null);
        }
    }
}
