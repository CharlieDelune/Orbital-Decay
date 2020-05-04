using UnityEngine;
using UnityEngine.UI;

/// Listens to changes to nextTurnVariable
public class WaitMessageDisplay : MonoBehaviour
{

	/// Label that will display the playing faction's
	/// FactionName field
	[SerializeField] private GameObject waitTextHolder;
	[SerializeField] private Text waitText;

	private bool fading;
	private float oscillation = 2.0f;

	void Update()
	{
		Color newCol = waitText.color;
		if (newCol.a >= 1)
		{
			fading = true;
		}
		if (newCol.a <= 0.5)
		{
			fading = false;
		}
		if (fading)
		{
			newCol.a -= Time.deltaTime / oscillation;
		}
		else
		{
			newCol.a += Time.deltaTime / oscillation;
		}
		waitText.color = newCol;
	}

	/// Connected by an IntListener
	/// Updates the UI based on the next faction (subtract one to obtain the current playing)
	/// if null, this means this the turn belongs to
	/// the environment
	public void OnChangeTurn(int nextTurn)
    {
    	Faction faction = (nextTurn == 0) ? null : GameStateManager.Instance.Factions?[nextTurn - 1];

		/// Display current faction and round info
		if(faction != null)
		{
			if (faction is PlayerFaction)
			{
				waitTextHolder.SetActive(false);
			}
			else
			{
				waitTextHolder.SetActive(true);
			}
		}
    }
}