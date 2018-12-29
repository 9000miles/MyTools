using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

public class RVInputCollection
{
    object data;
    RVNoParameterDel onClose;

    public RVInputCollection(RuntimeViewer rv, string nameLabel, string namePath, object data, RVControlBase parent, RVSettingData settingData, RVVisibility rvVisibility,
       RVNoParameterDel onClose)
    {
        this.data = data;
        this.onClose = onClose;
    }

    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        Type _type = data.GetType();
        if (typeof(IDictionary).IsAssignableFrom(_type) == true)//是个字典
        {
            AddItem_Dictionary(_type);
        }
        else if(typeof(ArrayList).IsAssignableFrom(_type) == true)
        {
            AddItem_ArrayList(_type);
        }
        else if (typeof(IList).IsAssignableFrom(_type) == true) //是个集合
        {
            AddItem_List(_type);
        }
        else if (_type == typeof(Array)) //是个数组
        {
            AddItem_Array(_type);
        }
        else
        {
            EditorGUILayout.LabelField("   not support this type yet ... ", GUILayout.Width(206));
        }

        if (GUILayout.Button("Cancel", GUILayout.Width(56)))
        {
            onClose();
        }

        EditorGUILayout.EndHorizontal();

    }

    void OnSetValue(object newValue, RVVisibility rvVisibility)
    {
        if (rvVisibility == null)
        {
            ErrorLog("RVVisibility == null ...", null);
            return;
        }
    }

    void AddItem_List(Type _type)
    {
        EditorGUILayout.LabelField("   add one item with default value ? ", GUILayout.Width(206));

        if (GUILayout.Button("Add", GUILayout.Width(66)))
        {
            IList list = (IList)this.data;
            Type itemType = RVHelper.GetCollectionItemType(list);
            if (itemType != null)
                list.Add(RVHelper.DefaultForType(itemType));

            onClose();
        }
    }

    void AddItem_Dictionary(Type _type)
    {
        EditorGUILayout.LabelField("   add one item, input key : ", GUILayout.Width(206));

        IDictionary dic = (IDictionary)this.data;

        Type keyType = RVHelper.GetDictionaryKeyType(dic);
        RVVisibility rvv = new RVVisibility(keyType);
        object newKey = null;
        RVInput.Intput_Value(ref newKey, rvv);
        if (GUILayout.Button("Add", GUILayout.Width(66)))
        {
            newKey = Convert.ChangeType(newKey, keyType);
            Type valueType = RVHelper.GetDictionaryValueType(dic);
            if (newKey != null)
            {
                dic.Add(newKey, RVHelper.DefaultForType(valueType));
            }
        }
    }

    void AddItem_Array(Type _type)
    {

    }

    void AddItem_ArrayList(Type _type)
    {
        EditorGUILayout.LabelField("   add one item with default value ? ", GUILayout.Width(206));

        if (GUILayout.Button("Add", GUILayout.Width(66)))
        {
            ArrayList list = (ArrayList)this.data;
            list.Add(null);

            onClose();
        }
    }

    void ErrorLog(string str, Exception e)
    {
        if (e != null)
            Debug.LogError("RuntimeViewer ChangeCollection Failure : " + str + " \n| error: " + e.Message + " |  " + e.StackTrace);
        else
            Debug.LogError("RuntimeViewer ChangeCollection Failure : " + str + "\n");
    }
}
