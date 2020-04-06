using System;
using UnityEngine;

public class PlayerFaction : Faction
{

	protected override void useTurn()
	{
	}

	/// Called when the player ends their turn
	/// Should be connected to the UI
	public void EndTurn()
	{
		this.next();
	}
}