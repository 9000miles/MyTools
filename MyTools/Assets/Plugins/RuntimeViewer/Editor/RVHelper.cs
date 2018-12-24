using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

public delegate void RVNoParameterDel();


public static class RVHelper
{

    public static Component[] GetComponent(GameObject data)
    {
        Component[] c = data.GetComponents<Component>();

        if (c == null || c.Length == 0)
            return null;

        return c;
    }

    public static bool IsNormalType(Type t)
    {
        if (t == null)
            return false;

        if (typeof(Enum).IsAssignableFrom(t) == true)
            return true;

        if (t.IsValueType == true && t.IsPrimitive == true)
        {
            return true;
        }

        return false;
    }

    public static bool IsString(Type t)
    {
        if (typeof(string).IsAssignableFrom(t) == true)
            return true;

        if (typeof(String).IsAssignableFrom(t) == true)
            return true;

        return false;
    }

    public static bool IsEnum(Type t)
    {
        return typeof(Enum).IsAssignableFrom(t);
    }

    //是否是个字典 or 集合
    public static bool IsCollection(Type type)
    {
        if (typeof(Enum).IsAssignableFrom(type) == true)
            return false;

        if (typeof(IDictionary).IsAssignableFrom(type) == true ||
            typeof(ICollection).IsAssignableFrom(type) == true)
        {
            return true;
        }
        return false;
    }

    public static bool IsCanToStringDirently(Type t)
    {
        if (RVHelper.IsNormalType(t) == true ||
            RVHelper.IsString(t) == true ||
            typeof(Vector2).IsAssignableFrom(t) == true ||
            typeof(Color).IsAssignableFrom(t) == true ||
            typeof(Color32).IsAssignableFrom(t) == true ||
            typeof(Vector4).IsAssignableFrom(t) == true ||
            typeof(Quaternion).IsAssignableFrom(t) == true ||
            typeof(Vector3).IsAssignableFrom(t) == true ||
             typeof(Type).IsAssignableFrom(t) == true)
            return true;

        return false;
    }

    public static bool IsUnityClass(Type t)
    {
        if (typeof(UnityEngine.Object).IsAssignableFrom(t) == true ||
            typeof(MonoBehaviour).IsAssignableFrom(t) == true)
            return true;

        return false;
    }

    public static string GetClearNameString(string str)
    {
        string _str = RemoveBehindChar(str, '-');
        _str = RemoveBehindChar(_str, ':');
        return _str;
    }

    static string RemoveBehindChar(string str, char _char)
    {
        string[] strs = str.Split(_char);
        if (strs.Length > 0)
            return strs[0];

        return str;
    }

    // Extension method, call for any object, eg "if (x.IsNumeric())..."
    public static bool IsNumeric(this object x) { return (x == null ? false : IsNumeric(x.GetType())); }

    // Method where you know the type of the object
    public static bool IsNumeric(Type type) { return IsNumeric(type, Type.GetTypeCode(type)); }

    // Method where you know the type and the type code of the object
    public static bool IsNumeric(Type type, TypeCode typeCode) { return (typeCode == TypeCode.Decimal || (type.IsPrimitive && typeCode != TypeCode.Object && typeCode != TypeCode.Boolean && typeCode != TypeCode.Char)); }

    public static Type GetCollectionItemType(ICollection iCollection)
    {
        Type[] ts = iCollection.GetType().GetGenericArguments();
        if (ts.Length > 0)
        {
            return ts[0];
        }

        return null;
    }

    public static object DefaultForType(Type targetType)
    {
        return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
    }

    public static Type GetDictionaryKeyType(IDictionary dic)
    {
        Type[] ts = dic.GetType().GetGenericArguments();
        if (ts.Length ==2)
        {
            return ts[0];
        }

        return null;
    }

    public static Type GetDictionaryValueType(IDictionary dic)
    {
        Type[] ts = dic.GetType().GetGenericArguments();
        if (ts.Length == 2)
        {
            return ts[1];
        }

        return null;
    }

    public static string GetTypeName(Type t)
    {
        if (!t.IsGenericType) return t.Name;
        if (t.IsNested && t.DeclaringType.IsGenericType)
        {
            return "no type name";
            //throw new NotImplementedException();
        }
        string txt = t.Name.Substring(0, t.Name.IndexOf('`')) + "<";
        int cnt = 0;
        foreach (Type arg in t.GetGenericArguments())
        {
            if (cnt > 0) txt += ", ";
            txt += GetTypeName(arg);
            cnt++;
        }
        return txt + ">";
    }

    public static Type GetTypeCollectionItem(object collection, object itemValue, bool isDicKey = false)
    {
        if (IsNull(itemValue) == false)
            return itemValue.GetType();

        if (collection is IDictionary)
        {
            Type[] ts = collection.GetType().GetGenericArguments();
            if(isDicKey == true && ts.Length > 0)
            {
                return ts[0];
            }
            else if (ts.Length > 1)
            {
                return ts[1];
            }
        }
        else if (collection is ICollection)
        {
            Type[] ts = collection.GetType().GetGenericArguments();
            if (ts.Length > 0)
                return ts[0];
        }
        return null;
    }

    public static bool IsNull(object ob)
    {
        if (ob == null)
        {
            return true;
        }
        else if (ob is UnityEngine.Object && (ob as UnityEngine.Object) == null)
        {
            return true;
        }

        return false;
    }
}
