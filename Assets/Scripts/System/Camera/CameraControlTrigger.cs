using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEditor;

namespace ActionPart
{
    public class CameraControlTrigger : MonoBehaviour
    {
        public CustomInspectorObjects customInspectorObjects;

        private Collider2D _coll;

        private void Start()
        {
            _coll = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (customInspectorObjects.panCameraOnContact)
                {
                    Debug.Log("들어옴");
                    VirtualCameraControl.Instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
                }
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;

                if (customInspectorObjects.swapCameras)
                {
                    VirtualCameraControl.Instance.SwapCamera(customInspectorObjects.swapDirection, exitDirection, customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, customInspectorObjects.cameraOnUp, customInspectorObjects.cameraOnDown);
                }

                if (customInspectorObjects.panCameraOnContact)
                {
                    Debug.Log("나감");
                    VirtualCameraControl.Instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
                }
            }
        }
    }

    [System.Serializable]
    public class CustomInspectorObjects
    {
        public bool swapCameras = false;
        public bool panCameraOnContact = false;

        [HideInInspector] public SwapDirection swapDirection;
        [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
        [HideInInspector] public CinemachineVirtualCamera cameraOnRight;
        [HideInInspector] public CinemachineVirtualCamera cameraOnUp;
        [HideInInspector] public CinemachineVirtualCamera cameraOnDown;

        [HideInInspector] public PanDirection panDirection;
        [HideInInspector] public float panDistance = 3f;
        [HideInInspector] public float panTime = 0.35f;
    }

    public enum PanDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum SwapDirection
    {
        Horizontal,
        Vertical,
    }

    [CustomEditor(typeof(CameraControlTrigger))]
    public class MyScriptEditor : Editor
    {
        CameraControlTrigger cameraControlTrigger;

        private void OnEnable()
        {
            cameraControlTrigger = (CameraControlTrigger)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (cameraControlTrigger.customInspectorObjects.swapCameras)
            {
                cameraControlTrigger.customInspectorObjects.swapDirection = (SwapDirection)EditorGUILayout.EnumPopup("Camera Swap Direction",
                    cameraControlTrigger.customInspectorObjects.swapDirection);

                switch (cameraControlTrigger.customInspectorObjects.swapDirection)
                {
                    case SwapDirection.Horizontal:
                        cameraControlTrigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera On Left Of Trigger", cameraControlTrigger.customInspectorObjects.cameraOnLeft,
                            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

                        cameraControlTrigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera On Right Of Trigger", cameraControlTrigger.customInspectorObjects.cameraOnRight,
                            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
                        break;

                    case SwapDirection.Vertical:
                        cameraControlTrigger.customInspectorObjects.cameraOnUp = EditorGUILayout.ObjectField("Camera On Up Of Trigger", cameraControlTrigger.customInspectorObjects.cameraOnUp,
                            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

                        cameraControlTrigger.customInspectorObjects.cameraOnDown = EditorGUILayout.ObjectField("Camera On Down Of Trigger", cameraControlTrigger.customInspectorObjects.cameraOnDown,
                            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
                        break;
                }
            }

            if (cameraControlTrigger.customInspectorObjects.panCameraOnContact)
            {
                cameraControlTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction",
                    cameraControlTrigger.customInspectorObjects.panDirection);

                cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTrigger.customInspectorObjects.panDistance);
                cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTrigger.customInspectorObjects.panTime);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(cameraControlTrigger);
            }
        }
    }
}