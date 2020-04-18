using UnityEngine;

/// A basic attack animation
public class BasicAttackAnimation : UnitAnimation
{

	private Rigidbody rb;

	[SerializeField] protected float attackSpeed;

	public override void SetupAnimation(HeavyGameEventData data)
	{
		this.rb = this.GetComponent<Rigidbody>();
		this.transform.LookAt(data.SourceCell.transform.position);
		this.rb.velocity = Vector3.Normalize(data.TargetCell.transform.position - data.SourceCell.transform.position) * this.attackSpeed;
	}
}