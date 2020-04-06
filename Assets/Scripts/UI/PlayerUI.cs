using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

	/// UI that will be on the screen only during the player's
	/// turn
	[SerializeField] private GameObject toggleableOnTurnPlayerUI;
	[SerializeField] private Button endTurnButton;

	private PlayerFaction playerFaction;

	/// This will be used later on
	/// for the resource tab set up
	public void Set(PlayerFaction _playerFaction)
	{
		this.playerFaction = _playerFaction;
		this.endTurnButton.onClick.AddListener(this.playerFaction.EndTurn);
	}

	public void DisplayPlayerTurnUI()
	{
		this.toggleableOnTurnPlayerUI.SetActive(true);
	}

	public void HidePlayerTurnUI()
	{
		this.toggleableOnTurnPlayerUI.SetActive(false);
	}
}