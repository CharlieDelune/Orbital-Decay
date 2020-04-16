using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour, INodable
{
    public int layer, slice, gCost, hCost, fCost;
    public int nodeValue {get; set;}
    public int id { get; set; }
    public CircularGrid parentGrid;
    [SerializeField]
    private List<GridCell> neighbors;
    public Selectable Selectable;

    void Start()
    {
        nodeValue = 0;
        id = GetInstanceID();
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
        nodeValue = fCost;
    }

    public List<GridCell> GetNeighbors()
    {
        return neighbors;
    }

    public void SetNeighbors(List<GridCell> neighborsIn)
    {
        neighbors = neighborsIn;
    }

    public void SetCoords((int layer, int slice) coordIn)
    {
        layer = coordIn.layer;
        slice = coordIn.slice;
    }

    public (int layer, int slice) GetCoords()
    {
        return (layer, slice);
    }
}
