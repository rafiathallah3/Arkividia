using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KamarKamera))]
public class KamarKameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        KamarKamera room = (KamarKamera)target;
        // Always show mode
        room.cameraMode = (CameraMode)EditorGUILayout.EnumPopup(
            "Camera Mode",
            room.cameraMode
        );

        EditorGUILayout.Space();

        if (room.cameraMode == CameraMode.Fixed)
        {
            EditorGUILayout.LabelField("Fixed Mode Settings", EditorStyles.boldLabel);
            room.cameraCenter = (Transform)EditorGUILayout.ObjectField(
                "Camera Position",
                room.cameraCenter,
                typeof(Transform),
                true
            );
        }
        else if (room.cameraMode == CameraMode.Follow)
        {
            EditorGUILayout.LabelField("Follow Mode Settings", EditorStyles.boldLabel);

            room.followMinPosition = EditorGUILayout.Vector3Field(
                "Min Position",
                room.followMinPosition
            );

            room.followMaxPosition = EditorGUILayout.Vector3Field(
                "Max Position",
                room.followMaxPosition
            );
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(room);
        }
    }
}
