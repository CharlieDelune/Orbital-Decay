using UnityEngine;
using UnityEngine.UI;

public class GeneralTurnDisplay : MonoBehaviour
{

	private PlayerFaction playerFaction;
	
	/// Parent GameObject for Player UI
	[SerializeField] private PlayerUI playerUI;

	private bool lastTurnWasPlayer = false;

	/// Label that will display the playing faction's
	/// FactionName field
	[SerializeField] private Text currentFactionLabel;

	[SerializeField] private Text currentRoundLabel;

	/// Sets the PlayerUI with the existing player faction
	/// Note: this method is not called if no PlayerFaction is
	/// found in the GameLoopManager's array of Factions
	public void SetPlayerFactionRef(PlayerFaction _playerFaction)
	{
		this.playerFaction = _playerFaction;
		this.playerUI.Set(this.playerFaction);
	}

	/// Updates the UI based on the current faction
	/// if null, this means this the turn belongs to
	/// the environment
	public void UpdateDisplay(Faction faction = null)
	{
		/// Hides Player's turn UI because player is
		/// no longer in player
		if(this.lastTurnWasPlayer)
		{
			this.playerUI.HidePlayerTurnUI();
			this.lastTurnWasPlayer = false;
		}

		/// Display current faction and round info
		if(faction != null)
		{
			this.currentFactionLabel.text = faction.FactionName;
			this.currentRoundLabel.text = "Round: " + GameLoopManager.Instance.TotalRounds;

			if(faction == this.playerFaction)
			{
				this.playerUI.DisplayPlayerTurnUI();
				this.lastTurnWasPlayer = true;
			}
		}
		/// Hide faction label because no faction is in play
		else
		{
			this.currentFactionLabel.text = "";
		}
	}
}