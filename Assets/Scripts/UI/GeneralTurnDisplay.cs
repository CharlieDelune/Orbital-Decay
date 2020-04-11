using UnityEngine;
using UnityEngine.UI;

/// Listens to changes to nextTurnVariable
public class GeneralTurnDisplay : MonoBehaviour
{

	/// Label that will display the playing faction's
	/// FactionName field
	[SerializeField] private Text currentFactionLabel;

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
			this.currentFactionLabel.text = faction.FactionName;
		}
		/// Hide faction label because no faction is in play
		else
		{
			this.currentFactionLabel.text = "";
		}
    }
}