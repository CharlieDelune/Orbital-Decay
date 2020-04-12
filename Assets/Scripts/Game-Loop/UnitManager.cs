using System.Collections;
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
/*
    public void RemovePlayerUnit(Unit unitOut)
    {
        playerUnits.Remove(unitOut);
    }

    public void PrepareAllPlayerUnitsForNextTurn()
    {
        foreach(Unit u in playerUnits)
        {
            u.PrepareForNextTurn();
        }
    }

    public void MoveAllOutstandingUnits()
    {
        foreach(Unit u in playerUnits)
        {
            u.TakeOutstandingMoves();
        }
    }
    */
}
