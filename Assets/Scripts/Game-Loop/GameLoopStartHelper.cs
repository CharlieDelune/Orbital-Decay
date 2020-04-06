using UnityEngine;

/// Helper class to start game loop
public class GameLoopStartHelper : MonoBehaviour
{
	private void Start()
	{
		GameLoopManager.Instance.StartGame();
	}
}