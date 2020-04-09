using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Selectable
{
	private int maxRange;
	private int health;
	private int armor;
	private int attack;

	private bool baseStatsSet;

	/// for quick debugging
	[SerializeField] private bool debug = false;

	public Faction Faction;

	/// Turn State Fields
	/// Reset in UpdatePreTurn

	/// Indicates how many more moves the unit has for this turn
	public int MovesLeft = 0;

	void Awake()
	{
		baseStatsSet = false;
	}

	void Start()
	{
		if(!baseStatsSet && !this.debug)
		{
			throw new System.Exception("Unit was instantiated, but no base stats were set. Make sure you are instantiating through UnitInfo");
		}
	}

	public void SetBaseStats(string name, int maxRange, int health, int armor, int attack) {
		gameObject.name = name;
		this.maxRange = maxRange;
		this.health = health;
		this.armor = armor;
		this.attack = attack;

		baseStatsSet = true;
	}

	/// Updates / Resets the turn fields for the Unit to get ready for the turn
	public void UpdatePreTurn()
	{
		this.MovesLeft = this.maxRange;
	}

	/// Connected by a HeavyGameEventListener
	/// Initial check to see if the fired HeavyGameEventData has to do with this unit
	public virtual void OnFilterGameEvent(HeavyGameEventData data)
	{
		if(data.SourceNode == this.ParentNode || data.TargetNode == this.ParentNode)
		{
			this.processAction(data);
		}
	}

	public override bool CanPerformAction(SelectableActionType actionType, PlaceholderNode targetNode, string param)
	{
		if(this.GetValidActionTypes().Contains(actionType))
		{
			switch(actionType)
			{
				case SelectableActionType.Build:
					/// maybe it should check to see if the param string refers to a unit
					return targetNode.Selectable == null && param != "";
				case SelectableActionType.Attack:
					/// Attacks if the targetNode has a selectable and if the Faction belonging to the selectable is
					/// not the current faction
					return targetNode.Selectable is Unit && ((Unit)targetNode.Selectable).Faction != this.Faction;
				case SelectableActionType.Enhance:
					/// Enhances if the target node is the same node
					return targetNode?.Selectable == this;
				case SelectableActionType.Move:
					/// Moves if the targetNode is empty
					return targetNode.Selectable == null;
			}
		}
		return false;
	}

	public override void PerformAction(SelectableActionType actionType, PlaceholderNode targetNode, string param)
	{
		switch(actionType)
		{
			case SelectableActionType.Build:
				this.performBuild(targetNode, param);
			break;
			case SelectableActionType.Attack:
				this.performAttack(targetNode);
			break;
			case SelectableActionType.Enhance:
				this.performEnhance(param);
			break;
			case SelectableActionType.Move:
				this.performMove(targetNode);
			break;
		}
	}

	/// Raises a build action
	protected virtual void performBuild(PlaceholderNode targetNode, string unitName)
	{
		HeavyGameEventData data = new HeavyGameEventData();
		data.SourceNode = this.ParentNode;
		data.TargetNode = targetNode;
		data.StringValue = unitName;
		data.ActionType = SelectableActionType.Build;
		GameLoopManager.Instance.PerformAction(data);
	}

	/// Raises an attack action
	protected virtual void performAttack(PlaceholderNode targetNode)
	{
		HeavyGameEventData data = new HeavyGameEventData();
		data.SourceNode = this.ParentNode;
		data.TargetNode = targetNode;
		data.IntValue = this.attack;
		data.ActionType = SelectableActionType.Attack;
		GameLoopManager.Instance.PerformAction(data);
	}

	/// Raises a move action
	/// Note:
	/// Should change MovesLeft variable to properly reflect
	/// the move instead of just decrementing the value
	protected virtual void performMove(PlaceholderNode targetNode)
	{
		HeavyGameEventData data = new HeavyGameEventData();
		data.SourceNode = this.ParentNode;
		data.TargetNode = targetNode;
		/// Should increase by how far moved
		this.MovesLeft--;
		GameLoopManager.Instance.PerformAction(data);
	}

	/// Raises an enhance action
	protected virtual void performEnhance(string param)
	{
		HeavyGameEventData data = new HeavyGameEventData();
		data.SourceNode = this.ParentNode;
		data.TargetNode = this.ParentNode;
		data.StringValue = param;
		GameLoopManager.Instance.PerformAction(data);
	}

	/// Returns list of valid actions based on the current turn state
	public override List<SelectableActionType> GetValidActionTypes()
	{
		List<SelectableActionType> actionTypes = new List<SelectableActionType>(this.validActionTypes);
		if(this.MovesLeft <= 0)
		{
			actionTypes.Remove(SelectableActionType.Move);
		}
		return actionTypes;
	}

	/// processes performed action
	/// if the unit is damaged, for example,
	/// this is where it could react to that
	public virtual void processAction(HeavyGameEventData data)
	{
	}
}