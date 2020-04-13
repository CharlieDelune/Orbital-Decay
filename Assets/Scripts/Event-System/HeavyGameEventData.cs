using System;
using UnityEngine;

/// Heavy because it carries a lot of information
/// This class' current fields are not definite
/// More can be added later, however, it's important
/// That is stays relatively confined

/// This class will be useful in ingame events
/// that have to do with Unit actions

[Serializable]
public class HeavyGameEventData
{

	public Faction SourceFaction;
	public Faction TargetFaction;

	public GridCell SourceNode;
	public GridCell TargetNode;

	public GameObject targetObject;

	public InGameResource ResourceValue;

	public int IntValue;
	public bool BoolValue;
	public float FloatValue;
	public string StringValue;

	public SelectableActionType ActionType;

	public HeavyGameEventData(
		Faction sourceFaction = null,
		Faction targetFaction = null,
		GridCell sourceNode = null,
		GridCell targetNode = null,
		InGameResource resourceValue = null,
		int intValue = 0,
		bool boolValue = false,
		float floatValue = 0.0f,
		string stringValue = "",
		SelectableActionType actionType = SelectableActionType.None)
	{
		this.SourceFaction = sourceFaction;
		this.TargetFaction = targetFaction;
		this.SourceNode = sourceNode;
		this.TargetNode = targetNode;
		this.ResourceValue = resourceValue;
		this.IntValue = intValue;
		this.BoolValue = boolValue;
		this.FloatValue = floatValue;
		this.StringValue = stringValue;
		this.ActionType = actionType;
	}
}

public enum SelectableActionType
{
	None,
	Attack,
	Build,
	Enhance,
	Move,
	ResourceGain,
	ResourceLose,
	Activate,
	Deactivate,
	ChangeGrid,
}