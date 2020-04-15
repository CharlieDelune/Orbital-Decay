using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacer : MonoBehaviour
{
    [SerializeField]
    private string unitName;
    [SerializeField]
    private CircularGrid grid;
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
        //The unit script is attached to the prefab so it's going to instantiate the prefab in the world
        GridCell parentCell = grid.GetGridCell(0, 0);
        GameStateManager.Instance.Factions[1].CreateUnit(parentCell, this.unitName);
    }
}
