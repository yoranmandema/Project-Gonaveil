#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerMovement))]
[CanEditMultipleObjects]
public class PlayerMovementEditor : Editor {
    SerializedProperty cameraTransform;
    SerializedProperty cameraHeight;

    SerializedProperty maxVelocity;
    SerializedProperty acceleration;

    SerializedProperty airAccelaration;
    SerializedProperty airDrag;

    SerializedProperty limitAirVelocity;
    SerializedProperty fallSpeedMultiplier;
    SerializedProperty fallMaxSpeedUp;

    SerializedProperty jumpHeight;
    SerializedProperty jumpLateralSpeedMultiplier;
    SerializedProperty autoJump;
    SerializedProperty jumpCooldown;
    SerializedProperty upHillJumpBoost;

    SerializedProperty surfSlope;

    SerializedProperty crouchHeight;
    SerializedProperty crouchTime;
    SerializedProperty crouchVelocity;
    SerializedProperty crouchCameraHeight;

    void OnEnable() {
        cameraTransform = serializedObject.FindProperty("cameraTransform");
        cameraHeight = serializedObject.FindProperty("cameraHeight");

        maxVelocity = serializedObject.FindProperty("maxVelocity");
        acceleration = serializedObject.FindProperty("acceleration");

        airAccelaration = serializedObject.FindProperty("airAccelaration");
        airDrag = serializedObject.FindProperty("airDrag");
        limitAirVelocity = serializedObject.FindProperty("limitAirVelocity");
        fallSpeedMultiplier = serializedObject.FindProperty("fallSpeedMultiplier");
        fallMaxSpeedUp = serializedObject.FindProperty("fallMaxSpeedUp");

        jumpHeight = serializedObject.FindProperty("jumpHeight");
        jumpLateralSpeedMultiplier = serializedObject.FindProperty("jumpLateralSpeedMultiplier");
        autoJump = serializedObject.FindProperty("autoJump");
        jumpCooldown = serializedObject.FindProperty("jumpCooldown");
        upHillJumpBoost = serializedObject.FindProperty("upHillJumpBoost");

        surfSlope = serializedObject.FindProperty("surfSlope");

        crouchHeight = serializedObject.FindProperty("crouchHeight");
        crouchTime = serializedObject.FindProperty("crouchTime");
        crouchVelocity = serializedObject.FindProperty("crouchVelocity");
        crouchCameraHeight = serializedObject.FindProperty("crouchCameraHeight");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.LabelField("Camera", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(cameraTransform);
        EditorGUILayout.PropertyField(cameraHeight);

        EditorGUILayout.LabelField("Grounded Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(maxVelocity);
        EditorGUILayout.PropertyField(acceleration);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Air Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(airAccelaration);
        EditorGUILayout.PropertyField(airDrag);
        EditorGUILayout.PropertyField(limitAirVelocity);
        EditorGUILayout.PropertyField(fallSpeedMultiplier);
        EditorGUILayout.PropertyField(fallMaxSpeedUp);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Jump Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(jumpHeight);
        EditorGUILayout.PropertyField(jumpLateralSpeedMultiplier);
        EditorGUILayout.PropertyField(autoJump);
        EditorGUILayout.PropertyField(jumpCooldown);
        EditorGUILayout.PropertyField(upHillJumpBoost);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Surf Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(surfSlope);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Crouch Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(crouchHeight);
        EditorGUILayout.PropertyField(crouchTime);
        EditorGUILayout.PropertyField(crouchVelocity);
        EditorGUILayout.PropertyField(crouchCameraHeight);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif