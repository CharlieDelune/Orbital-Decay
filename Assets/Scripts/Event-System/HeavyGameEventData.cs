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

	public GridCell SourceCell;
	public GridCell TargetCell;
	public CircularGrid targetGrid;

	public GameObject TargetObject;

	public InGameResource ResourceValue;

	public Recipe RecipeValue;

	public int IntValue;
	public bool BoolValue;
	public float FloatValue;
	public string StringValue;

	public SelectableActionType ActionType;
	public Selectable targetSelectable;

	public HeavyGameEventData(
		Faction sourceFaction = null,
		Faction targetFaction = null,
		GameObject targetObject = null,
		GridCell sourceCell = null,
		GridCell targetCell = null,
		InGameResource resourceValue = null,
		Recipe recipeValue = null,
		int intValue = 0,
		bool boolValue = false,
		float floatValue = 0.0f,
		string stringValue = "",
		Selectable targetSelectable = null,
		SelectableActionType actionType = SelectableActionType.None)
	{
		this.SourceFaction = sourceFaction;
		this.TargetFaction = targetFaction;
		this.TargetObject = targetObject;
		this.SourceCell = sourceCell;
		this.TargetCell = targetCell;
		this.ResourceValue = resourceValue;
		this.RecipeValue = recipeValue;
		this.IntValue = intValue;
		this.BoolValue = boolValue;
		this.FloatValue = floatValue;
		this.StringValue = stringValue;
		this.ActionType = actionType;
		this.targetSelectable = targetSelectable;
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