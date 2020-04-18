using UnityEngine;

public class UnitAnimation : MonoBehaviour
{

	[SerializeField] private float defaultAnimationTime;

	/// Called on set up
	public virtual void SetupAnimation(HeavyGameEventData data)
	{
	}

	/// Returns the duration of the animation
	/// Some animations should have dynamic animation times, which is why
	/// this might be overriden in sub classes
	public virtual float GetAnimationDuration(HeavyGameEventData data)
	{
		return this.defaultAnimationTime;
	}
}