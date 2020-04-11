using UnityEngine;

/// Helper class to start game loop
public class GameStartHelper : MonoBehaviour
{
	private void Start()
	{
		GameStateManager.Instance.StartGame();
	}
}