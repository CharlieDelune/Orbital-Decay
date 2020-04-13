using System;
using System.Collections.Generic;
using UnityEngine;

/// Container for individual resource displays
public class ResourcesPanel : FactionBaseListener
{

	[SerializeField] private Transform resourceRowsParent;
    [SerializeField] private ResourceRow resourceRowPrefab;

    public Faction TargetFaction;

    private Dictionary<InGameResource, ResourceRow> resourceRows = new Dictionary<InGameResource, ResourceRow>();

    /// Should listen to changes in Faction resources
    public override void OnRaise(Faction data)
    {
        if(data is PlayerFaction)
        {
        	FactionResources resources = data.Resources;
        	foreach(var (resource, quantity) in resources.GetResources())
        	{
                /// Instantiates Resource Row if not already present
                /// Later, this could be used for refined resources
                /// so that they would not be displayed at the start of the game
                /// when the player is far from obtaining them
        		if(!this.resourceRows.ContainsKey(resource))
        		{
        			ResourceRow row = Instantiate(this.resourceRowPrefab) as ResourceRow;
                    row.transform.SetParent(this.resourceRowsParent, false);
                    this.resourceRows[resource] = row;
        			this.resourceRows[resource].UpdateResourceNameLabel(resource.Name);
        		}
        		this.resourceRows[resource].UpdateAmountLabel(quantity);
        	}
        }
    }
}
