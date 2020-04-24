using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMeshGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateMesh(GameObject tilePrefab, Vector3[] vertices)
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = new int[6] { 2, 1, 0, 3, 2, 0};

        tilePrefab.GetComponent<MeshFilter>().mesh = mesh;

        tilePrefab.transform.Rotate(180, 0, 180);

        tilePrefab.AddComponent<MeshCollider>();
    }
}
