using System;
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

    private int seed;

    public void SetSeed(string seedIn)
    {
        this.seed =  Int32.Parse(seedIn);
    }

    public void StartLevelCreation()
    {
        //Eventually we'll replace this with a player specified seed
        if(this.seed == 0)
        {
            this.seed = new System.Random().Next(999999999);
        }
        UnityEngine.Random.InitState(seed);

        int solarSystemLayers = UnityEngine.Random.Range(9, 14);
        int solarSystemSlices = UnityEngine.Random.Range(20, 40);

        gridBuilder.BuildLevel(solarSystemLayers, solarSystemSlices);
        planetPlacer.PlacePlanets(solarSystemLayers, solarSystemSlices);
        unitPlacer.PlaceUnits();
        resourcePlacer.PlaceResources();

        GameStateManager.Instance.seed = this.seed;

        Destroy(gameObject);
    }

    void Update()
    {
        
    }
}
