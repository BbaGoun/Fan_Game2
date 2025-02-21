using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class BossTalkArea : MonoBehaviour, ITalkAble
    {
        Boss boss;
        public string talkEventName;
        public bool isFirst = true;

        public void Initialize(Boss _boss)
        {
            boss = _boss;
        }

        public void TalkStart()
        {

        }

        public void TalkDone()
        {
            boss.Initialize();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag.Equals("Player"))
            {
                if (isFirst)
                {
                    isFirst = false;
                    TalkManager.Instance.TalkStart(talkEventName, this);
                }
            }
        }
    }
}
