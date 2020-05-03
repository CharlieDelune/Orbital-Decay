using UnityEngine;

public class PlanetName : MonoBehaviour
{

	private Planet planet;
	[SerializeField] private Vector3 offset;
	[SerializeField] private TextMesh[] nameLabels;

	public void Setup(Planet _planet)
	{
		this.planet = _planet;
		this.transform.SetParent(this.planet.transform);
		this.transform.localPosition = this.offset;
		this.updateText();
	}

	/// Connected to BoolListener
	/// Listens to GameStateManager.InnerZoomed change
	public void OnInnerZoomedChange(bool zoomed)
	{
		if(zoomed)
		{
			foreach(TextMesh nameLabel in nameLabels)
			{
				nameLabel.gameObject.SetActive(true);
			}
		}
		else
		{
			foreach(TextMesh nameLabel in nameLabels)
			{
				nameLabel.gameObject.SetActive(false);
			}
		}
	}

	private void updateText()
	{
		foreach(TextMesh nameLabel in nameLabels)
		{
			nameLabel.text = this.planet.planetName;
		}
	}

	private void Update()
	{
		this.transform.rotation = Camera.main.transform.rotation;
	}
}