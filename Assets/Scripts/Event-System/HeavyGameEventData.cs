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

	public PlaceholderNode SourceNode;
	public PlaceholderNode TargetNode;

	public int IntValue;
	public bool BoolValue;
	public float FloatValue;
	public string StringValue;

	public SelectableActionType ActionType;
}

public enum SelectableActionType
{
	Attack,
	Build,
	Enhance,
	Move,
	ResourceGain,
	ResourceLose,
}