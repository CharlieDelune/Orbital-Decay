using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Faction would hold reference to all of its
/// units
public abstract class Faction : MonoBehaviour
{
	public string FactionName;

	public FactionResources Resources;

	protected List<Unit> units = new List<Unit>();

	[SerializeField] protected FactionGameEvent onFactionResourcesChange;
	
	/// Points that help to measure the faction's
	/// capabilities and progression - not sure if needed
	public int Points;

	/// holds the reference to the GameLoopManager's next method
	protected Action next;

	[SerializeField] protected FactionGameEvent onStartFactionTurn;

	/// Initializes this.Resources
	protected virtual void Start()
	{
		if(this.onFactionResourcesChange != null)
		{
			this.Resources.Set(() => {this.onFactionResourcesChange.Raise(this);}, this);
		}
		else
		{
			this.Resources.Set(() => {}, this);
		}
	}

	/// Setup before the Faction starts their turn
	/// Ideally where all the units belonging to
	/// the faction update their states
	public void StartTurn(Action _next)
	{
		this.next = _next;

		/// Allows all listening Units to update before the turn starts
		this.onStartFactionTurn.Raise(this);
		this.Resources.OnPreLoadRound();

		this.StartCoroutine(this.useTurn());
	}

	public void AddUnit(Unit unitIn)
	{
		units.Add(unitIn);
	}

	public virtual void processReceivedAction(HeavyGameEventData data)
	{
		if(data.TargetFaction == this)
		{
			switch(data.ActionType)
			{
				case SelectableActionType.ResourceGain:
					this.Resources.GainResource(data.ResourceValue, data.IntValue);
				break;
			}
		}
	}

	protected abstract IEnumerator useTurn();
}