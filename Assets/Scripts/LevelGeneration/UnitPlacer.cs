using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacer : MonoBehaviour
{
    [SerializeField]
    private string[] unitNames;
    [SerializeField]
    private GameObject unitHolder;
    private Faction playerFaction;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlaceUnits() {
        List<Planet> planetsContainingAFaction = new List<Planet>();

        //We'll place each faction on its own planet
        foreach(Faction faction in GameStateManager.Instance.Factions)
        {
            //Initialize a planet
            Planet placementPlanet = null;
            //Select a random planet, and continue selecting random planets until we find one that doesn't house a faction
            do
            {
                placementPlanet = PlanetManager.Instance.planets[Random.Range(0, PlanetManager.Instance.planets.Count)];
            }
            while (planetsContainingAFaction.Contains(placementPlanet));
            
            //Place the faction's main base onto a random cell in the planet grid
            CircularGrid planetGrid = placementPlanet.grid;
            int randomLayer = Random.Range(0, planetGrid.GetGridSize().layers - 1);
            int randomSlice = Random.Range(0, planetGrid.GetGridSize().slices);
            GridCell parentCell = placementPlanet.grid.GetGridCell(randomLayer, randomSlice);

            bool isPlayerFaction = faction.name == "Player Faction" ? true : false;

            faction.CreateUnit(parentCell, isPlayerFaction ? "Main Base" : "Enemy Base", isPlayerFaction);

            //If this is the player faction, put their planet in view just to be nice
            if (isPlayerFaction) {
                 GameStateManager.Instance.GridInView = planetGrid;
            }
            planetsContainingAFaction.Add(placementPlanet);
        }
    }
}
