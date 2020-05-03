using System;
using System.Collections;
using UnityEngine;

/// Base class for AI-controlled factions
public class AIFaction : Faction
{
	[SerializeField] private IFactionAI factionAI;

	protected override IEnumerator useTurn()
	{
		int currentUnit = 0;
		yield return null;
		while(true)
		{
			if(!GameStateManager.Instance.AnimationPresent)
			{
				/// performs actions
				factionAI.UseUnitTurn(this.units[currentUnit]);
				/// then, under a certain condition, break out of the loop
				yield return new WaitForSeconds(0.5f);
				currentUnit++;
				if (currentUnit == this.units.Count)
				{
					break;
				}
			}
			yield return null;
		}
		this.EndTurn();
	}

	public override void EndTurn()
	{
		foreach(Unit unit in this.units)
		{
			unit.TakeRemainingMoves();
		}
		this.onEndFactionTurn.Raise(this);
	}
}