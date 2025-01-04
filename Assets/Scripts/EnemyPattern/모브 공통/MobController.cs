using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ActionPart
{
    public class MobController : MonoBehaviour
    {
        [SerializeField]
        private List<Mob> mobs;

        [SerializeField]
        private float attackInterval;

        float timer;

        Coroutine coroutine;

        public void Start()
        {
            mobs = new List<Mob>(GetComponentsInChildren<Mob>());
            
            timer = 0f;
            coroutine = StartCoroutine(IELifeCycle());
        }

        IEnumerator IELifeCycle()
        {
            while (true)
            {
                if (mobs.Count <= 0)
                    break;

                for(int i=mobs.Count-1; i>=0; i--)
                {
                    if (mobs[i] == null)
                    {
                        Debug.Log("모브가 없단다");
                    }
                    if (mobs[i].health == null)
                    {
                        Debug.Log("모브한테 체력을 안 넣어줬단다");
                    }
                    if (!mobs[i].health.CheckIsAlive())
                    {
                        mobs.Remove(mobs[i]);
                    }
                }

                foreach (Mob mob in mobs)
                {
                    mob.GetDistance();
                }

                mobs = mobs.OrderBy(mob => mob.distance).ToList();

                int forwardCount = (int)Mathf.Ceil(mobs.Count * 0.66f);
                for(int i=0; i<forwardCount; i++)
                {
                    var mob = mobs[i];
                    mob.role = Mob.MobRole.Forward;
                }
                for(int j=forwardCount; j<mobs.Count; j++)
                {
                    var mob = mobs[j];
                    mob.role = Mob.MobRole.Support;
                }

                timer += Time.deltaTime;
                if (timer > attackInterval)
                {
                    timer = 0f;
                    foreach (Mob mob in mobs)
                        mob.ReadyAttack();
                }
                yield return null;
            }
        }
    }
}
