using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common
{
    [CustomPropertyDrawer(typeof(EnumFlags))]
    public class EnumFlagsEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }

    /// <summary>
    /// 枚举多选，在声明枚举的上方添加[EnumFlags]
    /// </summary>
    public class EnumFlags : PropertyAttribute
    {
    }
}