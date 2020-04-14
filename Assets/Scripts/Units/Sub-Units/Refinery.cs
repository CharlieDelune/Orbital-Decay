using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Should have a lower priority than Miners to allow for dependence on
/// the resource queue

public class Refinery : Unit
{

	[SerializeField] public HeavyGameEvent onProduceRefineryOutput;

	private InGameResource outputInGameResource;
	private int outputQuantity;
	private List<(InGameResource, int)> inputRequirements;

	private bool isActive = true;

	public override bool IsActive { get => this.isActive; }

	protected override void setExtra(string extra)
	{
		Debug.Log(GameData.Instance);
		ResourceRecipe recipe = (ResourceRecipe)GameData.Instance.GetRecipe(extra);
		this.inputRequirements = recipe.Inputs;
		this.outputQuantity = recipe.OutputQuantity;
		this.outputInGameResource = GameData.Instance.GetResource(recipe.OutputName);
	}

	public override bool CanPerformAction(SelectableActionType actionType, GridCell targetCell, string param)
	{
		return actionType == SelectableActionType.Enhance && targetCell?.Selectable == this;
	}

	protected override void updatePreTurn()
	{
		base.updatePreTurn();

		/// Only attempts refining if it is active
		if(this.IsActive)
		{

			/// If there is enough in the resourceQueue and the current resources
			/// then the required resources will be used up and
			/// an event will be fired to notify the Faction to gain
			/// the product resource
			if(this.Faction.Resources.TryUseFromQueue(this.inputRequirements))
			{
				/// this is an event rather than a direct change to the faction's
				/// Resources to allow for intercepting listeners that may want
				/// to modify this
				HeavyGameEventData data = new HeavyGameEventData(
					targetFaction: this.Faction,
					resourceValue: this.outputInGameResource,
					intValue: this.outputQuantity,
					actionType: SelectableActionType.ResourceGain
				);

				this.onProduceRefineryOutput.Raise(data);
			}

			/// Perhaps deactivate the Refinery if resources are not available
		}
	}

	/// "Enhancing" a refinery would be toggling it on and off
	protected override void performEnhance(string param)
	{
		this.isActive = !this.isActive;
		HeavyGameEventData data = new HeavyGameEventData(
			targetCell: this.ParentCell,
			actionType: ((this.IsActive) ? SelectableActionType.Deactivate : SelectableActionType.Activate)
		);
		GameStateManager.Instance.PerformAction(data);
	}
}