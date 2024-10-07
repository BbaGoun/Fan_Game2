using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UIToolkitExamples
{
    public class SimpleBindingExample : EditorWindow
    {
        TextField m_ObjectNameBinding;

        [MenuItem("Window/UIToolkitExamples/Simple Binding Example")]
        public static void ShowDefaultWindow()
        {
            var wnd = GetWindow<SimpleBindingExample>();
            wnd.titleContent = new GUIContent("Simple Binding");
        }

        public void CreateGUI()
        {
            m_ObjectNameBinding = new TextField("Object Name Binding");

            // Note: the "name" property of a GameObject is "m_Name" in serialization
            m_ObjectNameBinding.bindingPath = "m_Name";
            rootVisualElement.Add(m_ObjectNameBinding);
            OnSelectionChange();
        }

        public void OnSelectionChange()
        {
            GameObject selectedObject = Selection.activeObject as GameObject;
            if(selectedObject != null)
            {
                SerializedObject so = new SerializedObject(selectedObject);
                rootVisualElement.Bind(so);

                //Alternatively you can instead bind it to the TextField itself.
                //m_ObjectNameBinding.Bind(so);
            }
            else
            {
                rootVisualElement.Unbind();

                //if you bound the TextField itself, you'd do this instead;
                //m_ObjectNameObject.Unbind();

                m_ObjectNameBinding.value = "";
            }
        }
    }
}
