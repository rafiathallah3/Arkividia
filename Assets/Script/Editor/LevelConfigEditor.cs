using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelConfig))]
public class LevelConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty hasIntroDialogueProp = serializedObject.FindProperty("hasIntroDialogue");
        SerializedProperty introDialogueTextProp = serializedObject.FindProperty("introDialogueText");
        SerializedProperty introEventsProp = serializedObject.FindProperty("introEvents");

        // Intro Dialogue Section
        if (hasIntroDialogueProp != null)
        {
            EditorGUILayout.PropertyField(hasIntroDialogueProp);

            if (hasIntroDialogueProp.boolValue && introDialogueTextProp != null)
            {
                EditorGUILayout.PropertyField(introDialogueTextProp);
            }
        }

        // Intro Events Section
        EditorGUILayout.Space();
        if (introEventsProp != null)
        {
            EditorGUILayout.PropertyField(introEventsProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
