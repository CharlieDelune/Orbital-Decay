using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Origin for resources in the grid
public class ResourceDeposit : Selectable
{

	public List<GameResourceAmountPair> GameResourceAmountPairs;

	private List<(InGameResource, int)> output;

	private void Awake()
	{
		this.output = new List<(InGameResource, int)>();
		foreach(var pair in this.GameResourceAmountPairs)
		{
			output.Add((GameData.Instance.GetResource(pair.ResourceName), pair.Amount));
		}
	}

	public override bool CanPerformAction(SelectableActionType actionType, GridCell targetCell, string param)
	{
		return false;
	}

	public override void PerformAction(SelectableActionType actionType, GridCell targetCell, string param)
	{}

	public override void SetParentCell(GridCell cellIn)
	{
		this.ParentCell = cellIn;
		cellIn.ResourceDeposit = this;
		this.transform.position = cellIn.transform.position;
		this.transform.SetParent(cellIn.transform);
	}

	public List<(InGameResource, int)> GetResources()
	{
		return this.output;
	}
}

/// 
[Serializable]
public class GameResourceAmountPair
{
	public string ResourceName;
	public int Amount;
}