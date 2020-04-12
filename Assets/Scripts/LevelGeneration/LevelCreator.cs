using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [SerializeField]
    private CircleGridBuilder gridBuilder;
    [SerializeField]
    private UnitPlacer unitPlacer;
    [SerializeField]
    private PlanetPlacer planetPlacer;
    [SerializeField]
    private ResourcePlacer resourcePlacer;

    void Start()
    {
        gridBuilder.BuildLevel();
        unitPlacer.PlaceUnits();
        planetPlacer.PlacePlanets();
        //resourcePlacer.PlaceResources();
        //Pathfinder.InitializeGrid();
        GridManager.Instance.pathfinder.SetGrid(GridManager.Instance.grid);
        Destroy(gameObject);
    }

    void Update()
    {
        
    }
}
