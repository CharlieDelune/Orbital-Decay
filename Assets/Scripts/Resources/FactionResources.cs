using System;
using System.Collections.Generic;
using UnityEngine;

/// To add later: productionPotentialLevel - max amount to be able to produce from each InGameResource
/// also, currentProductionLevel - the amount that will be produced during the OnPreLoadRound setup

[Serializable]
public class FactionResources
{

	/// The current resources
	public Dictionary<InGameResource, int> resources;

	/// The queue resources to be added to the total resources
	/// This is generated from Miner's and similar units
	private Dictionary<InGameResource, int> queuedResources;

	/// Called whenever there is a change to this.resources
	private Action onChange;
	private Faction faction;

	/// Initializes the class
	/// Had to use a method rather than a constructor to do this
	/// because of issues that arose with null references
	public void Set(Action _onChange, Faction faction)
	{
		this.resources = GameData.Instance.GetInitialFactionResources();
		this.queuedResources = GameData.Instance.GetInitialFactionResources();
		this.faction = faction;
		this.onChange = _onChange;
		this.onChange();
	}

	/// Called before the Faction can begin taking actions
	public void OnPreLoadRound()
	{
		bool changed = false;

		foreach(KeyValuePair<InGameResource, int> keyValuePair in this.queuedResources)
		{
			this.resources[keyValuePair.Key] += keyValuePair.Value;
			changed = true;
		}

		if(changed)
		{
			this.queuedResources = GameData.Instance.GetInitialFactionResources();
			this.onChange();
		}
	}

	/// Returns the current resources in a List form
	public List<(InGameResource, int)> GetResources()
	{
		List<(InGameResource, int)> resourceQuantities = new List<(InGameResource, int)>();
		foreach(KeyValuePair<InGameResource, int> keyValuePair in this.resources)
		{
			resourceQuantities.Add((keyValuePair.Key, keyValuePair.Value));
		}
		return resourceQuantities;
	}

	public bool HasResource(InGameResource resource, int quantity)
	{
		return this.resources.ContainsKey(resource) && this.resources[resource] > quantity;
	}

	public void LoseResource(InGameResource resource, int quantity)
	{
		if(this.resources.ContainsKey(resource) && quantity > 0)
		{
			this.resources[resource] = Math.Max(this.resources[resource] + quantity, 0);
			this.onChange();
		}
	}

	public void GainResource(InGameResource resource, int quantity)
	{
		if(quantity > 0)
		{
			this.resources[resource] += quantity;
			this.onChange();
		}
	}

	/// Tries to use set of resources from the queue and currentResource
	/// Returns whether or not there are enough resources and removes them
	/// from this.queuedResources and this.resources

	/// This is used for refining resources and transforming them
	/// Note: it does not produce the product of the refining process
	public bool TryUseFromQueue(List<(InGameResource, int)> requiredResources)
	{
		foreach(var (resource, amount) in requiredResources)
		{
			if(this.queuedResources[resource] + this.resources[resource] <= amount)
			{
				return false;
			}
		}

		foreach(var (resource, amount) in requiredResources)
		{
			int diff = this.queuedResources[resource] - amount;
			if(diff < 0)
			{
				/// Subtracts from resources
				this.resources[resource] += diff;
				this.queuedResources[resource] = 0;
			}
			else
			{
				this.queuedResources[resource] = diff;
			}
		}
		return true;
	}

	/// Adds resources to this.queuedResources that will be added to
	/// this.resources before the turn starts
	public void AddToQueue(List<(InGameResource, int)> resourceProduction)
	{
		foreach(var (resource, amount) in resourceProduction)
		{
			this.queuedResources[resource] += amount;
		}
	}
}