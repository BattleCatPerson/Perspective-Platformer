using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetTags))]
[CanEditMultipleObjects]
public class SetTagsEditor : Editor
{
    SerializedProperty tag;
    SerializedProperty material;
    private void OnEnable()
    {
        tag = serializedObject.FindProperty("tagName");
        material = serializedObject.FindProperty("material");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(tag);
        EditorGUILayout.PropertyField(material);


        SetTags s = (SetTags)target;
        if (GUILayout.Button("Assign"))
        {
            s.Assign();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
