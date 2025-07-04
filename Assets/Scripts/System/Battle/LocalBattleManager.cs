using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class LocalBattleManager : MonoBehaviour
    {
        [SerializeField]
        private Boss boss;

        [SerializeField]
        private GameObject bossBeforeObjects;
        
        [SerializeField]
        private GameObject bossBattleObjects;
        
        [SerializeField]
        private GameObject bossBattlePhase1Objects;

        [SerializeField]
        private GameObject bossBattlePhase2Objects;

        [SerializeField]
        private GameObject bossAfterObjects;

        public void SetDefault(){
            bossBeforeObjects?.SetActive(true);
            bossBattleObjects?.SetActive(false);
            bossBattlePhase1Objects?.SetActive(false);
            bossBattlePhase2Objects?.SetActive(false);
            bossAfterObjects?.SetActive(false);
        }

        public void BossBattleStart()
        {
            bossBeforeObjects?.SetActive(false);
            bossBattleObjects?.SetActive(true);
            boss.Initialize();    
        }

        public void BossBattleEnd()
        {
            bossBattleObjects?.SetActive(false);
            bossAfterObjects?.SetActive(true);
        }
    }
}
