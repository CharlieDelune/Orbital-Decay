using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Faction would hold reference to all of its
/// units
public abstract class Faction : MonoBehaviour
{
	public string FactionName;

	protected List<Unit> units = new List<Unit>();
	
	/// Points that help to measure the faction's
	/// capabilities and progression - not sure if needed
	public int Points;

	/// holds the reference to the GameLoopManager's next method
	protected Action next;

	/// Setup before the Faction starts their turn
	/// Ideally where all the units belonging to
	/// the faction update their states
	public void StartTurn(Action _next)
	{
		this.next = _next;
		this.updateUnits();
		this.StartCoroutine(this.useTurn());
	}

	public void AddUnit(Unit unitIn)
	{
		units.Add(unitIn);
	}

	/// Calls the UpdatePreTurn method of all the Faction's units
	protected virtual void updateUnits()
	{
		foreach(Unit unit in this.units)
		{
			unit.UpdatePreTurn();
		}
	}

	protected abstract IEnumerator useTurn();
}