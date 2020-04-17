using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Display for individual build options
/// Auto updates based on resource availability
public class BuildButton : MonoBehaviour
{

	private bool isActive;
	private List<(InGameResource, int)> requirements;
	[SerializeField] private Button button;
	[SerializeField] private ResourceRow resourceRowPrefab;
	[SerializeField] private Transform resourceRowsParent;
	private Faction faction;

	public GameObject SetButton(Faction faction, Action callback, UnitRecipe recipe)
	{
		this.requirements = recipe.Inputs;
		this.faction = faction;

		foreach(var (resource, quantity) in this.requirements)
    	{
			ResourceRow row = Instantiate(this.resourceRowPrefab) as ResourceRow;
            row.transform.SetParent(this.resourceRowsParent, false);
			row.UpdateResourceNameLabel(resource.Name);
    		row.UpdateAmountLabel(quantity);
    	}

		this.updateState();

		this.button.GetComponentsInChildren<Text>()[0].text = recipe.OutputName;
		this.button.onClick.AddListener(() => callback());

		return this.gameObject;
	}

	/// Should be connected to a FactionListener
	/// listening to the FactionGameEvent fired
	/// by the PlayerFaction's Resources
	public void OnFactionResourcesChange(Faction _faction)
	{
		if(_faction == this.faction)
		{
			this.updateState();
		}
	}

	private void updateState()
	{
		this.isActive = this.faction.Resources.HasResources(this.requirements);
		this.button.interactable = this.isActive;
	}
}