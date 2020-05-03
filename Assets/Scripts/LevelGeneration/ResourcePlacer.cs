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
            List<bool> resourcesPlaced = new List<bool>();
            for(int i = 0; i < this.resourceDepositPrefabs.Length; i++)
            {
                resourcesPlaced.Add(false);
            }
            do
            {
                for (int i = 0; i < planet.grid.GetGridSize().layers - 1; i++)
                {
                    int notQuiteFiftyFifty = Random.Range(0, 10);
                    if(notQuiteFiftyFifty > 4)
                    {
                        int randomSlice = Random.Range(0, planet.grid.GetGridSize().slices);
                        int resourceType = Random.Range(0,this.resourceDepositPrefabs.Length);
                        bool placed = PlaceResource(planet.grid, i, randomSlice, resourceType);

                        if(placed)
                        {
                            resourcesPlaced[resourceType] = true;
                        }
                    }
                }
            }
            while (resourcesPlaced.Contains(false));
        }
    }

    private bool PlaceResource(CircularGrid grid, int layer, int slice, int prefabNumber)
    {
        GridCell parentCell = grid.GetGridCell(layer, slice);
        if (parentCell.Selectable == null && parentCell.ResourceDeposit == null)
        {
            ResourceDeposit deposit = Instantiate<ResourceDeposit>(this.resourceDepositPrefabs[prefabNumber]);
            deposit.transform.SetParent(resourceDepositHolder.transform);
            GridManager.Instance.AddResourceDeposit(deposit);
            deposit.SetParentCell(parentCell);
            return true;
        }
        return false;
    }
}
