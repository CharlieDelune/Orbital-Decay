using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Selectable : MonoBehaviour
{

	public String PopupLabel;
	public String PopupDescription;

	public virtual bool TryPerformAction(SelectableActionType actionType, GridCell targetCell, string param)
	{
		if(this.CanPerformAction(actionType, targetCell, param))
		{
			this.PerformAction(actionType, targetCell, param);
			return true;
		}
		return false;
	}
	
	public abstract bool CanPerformAction(SelectableActionType actionType, GridCell targetCell, string param);

	public abstract void PerformAction(SelectableActionType actionType, GridCell targetCell, string param);

	public abstract void SetParentCell(GridCell cellIn);

	public virtual List<SelectableActionType> GetValidActionTypes()
	{
		return new List<SelectableActionType>(this.validActionTypes);
	}

	/// Should be set on the prefab rather than programmatically
	[SerializeField] protected SelectableActionType[] validActionTypes = {};

	public GridCell ParentCell;

	public SelectableType selectableType;

	public HeavyGameEvent onMoveEvent;
}
public enum SelectableType{
	None,
	Unit,
	Planet
}