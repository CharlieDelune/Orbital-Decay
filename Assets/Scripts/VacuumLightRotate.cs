using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumLightRotate : MonoBehaviour
{
    [SerializeField] Vector3 sunPos = Vector3.zero;

    void RotateLight(Vector3 p) {
      transform.LookAt(p);
    }

    void Update() {
      RotateLight(sunPos);
    }
}
