using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRotate : MonoBehaviour
{
    [SerializeField]
    float speed = 1.5f;

    // Update is called once per frame
    void Update()
    {
          transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
