using UnityEngine;

[System.Serializable]
public class PlaceholderNode : MonoBehaviour
{
	public Selectable Selectable;
	public bool IsOnPlanetGridEdge = false;
	public PlaceholderGrid ParentGrid;
}