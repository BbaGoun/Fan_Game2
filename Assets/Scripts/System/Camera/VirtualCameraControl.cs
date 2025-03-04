using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class VirtualCameraControl : MonoBehaviour
    {
        public static VirtualCameraControl Instance { get; private set; }

        public float turnOffset;
        public float turnTime = 0;
        public string camAreaName;
        CinemachineVirtualCamera cvCamera;
        CinemachineBasicMultiChannelPerlin perlinNoise;
        CinemachineFramingTransposer vcFTposer;
        CinemachineConfiner2D confiner;
        Coroutine shakeCameraCoroutine;
        Coroutine turnCameraCoroutine;

        private void Awake()
        {
            #region Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }   
            #endregion


            confiner = GetComponent<CinemachineConfiner2D>();

            cvCamera = GetComponent<CinemachineVirtualCamera>();
            perlinNoise = cvCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            vcFTposer = cvCamera.GetComponentInChildren<CinemachineFramingTransposer>();
        }

        public void SetCamSize(float value)
        {
            cvCamera.m_Lens.OrthographicSize = value;
        }

        public void SetConfiner()
        {
            // 이건 실험 테스트로 string을 통해 가져오는 중
            // 다음에는 엑티브 신에서 가져오도록 해야함

            confiner.m_BoundingShape2D = GameObject.FindGameObjectWithTag("CamArea").GetComponent<CompositeCollider2D>();
            confiner.InvalidateCache();
        }

        public void ShakeCamera(float duration, float intensity, float frequency = 1f)
        {
            if (shakeCameraCoroutine != null)
                StopCoroutine(shakeCameraCoroutine);
            StartCoroutine(ShakeCameraCoroutine(duration, intensity, frequency));
        }

        public void SetShakeCameraDirect(float intensity, float frequency)
        {
            perlinNoise.m_AmplitudeGain = intensity;
            perlinNoise.m_FrequencyGain = frequency;
        }

        IEnumerator ShakeCameraCoroutine(float duration, float intensity, float frequency = 1f)
        {
            perlinNoise.m_AmplitudeGain = intensity;
            perlinNoise.m_FrequencyGain = frequency;

            float elapsedTime = 0f;
            while(elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var newIntensity = LeanTween.easeOutQuint(intensity, 0f, elapsedTime);
                perlinNoise.m_AmplitudeGain = newIntensity;
                yield return null;
            }

            perlinNoise.m_AmplitudeGain = 0f;
            perlinNoise.m_FrequencyGain = 0f;
        }

        public void TurnCameraRight(bool isRight)
        {
            if (turnCameraCoroutine != null)
                StopCoroutine(turnCameraCoroutine);

            var offsetX = GetEndOffset(isRight);
            turnCameraCoroutine = StartCoroutine(TurnCameraCoroutine(offsetX));
        }

        float GetEndOffset(bool isRight)
        {
            if (isRight)
                return turnOffset;
            else
                return -turnOffset;
        }

        IEnumerator TurnCameraCoroutine(float offsetX)
        {
            var startOffsetX = vcFTposer.m_TrackedObjectOffset.x;
            float elapsedTime = 0f;
            while (elapsedTime < turnTime)
            {
                elapsedTime += Time.deltaTime;

                var newOffset = LeanTween.easeInOutSine(startOffsetX, offsetX, elapsedTime / turnTime);
                vcFTposer.m_TrackedObjectOffset.x = newOffset;

                yield return null;
            }
        }
    }
}