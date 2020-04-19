using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePlacer : MonoBehaviour
{
    [SerializeField]
    private ResourceDeposit resourceDepositPrefab;
    [SerializeField]
    private GameObject resourceDepositHolder;
    private Faction playerFaction;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlaceResources() {
        //The unit script is attached to the prefab so it's going to instantiate the prefab in the world
        GridCell parentCell = GameStateManager.Instance.solarSystemGrid.GetGridCell(1, 0);
        ResourceDeposit deposit = Instantiate<ResourceDeposit>(this.resourceDepositPrefab);
        deposit.SetParentCell(parentCell);
        deposit.transform.SetParent(resourceDepositHolder.transform);
    }
}
