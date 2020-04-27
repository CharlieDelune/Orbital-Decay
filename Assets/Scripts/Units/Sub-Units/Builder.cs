using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Note: Miners should have a high priority level to the NextTurn
/// Specifically, should be higher priority than Refineries

public class Builder : Unit
{

	protected UnitRecipe[] buildOptions;
	/// Maybe used by Enemy Faction AI to dictate decision
	public UnitRecipe[] BuildOptions { get => this.buildOptions; }

	[SerializeField] protected string[] buildOptionLabels;
	public string[] BuildOptionLabels { get => this.buildOptionLabels; }

	protected UnitRecipe selectedBuildOption;

	/// Sets the build options based on the extra param
	/// An example of what the extra param would look like:
	/// RECIPE_Name:unit_button_label-RECIPE_NAME_2:unit_button_label2
	protected override void setExtra(string extra)
	{
		string[] buildOptionLabelPairs = extra.Split('-');
		this.buildOptions = new UnitRecipe[buildOptionLabelPairs.Length];
		this.buildOptionLabels = new string[buildOptionLabelPairs.Length];
		for(int i = 0; i < buildOptionLabelPairs.Length; i++)
		{
			string[] pair = buildOptionLabelPairs[i].Split(':');
			this.buildOptions[i] = (UnitRecipe) GameData.Instance.GetRecipe(pair[0]);
			this.buildOptionLabels[i] = pair[1];
		}
	}

	/// Returns whether or not the action can be performed
	public override bool CanPerformAction(SelectableActionType actionType, GridCell targetCell, string param)
	{
		if(this.GetValidActionTypes().Contains(actionType))
		{
			switch(actionType)
			{
				case SelectableActionType.Attack:
					/// Attacks if the targetNode has a selectable and if the Faction belonging to the selectable is
					/// not the current faction
					return targetCell.Selectable is Unit && ((Unit)targetCell.Selectable).Faction != this.Faction;
				case SelectableActionType.Move:
					/// Moves if the targetNode is empty
					return base.CanPerformAction(actionType, targetCell, param);
				case SelectableActionType.Build:
					if(param != null)
					{
						this.selectedBuildOption = this.BuildOptions[int.Parse(param)];
						bool canUseRecipe = this.faction.Resources.CanUseRecipe(this.selectedBuildOption);
						if(!canUseRecipe)
						{
							//TODO: Show this outside of the debugger
							Debug.Log("Insufficient resources!");
						}

						/// Make sure the target node has a ResourceDeposit if the buildOption is a Miner
						if(
							GameData.Instance.GetUnitInfo(this.selectedBuildOption.OutputName).UnitPrefab is Miner
							&& targetCell.ResourceDeposit == null)
						{
							return false;
						}
						return canUseRecipe && targetCell.Selectable == null && this.ParentCell.GetNeighbors().Contains(targetCell);
					}
					break;
			}
		}
		return false;
	}

	public override void PerformAction(SelectableActionType actionType, GridCell targetCell, string param)
	{
		switch(actionType)
		{
			case SelectableActionType.Attack:
				this.performAttack(targetCell);
			break;
			case SelectableActionType.Move:
				this.performMove(targetCell);
			break;
			case SelectableActionType.Build:
				this.performBuild(targetCell);
			break;
		}
	}

	/// Performs the build action
	protected virtual void performBuild(GridCell targetCell)
	{
		/// Causes the Faction's resources to be used to create the unit
		this.Faction.Resources.UseRecipe(this.selectedBuildOption);
		
		/// Offloads the actual building of the unit to a listener
		HeavyGameEventData data = new HeavyGameEventData(
			sourceCell: this.ParentCell,
			targetCell: targetCell,
			targetFaction: this.Faction,
			actionType: SelectableActionType.Build,
			recipeValue: this.selectedBuildOption
		);

		GameStateManager.Instance.PerformAction(data);
	}
}
