using System;
using System.Collections;
using UnityEngine;

public class PlayerFaction : Faction
{

	[SerializeField] private PlayerViewManager playerViewManager;

	public void LoadPlayer()
	{
		GameStateManager.Instance.LoadPlayer(this);
		this.playerViewManager.LoadPlayer(this);
	}

	protected override IEnumerator useTurn()
	{
		yield return null;
	}

	/// Called when the player ends their turn
	/// Should be connected to the UI
	public void EndTurn()
	{
		foreach(Unit unit in this.units)
		{
			unit.TakeOutstandingMoves();
		}
		this.next();
	}
}