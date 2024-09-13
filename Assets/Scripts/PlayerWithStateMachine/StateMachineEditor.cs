using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StateMachine))]
public class StateMachineEditor : Editor
{
    private Editor _editor;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var currentState = serializedObject.FindProperty("currentState");
        if (currentState != null)
        {
            CreateCachedEditor(currentState.objectReferenceValue, null, ref _editor);
            _editor?.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
