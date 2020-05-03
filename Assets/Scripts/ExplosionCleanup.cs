using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCleanup : MonoBehaviour
{
    [SerializeField] float cleanTime = 4.0f;

    void Start()
    {
        StartCoroutine(CleanUp());
    }

    IEnumerator CleanUp() {
      yield return new WaitForSeconds(cleanTime);
      Destroy(this.gameObject);
    }
}
