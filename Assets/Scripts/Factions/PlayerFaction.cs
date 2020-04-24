using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFaction : Faction
{

	private int finishedMoves;
	private int totalMoves;
	private bool canEndTurn = false;

	public void LoadPlayer()
	{
		GameStateManager.Instance.LoadPlayer(this);
	}

	protected override IEnumerator useTurn()
	{
		yield return null;
	}

	/// Connected to Faction listener
	/// Called when the player ends their turn
	/// Should be connected to the UI
	public override void EndTurn()
	{
		if(GameStateManager.Instance.NextTurn - 1 == this.Index)
		{
			this.finishedMoves = -1;
			this.totalMoves = 0;
			this.canEndTurn = true;
			foreach(Unit unit in this.units)
			{
				if(unit.TakeOutstandingMoves())
				{
					this.totalMoves++;
				}
			}
			this.OnEndMove();
		}
	}

	public override void OnEndMove()
	{
		this.finishedMoves++;
		if(this.finishedMoves >= this.totalMoves && this.canEndTurn)
		{
			this.canEndTurn = false;
			base.EndTurn();
		}
		else if(!this.canEndTurn)
		{
			GameStateManager.Instance.AnimationPresent = false;
		}
	}
}