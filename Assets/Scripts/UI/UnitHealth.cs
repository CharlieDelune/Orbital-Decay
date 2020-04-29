using UnityEngine;

public class UnitHealth : MonoBehaviour
{

	private Unit unit;
	[SerializeField] private Vector3 offset;
	[SerializeField] private TextMesh healthLabel;

	public void Setup(Unit _unit)
	{
		this.unit = _unit;
		this.transform.SetParent(this.unit.transform);
		this.transform.localPosition = this.offset;
		this.updateHealth();
	}

	/// Connected to BoolListener
	/// Listens to GameStateManager.InnerZoomed change
	public void OnInnerZoomedChange(bool zoomed)
	{
		if(zoomed)
		{
			this.healthLabel.gameObject.SetActive(true);
		}
		else
		{
			this.healthLabel.gameObject.SetActive(false);
		}
	}

	/// Connected to HeavyGameEventListener
	/// listens to attack events
	/// Should have a low Priority
	public void OnUnitAttackedEvent(HeavyGameEventData data)
	{
		if(data.ActionType == SelectableActionType.Attack && data.TargetCell?.Selectable != null && data.TargetCell.Selectable as Unit == this.unit)
		{
			this.updateHealth();
		}
	}

	private void updateHealth()
	{
		this.healthLabel.text = this.unit.Health.ToString();
	}

	private void Update()
	{
		this.transform.rotation = Camera.main.transform.rotation;
	}
}