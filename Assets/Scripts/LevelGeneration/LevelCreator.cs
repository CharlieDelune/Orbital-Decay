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

    public void StartLevelCreation()
    {
        gridBuilder.BuildLevel();
        unitPlacer.PlaceUnits();
        planetPlacer.PlacePlanets();
        //resourcePlacer.PlaceResources();
        Destroy(gameObject);
    }

    void Update()
    {
        
    }
}
