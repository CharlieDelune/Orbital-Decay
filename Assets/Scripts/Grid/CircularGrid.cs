using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularGrid : MonoBehaviour
{
    [SerializeField]
    GameObject gridPrefab;
    [SerializeField]
    GameObject gridHolder;
    [SerializeField]
    public GridCell[,] gridCells;
    public bool isSolarSystem;
    [SerializeField]
    private int layers, slices;
    public Pathfinder pathfinder;
    public Planet parentPlanet;


    void Start()
    {
        gridHolder = gameObject;
    }

    public (int layers, int slices) GetGridSize()
    {
        return (layers, slices);
    }

    public void SetGridSize(int layersIn, int slicesIn)
    {
        layers = layersIn;
        slices = slicesIn;
        gridCells = new GridCell[layersIn, slicesIn];
    }

    public void AddGridCell(GridCell cellIn)
    {
        gridCells[cellIn.layer, cellIn.slice] = cellIn;
        cellIn.parentGrid = this;
    }

    public GridCell GetGridCell(int layer, int slice)
    {
        return gridCells[layer, slice];
    }

    public List<GridCell> GetNeighborsForCell(int layer, int slice)
    {
        List<GridCell> neighbors = new List<GridCell>();

        if (layer - 1 >= 0)
        {
            neighbors.Add(gridCells[layer - 1, slice]);
        }
        if (layer + 1 < layers)
        {
            neighbors.Add(gridCells[layer + 1, slice]);
        }
        if (slice - 1 >= 0)
        {
            neighbors.Add(gridCells[layer, slice - 1]);
        }
        if (slice - 1 == -1)
        {
            neighbors.Add(gridCells[layer, slices-1]);
        }
        if (slice + 1 < slices)
        {
            neighbors.Add(gridCells[layer, slice + 1]);
        }
        if (slice + 1 == slices)
        {
            neighbors.Add(gridCells[layer, 0]);
        }

        return neighbors;
    }

    public void GenerateCellNeighbors()
    {
        foreach(GridCell cell in gridCells)
        {
            cell.SetNeighbors(GetNeighborsForCell(cell.layer, cell.slice));
        }
    }


    public void ColorCells(IEnumerable<GridCell> cells)
    {
        foreach(GridCell cell in cells)
        {
            MeshRenderer rend = cell.gameObject.GetComponent<MeshRenderer>();
            rend.material.color = Color.green;
        }
    }

    public HashSet<GridCell> GetCellsInRange(GridCell startCell, int range)
    {
        HashSet<GridCell> cellsInRange = new HashSet<GridCell>();

        cellsInRange.Add(startCell);

        int remainingRange = range;

        while (remainingRange > 0) {
            HashSet<GridCell> cellsToAdd = new HashSet<GridCell>();
            foreach(GridCell cell in cellsInRange)
            {
                cellsToAdd.UnionWith(cell.GetNeighbors());
            }
            cellsInRange.UnionWith(cellsToAdd);
            remainingRange--;
        }

        return cellsInRange;
    }

    public GridCell GetGridCellForRevolve(GridCell originCell, RevolveDirection direction, int revolveSpeed)
    {
        int currentSlice = originCell.slice;
        int targetSlice = -1;

        if (direction == RevolveDirection.CounterClockwise)
        {
            targetSlice = (currentSlice + revolveSpeed) % slices;
        }
        else
        {
            targetSlice = (currentSlice - revolveSpeed + slices) % slices;
        }

        return gridCells[originCell.layer, targetSlice];
    }

    public List<Vector3> GetGridVectorsForRevolve(int layer, int startSlice, int endSlice, RevolveDirection direction)
    {
        List<Vector3> vectors = new List<Vector3>();
        int currentSlice = startSlice;
        
        do
        {
            if (direction == RevolveDirection.CounterClockwise)
            {
                currentSlice = (currentSlice + 1) % slices;
            }
            else
            {
                currentSlice = (currentSlice - 1 + slices) % slices;
            }

            vectors.Add(gridCells[layer, currentSlice].transform.position);
        }
        while (currentSlice != endSlice);

        return vectors;
    }

    public GridCell GetGridCell(Vector3 position)
	{
        GridCell bestCell = null;
		float closestSqrDistance = Mathf.Infinity;
		foreach(GridCell cell in gridCells)
		{
			Vector3 directionToTarget = cell.transform.position - position;
			float dSqrToTarget = directionToTarget.sqrMagnitude;
			if (dSqrToTarget < closestSqrDistance)
			{
				closestSqrDistance = dSqrToTarget;
				bestCell = cell;
			}
		}
		return bestCell;
    }
}
