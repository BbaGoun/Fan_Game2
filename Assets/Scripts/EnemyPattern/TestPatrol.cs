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

        private void Awake()
        {
            origin = transform.position;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            transform.position = origin + new Vector3(Mathf.Sin(timer) * xMoveScale, 0f, 0f);
        }
    }
}