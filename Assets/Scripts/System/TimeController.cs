using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField, Range(0f, 2f)]
    private float timeScale;

    private void Update()
    {
        Time.timeScale = timeScale;
    }
}
