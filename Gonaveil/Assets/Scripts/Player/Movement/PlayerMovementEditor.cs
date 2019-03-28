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
    SerializedProperty friction;
    SerializedProperty frictionTime;
    SerializedProperty stepSlope;

    SerializedProperty airAcceleration;
    SerializedProperty airDrag;
    SerializedProperty maxAirVelocity;
    SerializedProperty fallSpeedMultiplier;
    SerializedProperty fallMaxSpeedUp;
    SerializedProperty flipTime;

    SerializedProperty jumpHeight;
    SerializedProperty jumpLateralSpeedMultiplier;
    SerializedProperty autoJumping;
    SerializedProperty queueJumping;
    SerializedProperty jumpCooldown;
    SerializedProperty upHillJumpBoost;

    SerializedProperty surfSlope;
    SerializedProperty surfAcceleration;

    SerializedProperty crouchHeight;
    SerializedProperty crouchTime;
    SerializedProperty crouchVelocity;
    SerializedProperty crouchCameraHeight;

    SerializedProperty slideBoost;
    SerializedProperty slideVelocity;

    void OnEnable() {
        cameraTransform = serializedObject.FindProperty("cameraTransform");
        cameraHeight = serializedObject.FindProperty("cameraHeight");

        maxVelocity = serializedObject.FindProperty("maxVelocity");
        acceleration = serializedObject.FindProperty("acceleration");
        friction = serializedObject.FindProperty("friction");
        frictionTime = serializedObject.FindProperty("frictionTime");
        stepSlope = serializedObject.FindProperty("stepSlope");

        airAcceleration = serializedObject.FindProperty("airAcceleration");
        airDrag = serializedObject.FindProperty("airDrag");
        maxAirVelocity = serializedObject.FindProperty("maxAirVelocity");
        fallSpeedMultiplier = serializedObject.FindProperty("fallSpeedMultiplier");
        fallMaxSpeedUp = serializedObject.FindProperty("fallMaxSpeedUp");
        flipTime = serializedObject.FindProperty("flipTime");

        jumpHeight = serializedObject.FindProperty("jumpHeight");
        jumpLateralSpeedMultiplier = serializedObject.FindProperty("jumpLateralSpeedMultiplier");
        autoJumping = serializedObject.FindProperty("autoJumping");
        queueJumping = serializedObject.FindProperty("queueJumping");
        jumpCooldown = serializedObject.FindProperty("jumpCooldown");
        upHillJumpBoost = serializedObject.FindProperty("upHillJumpBoost");

        surfSlope = serializedObject.FindProperty("surfSlope");
        surfAcceleration = serializedObject.FindProperty("surfAcceleration");

        crouchHeight = serializedObject.FindProperty("crouchHeight");
        crouchTime = serializedObject.FindProperty("crouchTime");
        crouchVelocity = serializedObject.FindProperty("crouchVelocity");
        crouchCameraHeight = serializedObject.FindProperty("crouchCameraHeight");

        slideBoost = serializedObject.FindProperty("slideBoost");
        slideVelocity = serializedObject.FindProperty("slideVelocityThreshold");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.LabelField("Camera", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(cameraTransform);
        EditorGUILayout.PropertyField(cameraHeight);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Grounded Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(maxVelocity);
        EditorGUILayout.PropertyField(acceleration);
        EditorGUILayout.PropertyField(friction);
        EditorGUILayout.PropertyField(frictionTime);
        EditorGUILayout.PropertyField(stepSlope);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Crouching", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(crouchHeight);
        EditorGUILayout.PropertyField(crouchTime);
        EditorGUILayout.PropertyField(crouchVelocity);
        EditorGUILayout.PropertyField(crouchCameraHeight);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Sliding", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(slideBoost);
        EditorGUILayout.PropertyField(slideVelocity);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Air Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(airAcceleration);
        EditorGUILayout.PropertyField(maxAirVelocity);
        EditorGUILayout.PropertyField(airDrag);
        EditorGUILayout.PropertyField(fallSpeedMultiplier);
        EditorGUILayout.PropertyField(fallMaxSpeedUp);
        EditorGUILayout.PropertyField(flipTime);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Jump Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(jumpHeight);
        EditorGUILayout.PropertyField(jumpLateralSpeedMultiplier);
        EditorGUILayout.PropertyField(autoJumping);
        EditorGUILayout.PropertyField(queueJumping);
        EditorGUILayout.PropertyField(jumpCooldown);
        EditorGUILayout.PropertyField(upHillJumpBoost);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Surf Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(surfSlope);
        EditorGUILayout.PropertyField(surfAcceleration);


        serializedObject.ApplyModifiedProperties();
    }
}
#endif