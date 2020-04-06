using System;
using System.Collections.Generic;
using UnityEngine;

/// Faction would hold reference to all of its
/// units
public abstract class Faction : MonoBehaviour
{
	public bool StillInGame = true;
	public string FactionName;
	
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
		this.useTurn();
	}

	/// Called when the Faction places a unit
	public void PlaceUnit()
	{
	}

	/// Called when a unit is removed
	/// This might be where the faction's
	/// defeat method is called
	public void RemoveUnit()
	{
	}

	protected abstract void useTurn();

	/// called to trigger the faction's defeat
	protected virtual void defeat()
	{
		this.StillInGame = false;
		GameLoopManager.Instance.OnFactionDefeated(this);
	}
}