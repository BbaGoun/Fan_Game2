using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEffect : MonoBehaviour
{
    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
}
