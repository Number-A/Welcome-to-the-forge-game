using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert : MonoBehaviour
{
    private Vector3 StartPosition;
    private float amplitude = 3f;

    void Start()
    {
        StartPosition = transform.position;
    }

    void Update()
    {
        transform.position = new Vector3(StartPosition.x, StartPosition.y + (Mathf.Sin(Time.time) * amplitude), 0);
    }
}
