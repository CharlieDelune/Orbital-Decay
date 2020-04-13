using System.Collections;
using UnityEngine;

/// Manages game initialization
/// Gets rid of the uncertainty of initialization (Start) method invocation order
public class EntryPoint : MonoBehaviour
{

	[SerializeField] private LevelCreator levelCreator;

	private void Start()
	{
		StartCoroutine(this.delayedStart(1.0f));
	}

	private IEnumerator delayedStart(float time)
	{
		yield return new WaitForSeconds(time);
		this.levelCreator.StartLevelCreation();
		yield return new WaitForSeconds(time);
		GameStateManager.Instance.StartGame();
	}
}