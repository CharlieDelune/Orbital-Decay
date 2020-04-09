using System;
using System.Collections;
using UnityEngine;

public class PlayerFaction : Faction
{

	[SerializeField] private PlayerViewState playerViewState;

	public void LoadPlayer()
	{
		this.playerViewState.LoadPlayer(this);
	}

	protected override IEnumerator useTurn()
	{
		yield return null;
	}

	/// Called when the player ends their turn
	/// Should be connected to the UI
	public void EndTurn()
	{
		this.next();
	}
}