using ActionPart.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ActionPart
{
    public class SequenceController : MonoBehaviour
    {
        public TimeController _timeController;
        public PlayerInputPart _playerInputPart;
        public SettingContainer _settingContainer;
        public DataManager _dataManager;
        public LoadingManager _loadingManager;
        public TalkManager _talkManager;
        public BattleManager _battleManager;
        public MainMenuController _mainMenuController;
        public AudioController _audioController;
        public GlobalTimelineController _timelineController;
        public PlayerWithStateMachine _playerWithStateMachine;
        
        private void Awake()
        {
            // 선행
            _timelineController.Initialize();
            _timeController.Initialize();
            _battleManager.Initialize();

            // 후행
            _settingContainer.Initialize();
            _dataManager.Initialize();
            _talkManager.Initialize();
            _audioController.Initialize();
            _playerInputPart.Initialize();

            // 상관없음
            _mainMenuController.Initialize();
            _loadingManager.Initialize();
            _playerWithStateMachine.Initialize();
        }
    }
}
