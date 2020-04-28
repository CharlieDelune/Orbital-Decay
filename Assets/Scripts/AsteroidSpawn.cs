using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Picks a random asteroid model and rotates it randomly.  also picks a random size between 80% and 100% of the mesh.
public class AsteroidSpawn : MonoBehaviour
{
    public List<GameObject> asteroids = new List<GameObject>();

    void GenAsteroid() {
      Quaternion r = Quaternion.identity;
      r.eulerAngles = new Vector3(Random.Range(-180,180),Random.Range(-180,180),Random.Range(-180,180));
      GameObject a = Instantiate(asteroids[Random.Range(0,asteroids.Count)],transform.position,r) as GameObject;
      a.transform.parent = transform;

      float s = Random.Range(80f,100f);
      a.transform.localScale = new Vector3(s,s,s);
    }

    void Start() {
        GenAsteroid();
    }
}
