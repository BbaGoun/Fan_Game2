using ActionPart.MemoryPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class AttackEffect : MonoBehaviour
    {
        [SerializeField]
        private float damage;

        private float shakeDuration;
        private float shakeIntensity;

        private Coroutine coroutine;
        //[SerializeField] private float duration;
        private Animator animator;
        public delegate void OnAttackHit();
        public event OnAttackHit eventAttackHit;

        private GameObject player;

        private void Awake()
        {
            player = transform.parent.gameObject;

            animator = GetComponent<Animator>();

            //animator.Rebind();
            //animator.Update(0f);
            animator.enabled = false;
        }

        private void OnEnable()
        {
            animator.enabled = true;
        }

        private void OnDisable()
        {
            //animator.Rebind();
            //animator.Update(0f);
            animator.enabled = false;

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        public void SetShakeDuration(float _shakeDuration)
        {
            shakeDuration = _shakeDuration;
        }

        public void SetShakeIntensity(float _shakeIntensity)
        {
            shakeIntensity = _shakeIntensity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                var enemy = collision.gameObject;

                var damageAble = enemy.GetComponent<IDamageAble>();

                var playerPos = new Vector2(player.transform.position.x, transform.position.y);
                var closestHitPoint = collision.collider.ClosestPoint(playerPos);
                bool isOnRight = playerPos.x < closestHitPoint.x;

                /*float minDistance = Mathf.Infinity;
                Vector2 closestHitPoint = Vector2.zero;
                bool isOnRight = true;
                foreach(ContactPoint2D hitPoint in collision.contacts)
                {
                    var attackPos = new Vector2(transform.position.x, transform.position.y);
                    var hitPos = new Vector2(hitPoint.point.x, hitPoint.point.y);
                    var distance = Vector2.Distance(attackPos, hitPos);

                    if (distance < minDistance)
                    {
                        if (attackPos.x < hitPos.x)
                            isOnRight = true;
                        else
                            isOnRight = false;

                        minDistance = distance;
                        closestHitPoint = hitPos;
                    }
                }*/

                var hitEffect = ObjectPoolManager.Instance.GetObject("Hit_Effect");
                hitEffect.transform.position = new Vector3(closestHitPoint.x, closestHitPoint.y, -5f);
                if (isOnRight)
                    hitEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                else
                    hitEffect.transform.localScale = new Vector3(-1f, 1f, 1f);

                VirtualCameraControl.Instance.ShakeCamera(shakeDuration, shakeIntensity);

                eventAttackHit?.Invoke();

                Vector2 dir = enemy.transform.position - player.transform.position;

                damageAble.GetDamage(damage, dir);
            }
        }
    }
}
