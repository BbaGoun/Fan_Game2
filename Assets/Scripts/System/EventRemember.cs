using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class EventRemember : MonoBehaviour
    {
        #region 지역 돌입
        public bool 안휘복도First = true;
        #endregion

        #region 보스 처리
        public bool 안휘성BossKilled = false;
        public bool 녹림BossKilled = false;
        public bool 점창BossKilled = false;
        public bool 사천BossKilled = false;
        public bool 곤륜BossKilled = false;
        public bool 소림BossKilled = false;
        public bool 마교BossKilled = false;
        #endregion

        #region NPC
        #region 안휘성
        public int 안휘성_집무실_가주TalkCount = 0;
        public int 안휘성_복도_만두1TalkCount = 0;
        public int 안휘성_복도_만두2TalkCount = 0;
        public int 안휘성_연무장가는길_만두1TalkCount = 0;
        public int 안휘성_연무장가는길_만두2TalkCount = 0;
        public int 안휘성_시장_세바스찬TalkCount = 0;
        public int 안휘성_시장_애비게일TalkCount = 0;
        public int 안휘성_시장_러끼TalkCount = 0;
        public int 안휘성_시장_마리안느TalkCount = 0;
        public int 안휘성_시장_셰인TalkCount = 0;
        public int 안휘성_시장_민수하TalkCount = 0;
        public int 안휘성_시장_오뉴TalkCount = 0;
        #endregion
        #endregion

        static public EventRemember Instance;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }

        public int GetNPCTalkCount(string NPCName)
        {
            switch(NPCName)
            {
                case "안휘성_집무실_가주":
                    return 안휘성_집무실_가주TalkCount;
                    break;
                case "안휘성_복도_만두1":
                    return 안휘성_복도_만두1TalkCount;
                    break;
                case "안휘성_복도_만두2":
                    return 안휘성_복도_만두2TalkCount;
                    break;
                case "안휘성_연무장가는길_만두1":
                    return 안휘성_연무장가는길_만두1TalkCount;
                    break;
                case "안휘성_연무장가는길_만두2":
                    return 안휘성_연무장가는길_만두2TalkCount;
                    break;
                case "안휘성_시장_세바스찬":
                    return 안휘성_시장_세바스찬TalkCount;
                    break;
                case "안휘성_시장_애비게일":
                    return 안휘성_시장_애비게일TalkCount;
                    break;
                case "안휘성_시장_러끼":
                    return 안휘성_시장_러끼TalkCount;
                    break;
                case "안휘성_시장_마리안느":
                    return 안휘성_시장_마리안느TalkCount;
                    break;
                case "안휘성_시장_셰인":
                    return 안휘성_시장_셰인TalkCount;
                    break;
                case "안휘성_시장_민수하":
                    return 안휘성_시장_민수하TalkCount;
                    break;
                case "안휘성_시장_오뉴":
                    return 안휘성_시장_오뉴TalkCount;
                    break;
                default:
                    return 1;
            }
        }

        public void UpNPCTalkCount(string NPCName)
        {
            switch (NPCName)
            {
                case "안휘성_집무실_가주":
                    안휘성_집무실_가주TalkCount++;
                    break;
                case "안휘성_복도_만두1":
                    안휘성_복도_만두1TalkCount++;
                    break;
                case "안휘성_복도_만두2":
                    안휘성_복도_만두2TalkCount++;
                    break;
                case "안휘성_연무장가는길_만두1":
                    안휘성_연무장가는길_만두1TalkCount++;
                    break;
                case "안휘성_연무장가는길_만두2":
                    안휘성_연무장가는길_만두2TalkCount++;
                    break;
                case "안휘성_시장_세바스찬":
                    안휘성_시장_세바스찬TalkCount++;
                    break;
                case "안휘성_시장_애비게일":
                    안휘성_시장_애비게일TalkCount++;
                    break;
                case "안휘성_시장_러끼":
                    안휘성_시장_러끼TalkCount++;
                    break;
                case "안휘성_시장_마리안느":
                    안휘성_시장_마리안느TalkCount++;
                    break;
                case "안휘성_시장_셰인":
                    안휘성_시장_셰인TalkCount++;
                    break;
                case "안휘성_시장_민수하":
                    안휘성_시장_민수하TalkCount++;
                    break;
                case "안휘성_시장_오뉴":
                    안휘성_시장_오뉴TalkCount++;
                    break;
            }
        }
    }
}
