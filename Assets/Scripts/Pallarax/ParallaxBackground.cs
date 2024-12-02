using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace ActionPart
{
    public class ParallaxBackground : MonoBehaviour
    {
        public Camera mainCamera;
        [SerializeField]
        private Vector3 cameraBasePosition;

        [SerializeField]
        List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

        public void SetCamera()
        {
            mainCamera = Camera.main;
        }

        /// <summary>
        /// 가까운 것이 더 빠르게 움직인다 -> 캐릭터를 따라오지 않음
        /// 먼 것이 더 느리게 움직인다 -> 캐릭터를 따라온다.
        /// </summary>

        void LateUpdate()
        {
            if (!LoadingManager.Instance.CheckIsCamSetDone())
                return;

            float delta = mainCamera.transform.position.x - cameraBasePosition.x;

            Move(delta);
        }

        void Move(float delta)
        {
            foreach(var layer in parallaxLayers)
            {
                layer.Move(delta);
            }
        }
    }
}
