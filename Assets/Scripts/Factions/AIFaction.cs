using System;
using System.Collections;
using UnityEngine;

/// Base class for AI-controlled factions
public class AIFaction : Faction
{

	protected override void useTurn()
	{
		/// AI would move around its units
		/// before ending its turn
		this.StartCoroutine(this.delayedNext());
	}

	private IEnumerator delayedNext()
	{
		yield return new WaitForSeconds(1.0f);
		this.next();
	}
}