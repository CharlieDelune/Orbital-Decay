using System;
using System.Collections;
using UnityEngine;

/// Base class for AI-controlled factions
public class AIFaction : Faction
{

	protected override IEnumerator useTurn()
	{
		while(true)
		{
			if(!GameStateManager.Instance.AnimationPresent)
			{
				/// performs actions

				/// then, under a certain condition, break out of the loop
				yield return new WaitForSeconds(1.0f);
				break;
			}
			yield return null;
		}
		this.next();
	}
}