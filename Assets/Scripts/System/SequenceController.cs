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
        public ForDebug forDebug;
        
        private void Awake()
        {
            // �� ��� ���� �͵�
            forDebug.Initialize();
            _timeController.Initialize();
            _playerInputPart.Initialize();
            _battleManager.Initialize();
            _audioController.Initialize();

            // ���� �ؾ� �ϴ� �͵�
            _settingContainer.Initialize();
            _dataManager.Initialize();
            _talkManager.Initialize();
            
            // ���� �ؾ� �ϴ� �͵�
            _mainMenuController.Initialize();
            _loadingManager.Initialize();
        }
    }
}
