using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart.MemoryPool
{
    public class PoolAble_Delay : MonoBehaviour
    {
        PoolAble poolAble;
        Animator animator;
        [SerializeField]
        private float delay;


        private void Awake()
        {
            poolAble = GetComponent<PoolAble>();
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            animator.enabled = true;
            animator.Rebind();
            animator.Update(0f);
            poolAble.ReleaseObjectWithDelay(delay);
        }

        private void OnDisable()
        {
            animator.Rebind();
            animator.Update(0f);
            animator.enabled = false;
        }
    }
}
