using UnityEngine;

public class PlaceholderGrid : MonoBehaviour
{

	[SerializeField] private PlaceholderNode placeholderNode;

	public bool IsSolarSystem = false;

	public PlaceholderNode GetNode(Vector3 position)
	{
		return this.placeholderNode;
	}
}