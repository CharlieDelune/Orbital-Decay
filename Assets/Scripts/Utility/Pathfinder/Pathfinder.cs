using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private CircularGrid grid;
    private PathNodeHolder<GridCell>[,] nodeGrid;
    private int gridLayers, gridSlices;

    void Start()
    {
    }
    
    public void SetGrid(CircularGrid gridIn)
    {
        grid = gridIn;
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        (int gridLayersIn, int gridSlicesIn) = grid.GetGridSize();
        nodeGrid = new PathNodeHolder<GridCell>[gridLayersIn, gridSlicesIn];

        gridLayers = gridLayersIn;
        gridSlices = gridSlicesIn;

        //Set up pathfinding nodes
        foreach (GridCell cell in grid.gridCells)
        {
            PathNodeHolder<GridCell> holder = new PathNodeHolder<GridCell>();
            holder.node = cell;
            holder.gCost = int.MaxValue;
            holder.hCost = cell.hCost;
            holder.CalculateFCost();
            holder.previousNodeHolder = null;
            holder.SetNeighbors(cell.GetNeighbors());
            holder.id = holder.node.GetInstanceID();
            nodeGrid[cell.layer,cell.slice] = holder;
        }

        //Set up pathfinding nodes
        foreach(PathNodeHolder<GridCell> node in nodeGrid)
        {
            List<PathNodeHolder<GridCell>> neighbors = new List<PathNodeHolder<GridCell>>();

            int layer = node.node.layer;
            int slice = node.node.slice;

            if (layer - 1 >= 0)
            {
                neighbors.Add(nodeGrid[layer - 1, slice]);
            }
            if (layer + 1 < gridLayersIn)
            {
                neighbors.Add(nodeGrid[layer + 1, slice]);
            }
            if (slice - 1 >= 0)
            {
                neighbors.Add(nodeGrid[layer, slice - 1]);
            }
            if (slice - 1 == -1)
            {
                neighbors.Add(nodeGrid[layer, gridSlicesIn-1]);
            }
            if (slice + 1 < gridSlicesIn)
            {
                neighbors.Add(nodeGrid[layer, slice + 1]);
            }
            if (slice + 1 == gridSlicesIn)
            {
                neighbors.Add(nodeGrid[layer, 0]);
            }

            node.SetNeighbors(neighbors);
        }
    }

    private void ResetCellCosts()
    {
        foreach (PathNodeHolder<GridCell> node in nodeGrid)
        {
            node.gCost = int.MaxValue;
            node.hCost = node.node.hCost;
            node.CalculateFCost();
            node.previousNodeHolder = null;
        }
    }

    public List<Vector3> FindVectorPath(List<GridCell> path)
    {
        List<Vector3> vectorPath = new List<Vector3>();
        foreach (GridCell cell in path)
        {
            vectorPath.Add(cell.transform.position);
        }
        return vectorPath;
    }

    public List<Vector3> FindVectorPath(GridCell startCell, GridCell endCell)
    {
        ResetCellCosts();
        List<GridCell> path = FindPath(startCell.layer, startCell.slice, endCell.layer, endCell.slice);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (GridCell cell in path)
            {
                vectorPath.Add(cell.transform.position);
            }
            return vectorPath;
        }
    }

    public List<GridCell> FindPath(int startLayer, int startSlice, int endLayer, int endSlice)
    {
        ResetCellCosts();
        PathNodeHolder<GridCell> startNodeHolder = nodeGrid[startLayer, startSlice];
        PathNodeHolder<GridCell> endNodeHolder = nodeGrid[endLayer, endSlice];

        BinaryTree<PathNodeHolder<GridCell>> openCells = new BinaryTree<PathNodeHolder<GridCell>>();
        HashSet<PathNodeHolder<GridCell>> closedCells = new HashSet<PathNodeHolder<GridCell>>();

        startNodeHolder.gCost = 0;
        startNodeHolder.hCost = CalculateDistance(startNodeHolder.node, endNodeHolder.node);
        startNodeHolder.CalculateFCost();

        openCells.Add(startNodeHolder);
        int numberOfLoops = 0;
        while(openCells.Count() > 0 && numberOfLoops < 1000)
        {
            PathNodeHolder<GridCell> currentCell = GetLowestFCostCell(openCells);
            if(currentCell.node == endNodeHolder.node)
            {
                return CalculatePath(endNodeHolder);
            }

            openCells.Remove(currentCell);
            closedCells.Add(currentCell);

            List<PathNodeHolder<GridCell>> currentCellNeighbors = currentCell.GetNeighbors();

            foreach (PathNodeHolder<GridCell> neighbor in currentCellNeighbors)
            {
                if (closedCells.Contains(neighbor))
                {
                    continue;
                }
                if (neighbor.node.Selectable != null && !(neighbor == endNodeHolder))
                {
                    openCells.Remove(currentCell);
                    closedCells.Add(neighbor);
                    continue;
                };
                int tentativeGCost = CalculateDistance(currentCell.node, neighbor.node);
                if (tentativeGCost < neighbor.gCost)
                {
                    neighbor.previousNodeHolder = currentCell;
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = CalculateDistance(neighbor.node, endNodeHolder.node);
                    neighbor.CalculateFCost();

                    if (!openCells.Contains(neighbor))
                    {
                        openCells.Add(neighbor);
                    }
                }
            }
            numberOfLoops++;
        }

        return null;
    }

    private int CalculateDistance(GridCell a, GridCell b)
    {
        int moveCost = 10;
        float xDistance = Mathf.Abs(a.gameObject.transform.position.x - b.gameObject.transform.position.x);
        float zDistance = Mathf.Abs(a.gameObject.transform.position.z - b.gameObject.transform.position.z);
        float remaining = Mathf.Abs(xDistance - zDistance);
        return (int)((moveCost * Mathf.Min(xDistance, zDistance)) + (moveCost * remaining));
    }

    private PathNodeHolder<GridCell> GetLowestFCostCell(BinaryTree<PathNodeHolder<GridCell>> gridCellList)
    {
        return gridCellList.FindLowestNode();
    }

    private List<GridCell> CalculatePath(PathNodeHolder<GridCell> endCell)
    {
        List<GridCell> instancePath = new List<GridCell>();
        if ((endCell.node.Selectable == null && endCell.node.ResourceDeposit == null) 
            || (endCell.node.Selectable != null && endCell.node.Selectable.selectableType == SelectableType.Planet))
        {
            instancePath.Add(endCell.node);
        }
        PathNodeHolder<GridCell> currentCell = endCell;
        while (currentCell.previousNodeHolder != null)
        {
            instancePath.Add(currentCell.previousNodeHolder.node);
            currentCell = currentCell.previousNodeHolder;
        }
        instancePath.Reverse();
        
        return instancePath;
    }
}
