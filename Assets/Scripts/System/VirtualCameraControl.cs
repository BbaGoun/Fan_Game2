using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class VirtualCameraControl : MonoBehaviour
    {
        public static VirtualCameraControl Instance { get; private set; }

        public float turnTime = 0;
        CinemachineVirtualCamera cvCamera;
        CinemachineBasicMultiChannelPerlin perlinNoise;
        CinemachineFramingTransposer vcFTposer;
        Coroutine shakeCameraCoroutine;
        Coroutine turnCameraCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

            DontDestroyOnLoad(gameObject);

            cvCamera = GetComponent<CinemachineVirtualCamera>();
            perlinNoise = cvCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            vcFTposer = cvCamera.GetComponentInChildren<CinemachineFramingTransposer>();
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
            yield return new WaitForSeconds(duration);
            perlinNoise.m_AmplitudeGain = 0f;
            perlinNoise.m_FrequencyGain = 0f;
        }

        public void TurnCamera(float offsetX)
        {
            if (turnCameraCoroutine != null)
                StopCoroutine(turnCameraCoroutine);
            turnCameraCoroutine = StartCoroutine(TurnCameraCoroutine(offsetX));
        }

        IEnumerator TurnCameraCoroutine(float offsetX)
        {
            float refFloat = 0f;
            while (true)
            {
                var current = vcFTposer.m_TrackedObjectOffset.x;

                if (Mathf.Abs(offsetX - current) < 0.001f)
                    break;

                float newOffset = Mathf.SmoothDamp(current, offsetX, ref refFloat, turnTime, Mathf.Infinity, deltaTime: Time.fixedDeltaTime);
                vcFTposer.m_TrackedObjectOffset.x = newOffset;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            yield return null;
        }
    }
}