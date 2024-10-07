using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UIToolkitExamples
{
    public class SimpleBindingPropertyExample : EditorWindow
    {
        TextField m_ObjectNameBinding;

        [MenuItem("Window/UIToolkitExamples/Simple Binding Property Example")]
        public static void ShowDefaultWindow()
        {
            var wnd = GetWindow<SimpleBindingPropertyExample>();
            wnd.titleContent = new GUIContent("Simple Binding Property");
        }

        public void CreateGUI()
        {
            m_ObjectNameBinding = new TextField("Object Name Binding");
            rootVisualElement.Add(m_ObjectNameBinding);
            OnSelectionChange();
        }

        public void OnSelectionChange()
        {
            GameObject selectedObject = Selection.activeObject as GameObject;
            if(selectedObject != null)
            {
                SerializedObject so = new SerializedObject(selectedObject);
                SerializedProperty property = so.FindProperty("m_Name");

                m_ObjectNameBinding.BindProperty(property);
            }
            else
            {
                m_ObjectNameBinding.Unbind();

                m_ObjectNameBinding.value = "";
            }
        }
    }
}
