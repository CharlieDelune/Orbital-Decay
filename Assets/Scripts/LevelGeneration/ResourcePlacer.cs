using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePlacer : MonoBehaviour
{
    [SerializeField]
    private ResourceDeposit[] resourceDepositPrefabs;
    [SerializeField]
    private GameObject resourceDepositHolder;
    private Faction playerFaction;

    public void PlaceResources() {
        foreach(Planet planet in PlanetManager.Instance.planets)
        {
            int resourcesPlaced = 0;
            do
            {
                for (int i = 0; i < planet.grid.GetGridSize().layers - 1; i++)
                {
                    int notQuiteFiftyFifty = Random.Range(0, 10);
                    if(notQuiteFiftyFifty > 4)
                    {
                        int randomSlice = Random.Range(0, planet.grid.GetGridSize().slices);
                        bool placed = PlaceResource(planet.grid, i, randomSlice);

                        if(placed)
                        {
                            resourcesPlaced++;
                        }
                    }
                }
            }
            while (resourcesPlaced < planet.grid.GetGridSize().layers / 2);
        }
    }

    private bool PlaceResource(CircularGrid grid, int layer, int slice)
    {
        GridCell parentCell = grid.GetGridCell(layer, slice);
        if (parentCell.Selectable == null && parentCell.ResourceDeposit == null)
        {
            ResourceDeposit deposit = Instantiate<ResourceDeposit>(this.resourceDepositPrefabs[Random.Range(0,this.resourceDepositPrefabs.Length)]);
            deposit.SetParentCell(parentCell);
            deposit.transform.SetParent(resourceDepositHolder.transform);
            GridManager.Instance.AddResourceDeposit(deposit);
            return true;
        }
        return false;
    }
}
