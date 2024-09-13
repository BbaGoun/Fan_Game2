using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ActionPart.MemoryPool
{
    public class PoolAble : MonoBehaviour
    {
        public IObjectPool<GameObject> pool { get; set; }
        private Animator animator;

        private void Awake()
        {
            animator = gameObject.GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (animator != null)
                animator.enabled = true;
        }

        private void OnDisable()
        {
            if (animator != null)
                animator.enabled = false;
        }

        public virtual void ReleaseObject()
        {
            if(gameObject.activeSelf)
                pool.Release(gameObject);
        }

        public virtual void ReleaseObjectWithDelay(float delay)
        {
            StartCoroutine(_ReleaseObjectWithDelay(delay));
        }

        IEnumerator _ReleaseObjectWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ReleaseObject();
        }
    }
}
