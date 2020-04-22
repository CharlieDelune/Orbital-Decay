using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject[] planetPrefabs;
    [SerializeField]
    private GameObject planetHolder;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlacePlanets(int systemLayers, int systemSlices) {
        int planetsPlaced = 0;
        List<int> layersWithPlanets = new List<int>();

        //TODO: If we let the player choose how many factions there will be in the game,
            //This shouldn't be 3, it should be however many factions there are
        //Must have at least 3 planets, two for enemy, one for friendly
        while(planetsPlaced < 3)
        {
            for (int i = 0; i < systemLayers-2; i++)
            {
                int notQuitefiftyFifty = Random.Range(1,10);

                //40 percent chance of placing a planet, but never on a layer that already contains a planet
                //Trust me, we don't want to deal with planet collisions right now.
                if (notQuitefiftyFifty > 4 && !layersWithPlanets.Contains(i))
                {
                    int planetSlice = Random.Range(0, systemSlices);
                    int orbit = Random.Range(1, Mathf.Abs((systemSlices/3) - i) + 1);
                    bool placed = PlacePlanet(i, planetSlice, orbit, RevolveDirection.CounterClockwise, planetPrefabs[Random.Range(0, planetPrefabs.Length)]);
                    if (placed)
                    {
                        layersWithPlanets.Add(i);
                        planetsPlaced++;
                    }
                }
            }
        }

    }

    private bool PlacePlanet(int layer, int slice, int revSpeed, RevolveDirection dir, GameObject planetPrefab)
    {
        GameObject planet = Instantiate(planetPrefab);
        GridCell parentCell = GameStateManager.Instance.solarSystemGrid.GetGridCell(layer, slice);
        if(parentCell.Selectable == null)
        {
            planet.transform.position = parentCell.transform.position;

            Planet planetScript = planet.GetComponent<Planet>();
            planetScript.SetParentCell(parentCell);
            planetScript.SetRevolveDirection(dir);
            planetScript.revolveSpeed = revSpeed;
            planet.transform.SetParent(planetHolder.transform);

            PlanetManager.Instance.AddPlanet(planetScript);
            parentCell.Selectable = planetScript;
            planetScript.ParentCell = parentCell;

            //The way level generation works, it will automatically subtract 2 from the number of layers
            //because we discard the inner ring (it's the sun) and the outer ring (because you can't move off of the solar system)
            //So a range from 4-8 is actually the same as a range from 2-6
            int systemLayers = Random.Range(4, 8);
            //Must have at least 4 slices. Max is inconsequential.
            int systemSlices =  Random.Range(8, 15);

            //TODO:
            //We will either need to find a new way of generating grid placement or cull out grids that aren't the
            //grid in view, because grids are now generating sufficiently close to one another to be seen
            planetScript.gravityWell = CircleGridBuilder.Instance.BuildGrid(systemLayers, systemSlices, new Vector3(systemLayers * 100,0, systemSlices * 100));

            planetScript.grid = planetScript.gravityWell.transform.Find("Grid").GetComponent<CircularGrid>();
            planetScript.grid.parentPlanet = planetScript;

            GameObject planet2 = Instantiate(planet);
            planet2.transform.SetParent(planetScript.gravityWell.transform);
            planet2.transform.position = planetScript.gravityWell.transform.position;

            Destroy(planet2.GetComponent<Planet>());
            return true;
        }
        return false;
    }
}
