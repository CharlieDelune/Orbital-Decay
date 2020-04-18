using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Handles unit models / animations
public class UnitModel : MonoBehaviour
{

	protected Unit parentUnit;

	[SerializeField] protected UnitAnimationInfo[] unitActions;

	private bool runningAnimation = false;

	private List<UnitAnimation> presentAnimations = new List<UnitAnimation>();

	private void Awake()
	{
		this.parentUnit = this.transform.parent.GetComponent<Unit>();
	}

	/// Should be connected via a HeavyGameEventListener
	/// Processes the initial listened to HeavyGameEvent
	public virtual void OnFilterReceiveAction(HeavyGameEventData data)
	{
		if(data.SourceCell.Selectable == this.parentUnit)
		{
			this.receiveAction(data);
		}
	}

	/// Sets up the animations for the given action
	protected virtual void receiveAction(HeavyGameEventData data)
	{
		/// Will only do the setup if no animations are running from this UnitModel
		if(this.runningAnimation)
		{
			return;
		}

		float maxAnimationTime = -1.0f;
		
		for(int i = 0; i < this.unitActions.Length; i++)
		{
			/// Only appropriate UnitAnimations should be activated
			if(this.unitActions[i].TriggerActionType == data.ActionType)
			{
				maxAnimationTime = Math.Max(maxAnimationTime, this.unitActions[i].UnitAnimation.GetAnimationDuration(data));

				UnitAnimation animation = Instantiate(this.unitActions[i].UnitAnimation) as UnitAnimation;
				animation.transform.SetParent(this.transform);
				animation.transform.localPosition = Vector3.zero;
				animation.SetupAnimation(data);
				this.presentAnimations.Add(animation);
			}
		}
		/// Only plays an animation if the time is > 0
		if(maxAnimationTime > 0.0f)
		{
			StartCoroutine(this.runAnimation(maxAnimationTime));
		}
	}

	/// Handles set up and tear down for all UnitAnimations
	private IEnumerator runAnimation(float time)
	{
		this.runningAnimation = true;
		GameStateManager.Instance.AnimationPresent = true;
		yield return new WaitForSeconds(time);
		for(int i = 0; i < this.presentAnimations.Count; i++)
		{
			Destroy(this.presentAnimations[i].gameObject);
		}
		this.presentAnimations.Clear();
		GameStateManager.Instance.AnimationPresent = false;
		this.runningAnimation = false;
	}
}

[Serializable]
public struct UnitAnimationInfo
{
	public SelectableActionType TriggerActionType;
	public UnitAnimation UnitAnimation;
}