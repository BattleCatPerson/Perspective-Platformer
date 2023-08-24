using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetTags))]
[CanEditMultipleObjects]
public class SetTagsEditor : Editor
{
    SerializedProperty floors;
    SerializedProperty tag;

    private void OnEnable()
    {
        tag = serializedObject.FindProperty("tagName");
        floors = serializedObject.FindProperty("floors");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(tag);

        SetTags s = (SetTags)target;
        if (GUILayout.Button("Assign Tags"))
        {
            s.Assign();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
