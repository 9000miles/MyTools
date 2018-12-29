using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

public class RVCollection : RVControlBase
{
    Dictionary<string, RVControlBase> children = new Dictionary<string, RVControlBase>();
    bool isFirstOpen = true;

    public RVCollection(RuntimeViewer rv , string UID, string nameLabel, object data, int depth, RVVisibility rvVisibility, RVControlBase parent)
         : base(rv, UID, nameLabel,data, depth, rvVisibility, parent)
    {
    }

    bool IsFold()
    {
        if (rvcStatus.IsOpens.ContainsKey(this.UID) == false)
            rvcStatus.IsOpens.Add(this.UID, false);

        rvcStatus.IsOpens[this.UID] = CollectionUI(rvcStatus.IsOpens[this.UID], settingData);
        return rvcStatus.IsOpens[this.UID];
    }

    public override void OnGUIUpdate(bool isRealtimeUpdate, RVSettingData settingData, RVCStatus rvcStatus)
    {
        base.OnGUIUpdate(isRealtimeUpdate, settingData, rvcStatus);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("     ", GUILayout.Width(depth * RVControlBase.Indent_class + IndentPlus));
        EditorGUILayout.BeginVertical();

        if (IsFold() == true)
        {

            if (isFirstOpen == true)
            {
                AnalyzeAndCreateChildren();
                isFirstOpen = false;
            }
            else if (isRealtimeUpdate == true)
            {
                AnalyzeAndUpdateChildren();
            }

            foreach (var item in children)
            {
                item.Value.OnGUIUpdate(isRealtimeUpdate, settingData, rvcStatus);
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    public override void OnGUIUpdate_Late()
    {
        base.OnGUIUpdate_Late();

        if (rvcStatus.IsOpens.ContainsKey(this.UID) == false)
            rvcStatus.IsOpens.Add(this.UID, false);

        if (rvcStatus.IsOpens[this.UID] == false)
            return;

        foreach (var item in children)
        {
            item.Value.OnGUIUpdate_Late();
        }
    }

    public override void OnDestroy()
    {
        foreach (var item in children)
        {
            //item.Value.OnDestroy();
            item.Value.OnDestroy();
        }
        children.Clear();
    }

    void AnalyzeAndCreateChildren()
    {
        OnDestroy();
        if (data == null)
        {
            return;
        }

        Type t = data.GetType();

        if (RVHelper.IsCollection(t) == true)
        {
            children= AnalyzeCollection(data, t);
        }
        else
        {
            children = AnalyzeClass(data, t);
        }
    }

    void AnalyzeAndUpdateChildren()
    {
        if (data == null)
        {
            OnDestroy();
            return;
        }

        Type t = data.GetType();

        Dictionary<string, RVControlBase> newChildren = new Dictionary<string, RVControlBase>();
        if (RVHelper.IsCollection(t) == true)
        {
            newChildren = AnalyzeCollection(data, t);
        }
        else
        {
            newChildren = AnalyzeClass(data, t);
        }

        foreach (var newItem in newChildren)
        {
            if(this.children.ContainsKey(newItem.Key) == true)
            {//update
                this.children[newItem.Key].UpdateData(newItem.Value);
            }
            else
            {
                this.children.Add(newItem.Key, newItem.Value);
            }
        }
    }

    Dictionary<string, RVControlBase> CreateDictionaryItemControl(DictionaryEntry item,object dic, int collectionNum = -1)
    {
        Dictionary<string, RVControlBase> dicItems = new Dictionary<string, RVControlBase>();
        Type _typeKey = RVHelper.GetTypeCollectionItem(dic, item.Key, true);

        CollectionItemInfo cIInfo_key = new CollectionItemInfo(CollectionItemInfo.CIType.IDictionaryItemKey, item.Key);
        RVVisibility rvvKey = new RVVisibility(cIInfo_key, _typeKey, dic);

        CollectionItemInfo cIInfo_value= new CollectionItemInfo(CollectionItemInfo.CIType.IDictionaryItemValue, item.Key);
        Type _typeValue = RVHelper.GetTypeCollectionItem(dic, item.Value, false);
        RVVisibility rvvValue = new RVVisibility(cIInfo_value, _typeValue,dic);
         
        RVControlBase key = CreateControl(item.Key, "┌ key  ┐", rvvKey, collectionNum);
        dicItems.Add(key.UID,key);
        RVControlBase _value= CreateControl(item.Value, "└value┘", rvvValue, collectionNum);
        dicItems.Add(_value.UID,_value);

        //对齐
        if(key is RVCollection  == true && _value is RVCollection == false)
        {
            _value.IndentPlus = 14;
        }
        else if (_value is RVCollection == true && key is RVCollection == false)
        {
            key.IndentPlus = 14;
        }

        return dicItems;
    }

    RVControlBase CreateControl(object ob, string fieldName, RVVisibility rvVisibility, int collectionNum = -1)
    {
        RVControlBase b = null;
        string nextUID = this.UID + " - " + fieldName;
        if (collectionNum >= 0)
            nextUID = this.UID + " - " + fieldName + "[" + collectionNum + "]";

        if (RVHelper.IsNull(ob) == true)
        {
            b = new RVText(this.rv, nextUID, fieldName, null, depth + 1, rvVisibility, this);
        }
        else
        {
            Type t = ob.GetType();
            if (RVHelper.IsCanToStringDirently(t) == true)
            {
                b = new RVText(this.rv, nextUID, fieldName, ob, depth + 1, rvVisibility, this);
            }
            else
            {
                RVVisibility rvv = rvVisibility.GetCopy();
                if (RVHelper.IsCollection(t) == false)
                    rvv.RVType = RVVisibility.NameType.Class;
                b = new RVCollection(this.rv, nextUID, GetSpecialNameLabel(ob, fieldName), ob, depth + 1, rvv, this);
            }
        }

        return b;
    }

    bool IsForbidThis(object obj, string fieldName)
    {
        if (obj == null)
            return false;

        if (Application.isPlaying == false)
        {
            if (obj is MeshFilter)
                return true;
            if (obj is Renderer)
                return true;
            if (obj is Collider)
                return true;
        }

        if (RuntimeViewer.IsEnableForbidNames == true)
        {
            if (this.settingData.IsForbid(fieldName) == true)
                return true;
        }
        return false;
    }

    string GetSpecialNameLabel(object ob, string fieldName)
    {
        if (ob == null)
            return fieldName;

        Type obType = ob.GetType();
        if (RVHelper.IsCollection(obType) == true)
        {
            fieldName += " - " + RVHelper.GetTypeName(obType);
            return fieldName;
        }

        if (ob is UnityEngine.Object && (ob as UnityEngine.Object) != null)
        {
            fieldName += " : '" + (ob as UnityEngine.Object).name+"'";
        }

        return fieldName;
    }

    //所有集合判断,数组,字典,list等等
    Dictionary<string, RVControlBase> AnalyzeCollection(object ob, Type type)
    {
        Dictionary<string, RVControlBase> result = new Dictionary<string, RVControlBase>();
        int index = 0;

        if (typeof(IDictionary).IsAssignableFrom(type) == true)//是个字典
        {
            IDictionary dic = ob as IDictionary;
            foreach (DictionaryEntry item in dic)
            {
                foreach (KeyValuePair<string, RVControlBase> item2 in CreateDictionaryItemControl(item, ob, index))
                {
                    result.Add(item2.Key, item2.Value);
                }
                index++;
            }
        }
        else if (typeof(ICollection).IsAssignableFrom(type) == true) //是个集合
        {
            foreach (var _v in (ICollection)ob)
            {
                string itemName = "["+index+"]";

                Type _type = RVHelper.GetTypeCollectionItem(ob, _v);

                CollectionItemInfo cIInfo = new CollectionItemInfo(CollectionItemInfo.CIType.ICollectionItem, null, index);
                RVVisibility rvv = new RVVisibility(cIInfo, _type, ob);
                RVControlBase rvc = CreateControl(_v, itemName, rvv, result.Count);
                index++;
                result.Add(rvc.UID,rvc);
            }
        }

        return result;
    }

    Dictionary<string, RVControlBase> AnalyzeClass(object data, Type thisType)
    {
        Dictionary<string, RVControlBase> result = new Dictionary<string, RVControlBase>();

        while (thisType.IsSubclassOf(typeof(object)))
        {
            FieldInfo[] fields = thisType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                if (IsForbidThis(data, field.Name) == true)
                {
                    continue;
                }
                object value = field.GetValue(data);

                RVVisibility rvv = new RVVisibility(field, data);

                RVControlBase cb = CreateControl(value, field.Name, rvv, -1);
                if (result.ContainsKey(cb.UID) == false)
                    result.Add(cb.UID, cb);
            }

            PropertyInfo[] properties = thisType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (PropertyInfo property in properties)
            {
                object value = null;
                try
                {
                    if (IsForbidThis(data, property.Name) == true)
                    {
                        continue;
                    }
                    value = property.GetValue(data, null);
                }
                catch
                {
                    value = null;
                }

                RVVisibility rvv = new RVVisibility(property, data);

                RVControlBase cb = CreateControl(value, property.Name, rvv,-1);

                if (result.ContainsKey(cb.UID) == false)
                    result.Add(cb.UID,cb);
            }

            thisType = thisType.BaseType;
        } 

        return result;
    }

    GUIStyle GetCollectionGUIStyle(RVSettingData settingData)
    {
        if (this.rvVisibility.RVType == RVVisibility.NameType.Class)
            return settingData.Get_name_class(EditorStyles.foldout);

        return settingData.Get_name_container(EditorStyles.foldout);
    }
     
    bool CollectionUI(bool isOpen, RVSettingData settingData)
    {
        bool isSelected = this.rvcStatus.IsSelected(this.NameLabel);

        GUIStyle guistyle = settingData.Get_name_container(EditorStyles.foldout);

        if (this.rvVisibility.RVType == RVVisibility.NameType.Class)
            guistyle = settingData.Get_name_class(EditorStyles.foldout);

        isOpen = EditorGUILayout.Foldout(isOpen, new GUIContent(this.NameLabel), guistyle);
        nameLabelRect = GUILayoutUtility.GetLastRect();
        Rect rect = GUILayoutUtility.GetLastRect();
        CollectionMenu(rect, settingData, isSelected);
        //RVText.RightClickMenu(rect, 1200, 16, settingData, "Copy", RVText.OnMenuClick_Copy, this.NameLabel, isSelected);
        //    rect.y += 16;
        //    RVText.RightClickMenu(rect, 1200, 16, settingData, "ChangeValue", RVText.OnMenuClick_Copy, this.NameLabel, isSelected);

        return isOpen;
    }
     
    void CollectionMenu(Rect rect, RVSettingData settingData, bool isSelected)
    {
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
                // Now create the menu, add items and show it
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Copy"), false, RVText.OnMenuClick_Copy, this.NameLabel);
                //menu.AddItem(new GUIContent("Add Item"), false, delegate(object obj) 
                //{
                //    this.rvInputCollection = new RVInputCollection(this.rv, this.NameLabel, this.GetNamePath(), this.data, this.Parent, settingData, this.rvVisibility,
                //    delegate ()
                //    {
                //        this.rvInputCollection = null;
                //    });
                //}, this.NameLabel);


                menu.ShowAsContext();
                currentEvent.Use();
            }
        }
    }
}

 