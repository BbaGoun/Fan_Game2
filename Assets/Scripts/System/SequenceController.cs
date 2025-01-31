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
        public ForDebug _forDebug;
        public GlobalTimelineController _timelineController;
        public PlayerWithStateMachine _playerWithStateMachine;
        
        private void Awake()
        {
            // �� ��� ���� �͵�
            _timelineController.Initialize();
            _forDebug.Initialize();
            _timeController.Initialize();
            _battleManager.Initialize();

            // ���� �ؾ� �ϴ� �͵�
            _settingContainer.Initialize();
            _dataManager.Initialize();
            _talkManager.Initialize();
            _audioController.Initialize();
            _playerInputPart.Initialize();

            // ���� �ؾ� �ϴ� �͵�
            _mainMenuController.Initialize();
            _loadingManager.Initialize();
            _playerWithStateMachine.Initialize();
        }
    }
}
