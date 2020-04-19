﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
	private static UnitManager _instance;
	public static UnitManager Instance { get { return _instance; } }

    private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			throw new System.Exception("Multiple UnitManager instances are present. There should only be one instance of a Singleton.");
		} else {
			_instance = this;
		}
        playerUnits = new List<Unit>();
	}

    public List<Unit> playerUnits;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayerUnit(Unit unitIn)
    {
        playerUnits.Add(unitIn);
        GameStateManager.Instance.AddPlayerUnit(unitIn);
    }

    public void OnUnitAttack(HeavyGameEventData data)
    {
        Unit attackingUnit = (Unit)data.SourceCell.Selectable;
        Unit defendingUnit = (Unit)data.TargetCell.Selectable;
        int damage = 0;
        //Close Range
        if (data.SourceCell.GetNeighbors().Contains(data.TargetCell))
        {
            //TODO: Verify formula
            damage = attackingUnit.GetAttack().closeAttack - defendingUnit.GetDefense().closeDefense;
        }
        //Long Range
        else
        {
            //TODO: Verify formula
            damage = attackingUnit.GetAttack().longAttack - defendingUnit.GetDefense().longDefense;
        }
        defendingUnit.TakeDamage(damage);
    }

    public void OnUnitDestroyed(MonoBehaviour unitObject)
    {
        Unit unit = unitObject.GetComponent<Unit>();
        unit.Faction.RemoveUnit(unit);
    }
}
