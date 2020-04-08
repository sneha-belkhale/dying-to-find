using UnityEditor;
using UnityEngine;
using System;
    #if UNITY_EDITOR

[CustomPropertyDrawer(typeof(EnumArrayAttribute))]
public class EnumArrayDrawer : PropertyDrawer {
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		EnumArrayAttribute enumArrayAttribute = attribute as EnumArrayAttribute;
		EditorGUI.BeginProperty(position, label, property);
        int end = property.propertyPath.LastIndexOf(']');
        if (end == -1) {
            EditorGUI.HelpBox(position, string.Format("{0} is not an array element but has [EnumArray].", property.propertyPath), MessageType.Error);
        } else {
            int start = property.propertyPath.LastIndexOf('[')+1;
            int index = int.Parse(property.propertyPath.Substring(start, end - start));
            string name = Enum.GetName(enumArrayAttribute.Type, index);
            name = name != null? name : "<UNDEF>";
            EditorGUI.PropertyField(position, property, new GUIContent(name), true);
        }
         EditorGUI.EndProperty ();
	}

    public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
        return EditorGUI.GetPropertyHeight(property, property.isExpanded);
    }
}
    #endif
