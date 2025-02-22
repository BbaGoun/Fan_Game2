using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class EventRemember : MonoBehaviour
    {
        #region ���� ����
        public bool ���ֺ���First = true;
        #endregion

        #region ���� ó��
        public bool ���ּ�BossKilled = false;
        public bool �츲BossKilled = false;
        public bool ��âBossKilled = false;
        public bool ��õBossKilled = false;
        public bool ���BossKilled = false;
        public bool �Ҹ�BossKilled = false;
        public bool ����BossKilled = false;
        #endregion

        #region NPC
        #region ���ּ�
        public int ���ּ�_������_����TalkCount = 0;
        public int ���ּ�_����_����1TalkCount = 0;
        public int ���ּ�_����_����2TalkCount = 0;
        public int ���ּ�_�����尡�±�_����1TalkCount = 0;
        public int ���ּ�_�����尡�±�_����2TalkCount = 0;
        public int ���ּ�_����_���ٽ���TalkCount = 0;
        public int ���ּ�_����_�ֺ����TalkCount = 0;
        public int ���ּ�_����_����TalkCount = 0;
        public int ���ּ�_����_�����ȴ�TalkCount = 0;
        public int ���ּ�_����_����TalkCount = 0;
        public int ���ּ�_����_�μ���TalkCount = 0;
        public int ���ּ�_����_����TalkCount = 0;
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
                case "���ּ�_������_����":
                    return ���ּ�_������_����TalkCount;
                    break;
                case "���ּ�_����_����1":
                    return ���ּ�_����_����1TalkCount;
                    break;
                case "���ּ�_����_����2":
                    return ���ּ�_����_����2TalkCount;
                    break;
                case "���ּ�_�����尡�±�_����1":
                    return ���ּ�_�����尡�±�_����1TalkCount;
                    break;
                case "���ּ�_�����尡�±�_����2":
                    return ���ּ�_�����尡�±�_����2TalkCount;
                    break;
                case "���ּ�_����_���ٽ���":
                    return ���ּ�_����_���ٽ���TalkCount;
                    break;
                case "���ּ�_����_�ֺ����":
                    return ���ּ�_����_�ֺ����TalkCount;
                    break;
                case "���ּ�_����_����":
                    return ���ּ�_����_����TalkCount;
                    break;
                case "���ּ�_����_�����ȴ�":
                    return ���ּ�_����_�����ȴ�TalkCount;
                    break;
                case "���ּ�_����_����":
                    return ���ּ�_����_����TalkCount;
                    break;
                case "���ּ�_����_�μ���":
                    return ���ּ�_����_�μ���TalkCount;
                    break;
                case "���ּ�_����_����":
                    return ���ּ�_����_����TalkCount;
                    break;
                default:
                    return 1;
            }
        }

        public void UpNPCTalkCount(string NPCName)
        {
            switch (NPCName)
            {
                case "���ּ�_������_����":
                    ���ּ�_������_����TalkCount++;
                    break;
                case "���ּ�_����_����1":
                    ���ּ�_����_����1TalkCount++;
                    break;
                case "���ּ�_����_����2":
                    ���ּ�_����_����2TalkCount++;
                    break;
                case "���ּ�_�����尡�±�_����1":
                    ���ּ�_�����尡�±�_����1TalkCount++;
                    break;
                case "���ּ�_�����尡�±�_����2":
                    ���ּ�_�����尡�±�_����2TalkCount++;
                    break;
                case "���ּ�_����_���ٽ���":
                    ���ּ�_����_���ٽ���TalkCount++;
                    break;
                case "���ּ�_����_�ֺ����":
                    ���ּ�_����_�ֺ����TalkCount++;
                    break;
                case "���ּ�_����_����":
                    ���ּ�_����_����TalkCount++;
                    break;
                case "���ּ�_����_�����ȴ�":
                    ���ּ�_����_�����ȴ�TalkCount++;
                    break;
                case "���ּ�_����_����":
                    ���ּ�_����_����TalkCount++;
                    break;
                case "���ּ�_����_�μ���":
                    ���ּ�_����_�μ���TalkCount++;
                    break;
                case "���ּ�_����_����":
                    ���ּ�_����_����TalkCount++;
                    break;
            }
        }
    }
}
