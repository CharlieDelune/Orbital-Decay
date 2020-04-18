using UnityEngine;
using UnityEngine.UI;

/// Controls how the specific resource is displayed
public class ResourceRow : MonoBehaviour
{

	[SerializeField] private Text resourceNameLabel;
	[SerializeField] private Text amountLabel;

	public void UpdateResourceNameLabel(string resourceName)
	{
		this.resourceNameLabel.text = resourceName.ToString();
	}
	
	public void UpdateAmountLabel(int amount)
	{
		this.amountLabel.text = amount.ToString();
	}
}