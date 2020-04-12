using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNode : MonoBehaviour
{
    Material mat;
    public GridCell cellRepresented;
    public Unit unitSelected;

    void Start()
    {
        mat = gameObject.GetComponent<MeshRenderer>().material;
    }

    void OnMouseEnter()
    {
        mat.color = Color.green;
    }

    void OnMouseExit()
    {
        mat.color = Color.white;
    }

    void OnMouseDown()
    {
    }
}
