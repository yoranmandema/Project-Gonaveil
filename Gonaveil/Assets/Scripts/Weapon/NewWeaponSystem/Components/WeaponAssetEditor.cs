#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[CustomEditor(typeof(WeaponAsset))]
[CanEditMultipleObjects]
public class WeaponAssetEditor : Editor {
    private string[] availableTypes;
    private int selectedPrimary;
    private int selectedSecondary;

    public SerializedProperty weaponMovementProfile;
    public SerializedProperty viewModel;
    public SerializedProperty worldModel;
    public SerializedProperty offset;

    void OnEnable() {
        var types = Assembly.GetAssembly(typeof(WeaponComponent))
            .GetTypes()
            .Where(t => t != typeof(WeaponComponent) && typeof(WeaponComponent).IsAssignableFrom(t));

        var typeNameList = new List<string>() { "None" };

        foreach (var t in types) {
            typeNameList.Add(t.Name);
        }

        availableTypes = typeNameList.ToArray();

        var asset = (WeaponAsset)target;

        selectedPrimary = asset.primaryComponent != null ? typeNameList.IndexOf(asset.primaryComponent.Name) : 0;
        selectedSecondary = asset.secondaryComponent != null ? typeNameList.IndexOf(asset.secondaryComponent.Name) : 0;

        weaponMovementProfile = serializedObject.FindProperty("weaponMovementProfile");
        viewModel = serializedObject.FindProperty("viewModel");
        worldModel = serializedObject.FindProperty("worldModel");
        offset = serializedObject.FindProperty("offset");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.LabelField("Fire Components", EditorStyles.boldLabel);

        selectedPrimary = EditorGUILayout.Popup("Label", selectedPrimary, availableTypes);
        selectedSecondary = EditorGUILayout.Popup("Label", selectedSecondary, availableTypes);

        var asset = (WeaponAsset)target;

        asset.primaryComponent = selectedPrimary > 0 ? Type.GetType(availableTypes[selectedPrimary]) : null;
        asset.primaryComponent = selectedSecondary > 0 ? Type.GetType(availableTypes[selectedSecondary]) : null;

        EditorGUILayout.LabelField("Models", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(viewModel);
        EditorGUILayout.PropertyField(worldModel);

        EditorGUILayout.LabelField("Offset", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(offset);

        //Debug.Log(asset.primaryComponent);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif