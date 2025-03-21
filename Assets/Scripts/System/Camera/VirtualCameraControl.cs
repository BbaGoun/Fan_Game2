using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class VirtualCameraControl : MonoBehaviour
    {
        public static VirtualCameraControl Instance { get; private set; }

        public CinemachineVirtualCamera[] allVirtualCameras;

        public float turnOffset;
        public float turnTime = 0;

        public float yDamping;
        public string camAreaName;
        
        CinemachineVirtualCamera _currentCamera;
        CinemachineBasicMultiChannelPerlin _perlinNoise;
        CinemachineFramingTransposer _framingTransposer;
        CinemachineConfiner2D _confiner;

        Coroutine shakeCameraCoroutine;
        Coroutine turnCameraCoroutine;
        Coroutine _panCameraCoroutine;
        
        private bool isOffsetUsed;

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

            _currentCamera = allVirtualCameras[0];

            _confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
            _perlinNoise = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _framingTransposer = _currentCamera.GetComponentInChildren<CinemachineFramingTransposer>();
        }

        public void SetCamSize(float value)
        {
            _currentCamera.m_Lens.OrthographicSize = value;
        }

        public void SetConfiner()
        {
            _confiner.m_BoundingShape2D = GameObject.FindGameObjectWithTag("CamArea").GetComponent<CompositeCollider2D>();
            _confiner.InvalidateCache();
        }

        public void ShakeCamera(float duration, float intensity, float frequency = 1f)
        {
            if (shakeCameraCoroutine != null)
                StopCoroutine(shakeCameraCoroutine);
            StartCoroutine(ShakeCameraCoroutine(duration, intensity, frequency));
        }

        public void SetShakeCameraDirect(float intensity, float frequency)
        {
            _perlinNoise.m_AmplitudeGain = intensity;
            _perlinNoise.m_FrequencyGain = frequency;
        }

        IEnumerator ShakeCameraCoroutine(float duration, float intensity, float frequency = 1f)
        {
            _perlinNoise.m_AmplitudeGain = intensity;
            _perlinNoise.m_FrequencyGain = frequency;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var newIntensity = LeanTween.easeOutQuint(intensity, 0f, elapsedTime);
                _perlinNoise.m_AmplitudeGain = newIntensity;
                yield return null;
            }

            _perlinNoise.m_AmplitudeGain = 0f;
            _perlinNoise.m_FrequencyGain = 0f;
        }

        public void TurnCameraRight(bool isRight)
        {
            if (_currentCamera != allVirtualCameras[0] && _currentCamera != allVirtualCameras[1])
            {
                _framingTransposer.m_TrackedObjectOffset.x = 0f;
                return;
            }

            if (turnCameraCoroutine != null)
                StopCoroutine(turnCameraCoroutine);
            
            if (CheckIsOffsetUsed())
            {
                Debug.Log("이미 카메라를 쓰고 계셔유");
                return;
            }

            var offsetX = GetEndOffset(isRight);
            turnCameraCoroutine = StartCoroutine(TurnCameraCoroutine(offsetX));

            IEnumerator TurnCameraCoroutine(float offsetX)
            {
                var startOffsetX = _framingTransposer.m_TrackedObjectOffset.x;
                float elapsedTime = 0f;
                while (elapsedTime < turnTime)
                {
                    elapsedTime += Time.deltaTime;

                    var newOffset = LeanTween.easeInOutSine(startOffsetX, offsetX, elapsedTime / turnTime);
                    _framingTransposer.m_TrackedObjectOffset.x = newOffset;

                    yield return null;
                }
            }
        }

        public void ChangeYDumping(float velocityY, float maxFallSpeed)
        {
            if (_currentCamera != allVirtualCameras[0]) {
                _framingTransposer.m_YDamping = 0f;
                return;
            }

            if (velocityY >= Physics2D.gravity.y)
            {
                _framingTransposer.m_YDamping = yDamping;
            }
            else
            {
                var changed_yDamping = Mathf.Lerp(0f, yDamping, (velocityY - maxFallSpeed) / (Physics2D.gravity.y - maxFallSpeed));
                _framingTransposer.m_YDamping = changed_yDamping;
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
            if(turnCameraCoroutine != null)
                StopCoroutine(turnCameraCoroutine);
            if(_panCameraCoroutine != null)
                StopCoroutine(_panCameraCoroutine);

            _panCameraCoroutine = StartCoroutine(IEPanCamera(panDistance, panTime, panDirection, panToStartingPos));

            IEnumerator IEPanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
            {
                Vector2 endPos = Vector2.zero;
                Vector2 startingPos = Vector2.zero;

                if (!panToStartingPos)
                {
                    SetIsOffsetUsed(true);

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
        
                    startingPos = _framingTransposer.m_TrackedObjectOffset;
                }
                else
                {
                    SetIsOffsetUsed(false);

                    startingPos = _framingTransposer.m_TrackedObjectOffset;
                    endPos = new Vector2(GetEndOffset(PlayerWithStateMachine.Instance.CheckIsRight()), 0f);
                }

                float elapsedTime = 0f;
                while(elapsedTime < panTime)
                {
                    elapsedTime += Time.deltaTime;

                    Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
                    _framingTransposer.m_TrackedObjectOffset = panLerp;

                    yield return null;
                }
            }
        }

        private bool CheckIsOffsetUsed()
        {
            return isOffsetUsed;
        }

        private void SetIsOffsetUsed(bool _isOffsetUsed)
        {
            isOffsetUsed = _isOffsetUsed;
        }

        #endregion

        #region Camera Swap

        public void SwapCamera(SwapDirection swapDirection, Vector2 exitDirection, 
            CinemachineVirtualCamera cameraOnLeft, CinemachineVirtualCamera cameraOnRight, CinemachineVirtualCamera cameraOnUp, CinemachineVirtualCamera cameraOnDown)
        {
            switch (swapDirection)
            {
                case SwapDirection.Horizontal:
                    if (_currentCamera == cameraOnLeft && exitDirection.x > 0f)
                    {
                        cameraOnLeft.enabled = false;
                        cameraOnRight.enabled = true;
                        _currentCamera = cameraOnRight;

                        _confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
                        _perlinNoise = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                        _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    }
                    else if (_currentCamera == cameraOnRight && exitDirection.x < 0f)
                    {
                        cameraOnLeft.enabled = true;
                        cameraOnRight.enabled = false;
                        _currentCamera = cameraOnLeft;

                        _confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
                        _perlinNoise = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                        _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    }
                    break;

                case SwapDirection.Vertical:
                    if (_currentCamera == cameraOnUp && exitDirection.y < 0f)
                    {
                        cameraOnUp.enabled = false;
                        cameraOnDown.enabled = true;
                        _currentCamera = cameraOnDown;

                        _confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
                        _perlinNoise = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                        _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    }
                    else if (_currentCamera == cameraOnDown && exitDirection.y > 0f)
                    {
                        cameraOnUp.enabled = true;
                        cameraOnDown.enabled = false;
                        _currentCamera = cameraOnUp;

                        _confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
                        _perlinNoise = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                        _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    }
                    break;
            }
        }

        #endregion
    }
}