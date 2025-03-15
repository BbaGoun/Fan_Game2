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

        public float yDamping;
        public string camAreaName;
        CinemachineVirtualCamera cvCamera;
        CinemachineBasicMultiChannelPerlin perlinNoise;
        CinemachineFramingTransposer vcFTposer;
        CinemachineConfiner2D confiner;
        Coroutine shakeCameraCoroutine;
        Coroutine turnCameraCoroutine;
        Coroutine _panCameraCoroutine;
        private Vector2 _startingTrackedObjectOffset;

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

            _startingTrackedObjectOffset = vcFTposer.m_TrackedObjectOffset;
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
            while (elapsedTime < duration)
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

        public void ChangeYDumping(float velocityY, float maxFallSpeed)
        {
            if (velocityY >= Physics2D.gravity.y)
            {
                vcFTposer.m_YDamping = yDamping;
            }
            else
            {
                var changed_yDamping = Mathf.Lerp(0f, yDamping, (velocityY - maxFallSpeed) / (Physics2D.gravity.y - maxFallSpeed));
                vcFTposer.m_YDamping = changed_yDamping;
            }
        }

        float GetEndOffset(bool isRight)
        {
            if (isRight)
                return turnOffset;
            else
                return -turnOffset;
        }

        #region Pan Camera

        public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
        {
            _panCameraCoroutine = StartCoroutine(IEPanCamera(panDistance, panTime, panDirection, panToStartingPos));


            IEnumerator IEPanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
            {
                Vector2 endPos = Vector2.zero;
                Vector2 startingPos = Vector2.zero;

                if (!panToStartingPos)
                {
                    switch (panDirection)
                    {
                        case PanDirection.Up:
                            endPos = Vector2.up;
                            break;
                        case PanDirection.Down:
                            endPos = Vector2.down;
                            break;
                        case PanDirection.Left:
                            endPos = Vector2.left;
                            break;
                        case PanDirection.Right:
                            endPos = Vector2.right;
                            break;
                        default:
                            break;
                    }
    
                    endPos *= panDistance;
        
                    startingPos = _startingTrackedObjectOffset;
                    
                    endPos += startingPos;
                }
                else
                {
                    startingPos = vcFTposer.m_TrackedObjectOffset;
                    endPos = _startingTrackedObjectOffset;
                }

                float elapsedTime = 0f;
                while(elapsedTime < panTime)
                {
                    elapsedTime += Time.deltaTime;

                    Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
                    vcFTposer.m_TrackedObjectOffset = panLerp;

                    yield return null;
                }
            }
    }

    #endregion
}
}