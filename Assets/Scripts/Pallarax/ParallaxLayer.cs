using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class ParallaxLayer : MonoBehaviour
    {
        [Range(-1f, 1f)]
        public float parallaxFactor;
        private Vector3 basePosition;

        private void Start()
        {
            basePosition = transform.localPosition;
        }

        public void Move(float delta)
        {
            Vector3 deltaMove = new Vector3();
            deltaMove.x = delta * parallaxFactor;

            transform.localPosition = basePosition + deltaMove;
        }
    }
}
