using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    private Player myPlayer;
    private SerializedObject soTarget;

    [Header("Custom Physics Parameters")]
    private SerializedProperty moveSpeed;
    [Header("Jumping")]
    private SerializedProperty maxJumpHeight;
    private SerializedProperty doubleJumpHeightRatio;
    private SerializedProperty minJumpHeight;
    private SerializedProperty timeToJumpApex;
    private SerializedProperty fallGravityModifier;
    private SerializedProperty terminalVelocityY;

    [Header("Dash")]
    private SerializedProperty dashLength;
    private SerializedProperty dashApexTime;
    private SerializedProperty dashTime;
    private SerializedProperty dashAcceleration;
    private SerializedProperty dashAttack;


    [Header("Acceleration")]
    private SerializedProperty accelerationAirborne;
    private SerializedProperty accelerationGrounded;
    private SerializedProperty accelerationJump;
    private SerializedProperty accelerationWallKick;

    [Header("Wall Jumping")]
    private SerializedProperty wallStickTime;
    private SerializedProperty wallSlideSpeedMax;
    private SerializedProperty wallJumpClimb;
    private SerializedProperty wallJumpOff;
    private SerializedProperty wallLeap;

    [Header("VFX")]
    private SerializedProperty effectPos;
    private SerializedProperty dustEffect;
    private SerializedProperty fadeEffect;
    private SerializedProperty fadeInterval;
    private SerializedProperty jumpEffect;

    // [Header("Audio")]
    // private SerializedProperty swordSwingSFX;

    [Header("Essential Components")]
    private SerializedProperty pivotObject;
    private void OnEnable()
    {
        myPlayer = (Player)target;
        soTarget = new SerializedObject(target);

        moveSpeed = soTarget.FindProperty("moveSpeed");
        maxJumpHeight = soTarget.FindProperty("maxJumpHeight");
        doubleJumpHeightRatio = soTarget.FindProperty("doubleJumpHeightRatio");
        minJumpHeight = soTarget.FindProperty("minJumpHeight");
        timeToJumpApex = soTarget.FindProperty("timeToJumpApex");
        fallGravityModifier = soTarget.FindProperty("fallGravityModifier");
        terminalVelocityY = soTarget.FindProperty("terminalVelocityY");
        accelerationAirborne = soTarget.FindProperty("accelerationAirborne");
        accelerationGrounded = soTarget.FindProperty("accelerationGrounded");
        accelerationJump = soTarget.FindProperty("accelerationJump");
        accelerationWallKick = soTarget.FindProperty("accelerationWallKick");

        dashLength = soTarget.FindProperty("dashLength");
        dashApexTime = soTarget.FindProperty("dashApexTime");
        dashTime = soTarget.FindProperty("dashTime");
        dashAcceleration = soTarget.FindProperty("dashAcceleration");
        dashAttack = soTarget.FindProperty("dashAttack");


        wallStickTime = soTarget.FindProperty("wallStickTime");
        wallSlideSpeedMax = soTarget.FindProperty("wallSlideSpeedMax");
        wallJumpClimb = soTarget.FindProperty("wallJumpClimb");
        wallJumpOff = soTarget.FindProperty("wallJumpOff");
        wallLeap = soTarget.FindProperty("wallLeap");

        effectPos = soTarget.FindProperty("effectPos");
        dustEffect = soTarget.FindProperty("dustEffect");
        fadeEffect = soTarget.FindProperty("fadeEffect");
        fadeInterval = soTarget.FindProperty("fadeInterval");
        jumpEffect = soTarget.FindProperty("jumpEffect");

        pivotObject = soTarget.FindProperty("pivotObject");

        // swordSwingSFX = soTarget.FindProperty("swordSwingSFX");
    }


    public override void OnInspectorGUI()
    {
        soTarget.Update();
        EditorGUI.BeginChangeCheck();

        myPlayer.toolBarTab = GUILayout.Toolbar(myPlayer.toolBarTab, new string[] { "Basic Movement", "Wall Jumping", "Dash", "VFX", "Misc" });

        switch (myPlayer.toolBarTab)
        {
            case 0:
                myPlayer.currentTab = "Basic Movement";
                break;

            case 1:
                myPlayer.currentTab = "Wall Jumping";
                break;

            case 2:
                myPlayer.currentTab = "Dash";
                break;

            case 3:
                myPlayer.currentTab = "VFX";
                break;

            case 4:
                myPlayer.currentTab = "Misc";
                break;
            case 5:
                myPlayer.currentTab = "Misc";
                break;
        }
        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        EditorGUI.BeginChangeCheck();

        switch (myPlayer.currentTab)
        {
            case "Basic Movement":
                EditorGUILayout.PropertyField(moveSpeed);
                EditorGUILayout.PropertyField(maxJumpHeight);
                EditorGUILayout.PropertyField(doubleJumpHeightRatio);
                EditorGUILayout.PropertyField(minJumpHeight);
                EditorGUILayout.PropertyField(timeToJumpApex);
                EditorGUILayout.PropertyField(fallGravityModifier);
                EditorGUILayout.PropertyField(terminalVelocityY);
                EditorGUILayout.PropertyField(accelerationAirborne);
                EditorGUILayout.PropertyField(accelerationGrounded);
                EditorGUILayout.PropertyField(accelerationJump);
                EditorGUILayout.PropertyField(accelerationWallKick);

                break;

            case "Wall Jumping":
                EditorGUILayout.PropertyField(wallStickTime);
                EditorGUILayout.PropertyField(wallSlideSpeedMax);
                EditorGUILayout.PropertyField(wallJumpClimb);
                EditorGUILayout.PropertyField(wallJumpOff);
                EditorGUILayout.PropertyField(wallLeap);
                break;

            case "Dash":
                EditorGUILayout.PropertyField(dashLength);
                EditorGUILayout.PropertyField(dashApexTime);
                EditorGUILayout.PropertyField(dashTime);
                EditorGUILayout.PropertyField(dashAcceleration);
                EditorGUILayout.PropertyField(dashAttack);
                break;

            case "VFX":
                EditorGUILayout.PropertyField(effectPos);
                EditorGUILayout.PropertyField(dustEffect);
                EditorGUILayout.PropertyField(fadeEffect);
                EditorGUILayout.PropertyField(fadeInterval);
                EditorGUILayout.PropertyField(jumpEffect);
                break;

            case "Misc":
                EditorGUILayout.PropertyField(pivotObject);
                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
        }
    }
}
