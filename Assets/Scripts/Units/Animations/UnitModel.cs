using System;
using UnityEngine;

/// Controls the unit's model
/// Support for animations
[RequireComponent(typeof(Animator))]
public class UnitModel : MonoBehaviour
{

	protected Animator animator;

	protected Unit parentUnit;

	[SerializeField] protected ActionTypeAnimatorStatePair[] actionStatePairs;

	private void Awake()
	{
		this.animator = this.GetComponent<Animator>();
		this.parentUnit = this.transform.parent.GetComponent<Unit>();
	}

	public virtual void OnFilterReceiveAction(HeavyGameEventData data)
	{
		if(data.SourceCell.Selectable == this.parentUnit)
		{
			this.receiveAction(data);
		}
	}

	protected virtual void receiveAction(HeavyGameEventData data)
	{
		foreach(var actionStatePair in this.actionStatePairs)
		{
			if(actionStatePair.ActionType == data.ActionType)
			{
				///this.animator.SetInt(actionStatePair.ParamName, actionStatePair.AnimatorState);
				return;
			}
		}
	}
}

[Serializable]
public struct ActionTypeAnimatorStatePair
{
	public SelectableActionType ActionType;
	public string ParamName;
	public int AnimatorState;
}