using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePlacer : MonoBehaviour
{
    [SerializeField]
    private ResourceDeposit resourceDepositPrefab;
    [SerializeField]
    private CircularGrid grid;
    private Faction playerFaction;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlaceResources() {
        //The unit script is attached to the prefab so it's going to instantiate the prefab in the world
        GridCell parentCell = grid.GetGridCell(1, 0);
        ResourceDeposit deposit = Instantiate<ResourceDeposit>(this.resourceDepositPrefab);
        deposit.SetParentCell(parentCell);
    }
}
