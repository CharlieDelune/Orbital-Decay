using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Selectable : MonoBehaviour
{

	public virtual bool TryPerformAction(SelectableActionType actionType, GridCell targetNode, string param)
	{
		if(this.CanPerformAction(actionType, targetNode, param))
		{
			this.PerformAction(actionType, targetNode, param);
			return true;
		}
		return false;
	}
	
	public abstract bool CanPerformAction(SelectableActionType actionType, GridCell targetNode, string param);

	public abstract void PerformAction(SelectableActionType actionType, GridCell targetNode, string param);

	public abstract List<SelectableActionType> GetValidActionTypes();

	[SerializeField] protected SelectableActionType[] validActionTypes;

	public GridCell ParentNode;
}