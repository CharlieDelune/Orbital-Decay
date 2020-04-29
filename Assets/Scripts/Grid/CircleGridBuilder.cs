using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*    This tool is used to help initialize circle grids for new levels,
*    should the need to make a new one ever arise.
**/
public class CircleGridBuilder : MonoBehaviour
{
    private static CircleGridBuilder _instance;
	public static CircleGridBuilder Instance { get { return _instance; } }

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			throw new System.Exception("Multiple CircleGridBuilder instances are present. There should only be one instance of a Singleton.");
		} else {
			_instance = this;
		}
	}

    [SerializeField]
    private int gridRadius;
    [SerializeField]
    private GameObject node;
    [SerializeField]
    private Material lineMaterial;
    [SerializeField]
    private GameObject SystemPrefab;
    [SerializeField]
    private GameObject theSun;
    [SerializeField]
    private GameObject gravityWellsHolder;
    [SerializeField]
    private TileMeshGenerator tileMeshGenerator;
    [SerializeField]
    private GameObject tilePrefab;

    void Start()
    {
    }

    void Update()
    {
        
    }

    public GameObject BuildGrid(int layers, int slices, Vector3 centerPosition, bool isSolarSystemGrid = false)
    {
        GameObject systemHolder = Instantiate(SystemPrefab);
        GameObject gridHolder = systemHolder.transform.Find("Grid").gameObject;
        gridHolder.transform.SetParent(systemHolder.transform);
        CircularGrid grid = gridHolder.GetComponent<CircularGrid>();

        GameObject lineHolder = gridHolder.transform.Find("Lines").gameObject;
        lineHolder.transform.SetParent(gridHolder.transform);
        GameObject nodeHolder = gridHolder.transform.Find("Nodes").gameObject;
        nodeHolder.transform.SetParent(gridHolder.transform);

        GameObject plane = gridHolder.transform.Find("Plane").gameObject;
        plane.transform.localScale = new Vector3(layers, 1, layers);

        if(isSolarSystemGrid)
        {
            GameObject sun = Instantiate(theSun);
            sun.transform.SetParent(grid.gameObject.transform);

            grid.SetGridSize(layers - 2, slices);
            grid.isSolarSystem = true;
            GameStateManager.Instance.solarSystemGrid = grid;
        }
        else
        {
            grid.SetGridSize(layers - 1, slices);
            grid.isSolarSystem = false;
        }

        Vector3 previousPos = centerPosition;
        Vector3 currentPos;
        Vector3 desiredPos;

        int maxLayer = isSolarSystemGrid ? layers-1 : layers;
        for (int layer = 1; layer < maxLayer; layer++)
        {
            Vector3 layerStartPos = centerPosition;
            for (int slice = 0; slice < slices; slice++)
            {
                float angle = (slice * Mathf.PI * 2) / slices;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (gridRadius * layer);

                currentPos = transform.position;
                desiredPos = new Vector3(currentPos.x + pos.x, currentPos.y + pos.y, currentPos.z + pos.z);

                //Create grid node for movement and tiling
                GameObject newNode = Instantiate(node);
                //Find center of grid tile
                float diagonalAngle = ((slice + 1) * Mathf.PI * 2) / slices;
                Vector3 diagonalPos = new Vector3(Mathf.Cos(diagonalAngle), 0, Mathf.Sin(diagonalAngle)) * (gridRadius * (layer + 1));
                Vector3 finaldesiredPos = new Vector3(desiredPos.x + diagonalPos.x, 0, desiredPos.z + diagonalPos.z);
                Vector3 nodePosition = desiredPos + (diagonalPos - desiredPos) / 2;
                newNode.transform.position = nodePosition;
                newNode.transform.SetParent(nodeHolder.transform);
                //Initialize GridCell script
                GridCell nodeCell = newNode.GetComponent<GridCell>();
                nodeCell.SetCoords((layer - 1, slice));
                grid.AddGridCell(nodeCell);

                //Draw line between nodes to create rings
                GameObject lineObject = new GameObject("LineHolder");
                LineRenderer line = lineObject.AddComponent<LineRenderer>();
                line.transform.SetParent(lineHolder.transform);
                line.material = lineMaterial;
                line.startWidth = 0.2f;
                line.endWidth = 0.2f;
                line.useWorldSpace = false;
                if(slice != 0) {
                    line.SetPosition(0, previousPos);
                    line.SetPosition(1, desiredPos);
                }
                if (slice == 0)
                {
                    layerStartPos = desiredPos;
                }
                if(line.GetPosition(0).x == 0 && line.GetPosition(0).z == 0 && line.GetPosition(1).x == 0 && line.GetPosition(1).z == 1)
                {
                    Destroy(lineObject);
                }

                //Close rings if we're on the last node of a ring
                if (slice == slices - 1) {
                    GameObject newLineObject = new GameObject("LineHolder");
                    LineRenderer newLine = newLineObject.AddComponent<LineRenderer>();
                    newLine.transform.SetParent(lineHolder.transform);
                    newLine.material = lineMaterial;
                    newLine.startWidth = 0.2f;
                    newLine.endWidth = 0.2f;
                    newLine.useWorldSpace = false;
                    newLine.SetPosition(0, desiredPos);
                    newLine.SetPosition(1, layerStartPos);
                }

                //Draw lines from inner circle to outer circle
                if (layer == 1){
                    GameObject newLineObject = new GameObject("LineHolder");
                    LineRenderer newLine = newLineObject.AddComponent<LineRenderer>();
                    newLine.transform.SetParent(lineHolder.transform);
                    newLine.material = lineMaterial;
                    newLine.startWidth = 0.2f;
                    newLine.endWidth = 0.2f;
                    newLine.useWorldSpace = false;
                    newLine.SetPosition(0, desiredPos);
                    newLine.SetPosition(1, new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (gridRadius * (layers-1)));
                }

                //Create last layer ring (only necessary for solar system grids)
                if (layer == layers-2 && isSolarSystemGrid)
                {
                    GameObject newLineObject = new GameObject("LineHolder");
                    LineRenderer newLine = newLineObject.AddComponent<LineRenderer>();
                    newLine.transform.SetParent(lineHolder.transform);
                    newLine.material = lineMaterial;
                    newLine.startWidth = 0.2f;
                    newLine.endWidth = 0.2f;
                    newLine.SetPosition(0,  new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (gridRadius * (layers-1)));
                    float newAngle = (slice+1) * Mathf.PI * 2 / slices;
                    newLine.SetPosition(1, new Vector3(Mathf.Cos(newAngle), 0, Mathf.Sin(newAngle)) * (gridRadius * (layers-1)));
                }

                Vector3[] verts = new Vector3[] {
                    newNode.transform.position - new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (gridRadius * layer),
                    newNode.transform.position - new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (gridRadius * (layer + 1)),
                    newNode.transform.position - new Vector3(Mathf.Cos(diagonalAngle), 0, Mathf.Sin(diagonalAngle)) * (gridRadius * (layer + 1)),
                    newNode.transform.position - new Vector3(Mathf.Cos(diagonalAngle), 0, Mathf.Sin(diagonalAngle)) * (gridRadius * (layer)),
                };
                TileMeshGenerator gen = newNode.GetComponent<TileMeshGenerator>();
                GameObject tile = newNode.transform.Find("GridTile").gameObject;
                gen.CreateMesh(tile, verts);
                if (layer == maxLayer - 1 && !isSolarSystemGrid)
                {
                    tile.GetComponent<TileShadingHandler>().SetDefaultColor(Constants.tileEdge);
                    nodeCell.isEdgeCell = true;
                }
                previousPos = desiredPos;
            }
        }
        // Tell all grid cells to collect and store their neighbors to make pathfinding way better
        grid.GenerateCellNeighbors();
        systemHolder.transform.position = centerPosition;
        grid.pathfinder.SetGrid(grid);
        systemHolder.transform.SetParent(gravityWellsHolder.transform);
        return systemHolder;
    }
}
