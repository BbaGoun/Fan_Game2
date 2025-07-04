using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace ActionPart
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }

        [SerializeField, ReadOnly(true)]
        private LocalBattleManager localBattleManager;


        public void Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void ChangeLocalBattleManager()
        {
            var sceneSetting = GameObject.FindGameObjectWithTag("SceneSetting");
            // GetComponent는 재귀적으로(자식까지) 탐색하지 않습니다.
            // 따라서 자식 오브젝트를 직접 순회하며 LocalBattleManager를 찾아야 합니다.
            LocalBattleManager _localBattleManager = null;
            foreach (Transform child in sceneSetting.transform)
            {
                _localBattleManager = child.GetComponent<LocalBattleManager>();
                if (_localBattleManager != null)
                {
                    break;
                }
            }
            if(_localBattleManager != null)
            {
                localBattleManager = _localBattleManager;
                localBattleManager.SetDefault();
            }
        }

        public void LocalBossBattleStart()
        {
            localBattleManager.BossBattleStart();
        }
    }
}
