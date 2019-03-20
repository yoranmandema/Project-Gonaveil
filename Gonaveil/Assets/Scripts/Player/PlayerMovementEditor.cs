using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerMovement))]
[CanEditMultipleObjects]
public class PlayerMovementEditor : Editor {
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

    void OnEnable() {
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
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PrefixLabel("Grounded Movement");
        EditorGUILayout.PropertyField(maxVelocity);
        EditorGUILayout.PropertyField(acceleration);

        EditorGUILayout.PrefixLabel("Air Movement");
        EditorGUILayout.PropertyField(airAccelaration);
        EditorGUILayout.PropertyField(airDrag);
        EditorGUILayout.PropertyField(limitAirVelocity);
        EditorGUILayout.PropertyField(fallSpeedMultiplier);
        EditorGUILayout.PropertyField(fallMaxSpeedUp);

        EditorGUILayout.PrefixLabel("Jump Movement");
        EditorGUILayout.PropertyField(jumpHeight);
        EditorGUILayout.PropertyField(jumpLateralSpeedMultiplier);
        EditorGUILayout.PropertyField(autoJump);
        EditorGUILayout.PropertyField(jumpCooldown);
        EditorGUILayout.PropertyField(upHillJumpBoost);

        EditorGUILayout.PrefixLabel("Surf Movement");
        EditorGUILayout.PropertyField(surfSlope);

        serializedObject.ApplyModifiedProperties();
    }
}