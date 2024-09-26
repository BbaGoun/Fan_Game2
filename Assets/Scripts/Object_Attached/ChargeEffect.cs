using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class ChargeEffect : MonoBehaviour
    {
        public Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
    }
}