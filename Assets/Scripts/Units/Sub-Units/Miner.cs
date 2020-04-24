using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Note: Miners should have a high priority level to the NextTurn
/// Specifically, should be higher priority than Refineries

public class Miner : Unit
{

	/// Miner's don't have any selectable actions, so this should always return false
	/// (assuming they do not have movement)
	public override bool CanPerformAction(SelectableActionType actionType, GridCell targetCell, string param)
	{
		return false;
	}

	protected override void updatePreTurn()
	{
		base.updatePreTurn();

		this.Faction.Resources.AddToQueue(this.ParentCell.ResourceDeposit.GetResources());
	}
}