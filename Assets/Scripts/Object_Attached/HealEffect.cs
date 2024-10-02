using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class HealEffect : MonoBehaviour
    {
        public Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
    }
}