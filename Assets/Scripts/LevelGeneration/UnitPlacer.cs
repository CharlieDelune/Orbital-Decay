using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject unitPrefab;
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
        UnitInfo unitInfo = new UnitInfo("Player Unit", 2, 10 , 5, 2, unitPrefab.GetComponent<Unit>());
        Unit unit = unitInfo.InstantiateUnit(GameStateManager.Instance.Factions[1]);
        GridCell parentCell = grid.GetGridCell(0, 0);
        parentCell.Selectable = unit;
        unit.transform.position = parentCell.transform.position;
        unit.SetParentCell(parentCell);
        unit.transform.SetParent(unitHolder.transform);
        UnitManager.Instance.AddPlayerUnit(unit);
    }
}
