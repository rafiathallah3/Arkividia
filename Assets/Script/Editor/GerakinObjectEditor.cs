using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GerakinObject))]
public class GerakinObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GerakinObject script = (GerakinObject)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("direction"));

        if (script.direction == GerakinObject.MoveDirection.CUSTOM)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("customTargetPosition"));
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("distance"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));

        serializedObject.ApplyModifiedProperties();
    }
}
