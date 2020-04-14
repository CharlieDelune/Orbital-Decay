using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Note: Miners should have a high priority level to the NextTurn
/// Specifically, should be higher priority than Refineries

public class Miner : Unit
{

	private List<(InGameResource, int)> resourceProduction;

	protected override void setExtra(string extra)
	{
		this.resourceProduction = new List<(InGameResource, int)>();
		String[] resourceAmountPairs = extra.Split('-');

		for(int i = 0; i < resourceAmountPairs.Length; i++)
		{
			string[] resourceNameAndAmount = resourceAmountPairs[i].Split(':');
			InGameResource resource = GameData.Instance.GetResource(resourceNameAndAmount[0]);
			int amount = int.Parse(resourceNameAndAmount[1]);
			this.resourceProduction.Add((resource, amount));
		}
	}

	/// Miner's don't have any selectable actions, so this should always return false
	/// (assuming they do not have movement)
	public override bool CanPerformAction(SelectableActionType actionType, GridCell targetCell, string param)
	{
		return false;
	}

	protected override void updatePreTurn()
	{
		base.updatePreTurn();

		this.Faction.Resources.AddToQueue(this.resourceProduction);
	}
}