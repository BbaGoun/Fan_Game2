using UnityEngine;
using UnityEditor;

public class MyObject : ScriptableObject
{
    public int myInt = 42;
}

public class SerializedPropertyTest : MonoBehaviour
{
    void Start()
    {
        MyObject obj = ScriptableObject.CreateInstance<MyObject>();
        SerializedObject serializedObject = new UnityEditor.SerializedObject(obj);

        SerializedProperty serializedPropertyMyInt = serializedObject.FindProperty("myInt");

        Debug.Log("myInt " + serializedPropertyMyInt.intValue);
    }
}