using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
    }
}

[CustomPropertyDrawer(typeof(TypeToString))]
public class MapObjectAttributeDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (attribute is TypeToString attr)
        {
            if (property.type == "string")
            {
                GUIContent[] TypeNames = attr.types.Select(type => new GUIContent(type.Name)).ToArray();
                string stringValue = property.stringValue;
                int index = -1;
                index = Array.FindIndex(attr.types, type => type.Name == stringValue);

                index = EditorGUI.Popup(position, label, index, TypeNames);
                if (index >= 0)
                {
                    property.stringValue = attr.types[index].Name;
                }
            }else{
                EditorGUI.LabelField(position, label, new GUIContent("只能用于string的字段"));
            }
        }
    }
}