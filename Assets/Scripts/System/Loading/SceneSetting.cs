using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class SceneSetting : MonoBehaviour
    {
        public Transform leftSpawnPoint;
        public Transform leftWalkOutPoint;
        
        public Transform rightSpawnPoint;
        public Transform rightWalkOutPoint;

        public Transform noneSpawnPoint;

        public float[] camSizes = new float[5];
        public Transform[] lockedCamPoint = new Transform[3];

        public AudioClip bgmClip;
    }
}
