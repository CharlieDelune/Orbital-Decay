using UnityEngine;

public class PlaceholderGrid : MonoBehaviour
{
	[SerializeField] private PlaceholderNode[] placeholderNodes;

	public bool IsSolarSystem = false;

	public PlaceholderNode GetNode(Vector3 position)
	{
		PlaceholderNode bestNode = null;
		float closestSqrDistance = Mathf.Infinity;
		foreach(PlaceholderNode node in placeholderNodes)
		{
			Vector3 directionToTarget = node.transform.position - position;
			float dSqrToTarget = directionToTarget.sqrMagnitude;
			if (dSqrToTarget < closestSqrDistance)
			{
				closestSqrDistance = dSqrToTarget;
				bestNode = node;
			}
		}
		return bestNode;
	}
}