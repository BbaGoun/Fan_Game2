using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class MobController : MonoBehaviour
    {
        [SerializeField]
        private List<Mob> mobs = new List<Mob>();

        [SerializeField]
        private float attackInterval;

        float timer;

        Coroutine coroutine;

        public void Initialize()
        {
            timer = 0f;
            coroutine = StartCoroutine(IELifeCycle());
        }

        IEnumerator IELifeCycle()
        {
            while (true)
            {
                timer += Time.deltaTime;
                if (timer > attackInterval)
                {
                    timer = 0f;
                    // ���鿡�� ���� ��ȣ ������    
                }
                yield return null;
            }
        }
    }
}
