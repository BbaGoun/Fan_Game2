using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class TestPatrol : MonoBehaviour
    {
        private Vector3 origin;
        private float timer;
        [SerializeField]
        private float xMoveScale;
        [SerializeField]
        private float frequency = 1;

        private void Awake()
        {
            origin = transform.position;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            transform.position = origin + new Vector3(Mathf.Sin(timer/frequency) * xMoveScale, 0f, 0f);
        }
    }
}