using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject planetPrefab;
    [SerializeField]
    private GameObject planetPrefab2;
    [SerializeField]
    private GameObject planetPrefab3;
    [SerializeField]
    private GameObject planetHolder;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlacePlanets() {
        PlacePlanet(4, 22, 4, RevolveDirection.CounterClockwise, planetPrefab);

        PlacePlanet(5, 12, 2, RevolveDirection.CounterClockwise, planetPrefab2);

        PlacePlanet(2, 9, 8, RevolveDirection.CounterClockwise, planetPrefab3);
    }

    private void PlacePlanet(int layer, int slice, int revSpeed, RevolveDirection dir, GameObject planetPrefab)
    {
        GameObject planet = Instantiate(planetPrefab);
        GridCell parentCell = GridManager.Instance.grid.GetGridCell(layer, slice);
        planet.transform.position = parentCell.transform.position;
        Planet planetScript = planet.GetComponent<Planet>();
        planetScript.SetParentCell(parentCell);
        planetScript.SetRevolveDirection(dir);
        planetScript.revolveSpeed = revSpeed;
        planet.transform.SetParent(planetHolder.transform);
        PlanetManager.Instance.AddPlanet(planetScript);
        parentCell.Selectable = planetScript;
        planetScript.ParentCell = parentCell;
        planetScript.gravityWell = CircleGridBuilder.Instance.BuildLevel((int)Mathf.Floor(layer * 1.5f), slice * 1, new Vector3(layer * 100,0, slice * 100));
        planetScript.grid = planetScript.gravityWell.transform.Find("Grid").GetComponent<CircularGrid>();
        planetScript.grid.parentPlanet = planetScript;
        GameObject planet2 = Instantiate(planet);
        planet2.transform.SetParent(planetScript.gravityWell.transform);
        planet2.transform.position = planetScript.gravityWell.transform.position;
    }
}
