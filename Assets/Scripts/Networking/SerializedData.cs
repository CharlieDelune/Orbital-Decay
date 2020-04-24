using System;
using UnityEngine;

/// Serializable (networking-wise) container selectable actions

[Serializable]
public struct SerializedData
{

	/// -1 represents a solar system
	public int SourceCellPlanetIndex;
	public int SourceCellLayer;
	public int SourceCellSlice;

	public int TargetCellPlanetIndex;
	public int TargetCellLayer;
	public int TargetCellSlice;

	public string StringValue;

	public int ActionType;

	public int LocalIndex;

	public SerializedData(GridCell sourceCell, GridCell targetCell, int selectedAction, string param, int localIndex)
	{
		if(sourceCell != null)
		{
			this.SourceCellPlanetIndex = PlanetManager.Instance.planets.FindIndex(
				planet => planet == sourceCell.parentGrid.parentPlanet
			);
			this.SourceCellLayer = sourceCell.layer;
			this.SourceCellSlice = sourceCell.slice;
		}
		else
		{
			this.SourceCellPlanetIndex = -2;
			this.SourceCellLayer = -1;
			this.SourceCellSlice = -1;
		}

		if(targetCell != null)
		{
			this.TargetCellPlanetIndex = PlanetManager.Instance.planets.FindIndex(
				planet => planet == targetCell.parentGrid.parentPlanet
			);
			this.TargetCellLayer = targetCell.layer;
			this.TargetCellSlice = targetCell.slice;
		}
		else
		{
			this.TargetCellPlanetIndex = -2;
			this.TargetCellLayer = -1;
			this.TargetCellSlice = -1;
		}

		this.StringValue = param;

		this.ActionType = selectedAction;

		this.LocalIndex = localIndex;
	}
}