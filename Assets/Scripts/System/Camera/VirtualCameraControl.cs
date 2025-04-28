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

        public enum VirtaulCamList
        {
            PlayerFollowCam = 0,
            NoYFollowCam = 1,
            LockedCam1 = 2,
            LockedCam2 = 3,
            LockedCam3 = 4,
            TimelineCam = 5,
        }

        public float turnOffset;
        public float turnTime = 0;

        public float yDamping;

        CinemachineBrain _cinemachineBrain;
        CinemachineBlendDefinition _blendCut;
        CinemachineBlendDefinition _blendEaseInOut;
        
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

            _cinemachineBrain = Camera.main.gameObject.GetComponent<CinemachineBrain>();
            _blendCut = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
            _blendEaseInOut = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 2f);

            _currentCamera = allVirtualCameras[0];

            _confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
            _perlinNoise = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _framingTransposer = _currentCamera.GetComponentInChildren<CinemachineFramingTransposer>();
        }

        public void SetCamSize(float value)
        {
            _currentCamera.m_Lens.OrthographicSize = value;
        }

        public void SetCamBySceneSetting()
        {
            var sceneSetting = GameObject.FindGameObjectWithTag("SceneSetting").GetComponent<SceneSetting>();
            SetCamSize(sceneSetting);
            SetConfiner();
            SetLockedCam(sceneSetting);
        }

        private void SetCamSize(SceneSetting sceneSetting)
        {
            for (int i = 0; i < 5; i++)
            {
                allVirtualCameras[i].m_Lens.OrthographicSize = sceneSetting.camSizes[i];
            }
        }

        private void SetConfiner()
        {
            _confiner.m_BoundingShape2D = GameObject.FindGameObjectWithTag("CamArea").GetComponent<CompositeCollider2D>();
            _confiner.InvalidateCache();
        }

        private void SetLockedCam(SceneSetting sceneSetting)
        {
            if(sceneSetting.lockedCamPoint[0] != null)
                allVirtualCameras[(int)VirtaulCamList.LockedCam1].Follow = sceneSetting.lockedCamPoint[0];
            if(sceneSetting.lockedCamPoint[1] != null)
                allVirtualCameras[(int)VirtaulCamList.LockedCam2].Follow = sceneSetting.lockedCamPoint[1];
            if(sceneSetting.lockedCamPoint[2] != null)
                allVirtualCameras[(int)VirtaulCamList.LockedCam3].Follow = sceneSetting.lockedCamPoint[2];
        }

        public void ShakeCamera(float duration, float intensity, float frequency = 1f)
        {
            if (shakeCameraCoroutine != null)
                StopCoroutine(shakeCameraCoroutine);
            StartCoroutine(ShakeCameraCoroutine(duration, intensity, frequency));
        }

        public void OffTimelineCam()
        {
            allVirtualCameras[(int)VirtaulCamList.TimelineCam].gameObject.SetActive(false);
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
            VirtaulCamList cameraOnLeft, VirtaulCamList cameraOnRight, VirtaulCamList cameraOnUp, VirtaulCamList cameraOnDown)
        {
            switch (swapDirection)
            {
                case SwapDirection.Horizontal:
                    if (_currentCamera == allVirtualCameras[(int)cameraOnLeft] && exitDirection.x > 0f)
                    {
                        allVirtualCameras[(int)cameraOnLeft].gameObject.SetActive(false);
                        allVirtualCameras[(int)cameraOnRight].gameObject.SetActive(true);
                        _currentCamera = allVirtualCameras[(int)cameraOnRight];

                        _confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
                        _perlinNoise = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                        _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    }
                    else if (_currentCamera == allVirtualCameras[(int)cameraOnRight] && exitDirection.x < 0f)
                    {
                        allVirtualCameras[(int)cameraOnLeft].gameObject.SetActive(true);
                        allVirtualCameras[(int)cameraOnRight].gameObject.SetActive(false);
                        _currentCamera = allVirtualCameras[(int)cameraOnLeft];

                        _confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
                        _perlinNoise = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                        _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    }
                    break;

                case SwapDirection.Vertical:
                    if (_currentCamera == allVirtualCameras[(int)cameraOnUp] && exitDirection.y < 0f)
                    {
                        allVirtualCameras[(int)cameraOnUp].gameObject.SetActive(false);
                        allVirtualCameras[(int)cameraOnDown].gameObject.SetActive(true);
                        _currentCamera = allVirtualCameras[(int)cameraOnDown];

                        _confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
                        _perlinNoise = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                        _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    }
                    else if (_currentCamera == allVirtualCameras[(int)cameraOnDown] && exitDirection.y > 0f)
                    {
                        allVirtualCameras[(int)cameraOnUp].gameObject.SetActive(true);
                        allVirtualCameras[(int)cameraOnDown].gameObject.SetActive(false);
                        _currentCamera = allVirtualCameras[(int)cameraOnUp];

                        _confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
                        _perlinNoise = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                        _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    }
                    break;
            }
        }

        public void ResetSwapedCamera()
        {
            foreach (var cam in allVirtualCameras)
            {
                cam.gameObject.SetActive(false);
            }
            allVirtualCameras[(int)VirtaulCamList.PlayerFollowCam].gameObject.SetActive(true);
            _currentCamera = allVirtualCameras[(int)VirtaulCamList.PlayerFollowCam];

            _confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
            _perlinNoise = _currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        #endregion

        public void SetCinemachineBrainBlend(CinemachineBlendDefinition.Style style)
        {
            switch (style)
            {
                case CinemachineBlendDefinition.Style.Cut:
                    _cinemachineBrain.m_DefaultBlend = _blendCut;
                    break;
                case CinemachineBlendDefinition.Style.EaseInOut:
                    _cinemachineBrain.m_DefaultBlend = _blendEaseInOut;
                    break;
            }
        }
    }
}