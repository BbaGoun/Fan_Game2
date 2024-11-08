using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;

namespace ActionPart
{
    public class JobQueue : MonoBehaviour
    {
        public struct Job : IJob
        {
            public void Execute()
            {
            }
        }

        public struct PJob : IJobParallelFor
        {
            public void Execute(int index)
            {

            }
        }

    }
}
