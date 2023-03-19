using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VisualGraphRuntime;

[DefaultPortType(typeof(DialogPort))]
public class DialogNode : BaseNode
{
    [HideInInspector]
    public AudioClip Audio;
    [HideInInspector]
    public RolePositionEnum RolePosition;
    //[HideInInspector]
    //public ScriptableObject testSO;

    public List<StoryData> storyData = new List<StoryData>();

    public override void Init()
    {
        base.Init();
        NodeEnum = DialogNodeEnum.StoryNode;
    }
}


[CustomEditor(typeof(DialogNode))]
public class DialogNodeEditor : Editor
{
    SerializedProperty AudioClipField;
    SerializedProperty RolePositionField;
    //SerializedProperty ScriptableObjectField;

    void OnEnable()
    {
        AudioClipField = serializedObject.FindProperty("Audio");
        RolePositionField = serializedObject.FindProperty("RolePosition");
        //ScriptableObjectField = serializedObject.FindProperty("testSO");
    }
    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        if (serializedObject != null)
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(AudioClipField);
            if (GUILayout.Button("Play"))
            {

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(RolePositionField);
            //EditorGUILayout.PropertyField(ScriptableObjectField);

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            //需要在OnInspectorGUI之前修改属性，否则无法修改值
            serializedObject.ApplyModifiedProperties();
        }

        base.OnInspectorGUI();
    }
}