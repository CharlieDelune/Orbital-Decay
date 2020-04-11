using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// A container for the Player's turn state
public class PlayerViewManager : MonoBehaviour, IPointerClickHandler
{

	/// UI that will be on the screen only during the player's
	/// turn
	[SerializeField] private GameObject toggleableOnTurnPlayerUI;
	[SerializeField] private Button endTurnButton;

	private PlayerFaction playerFaction;

	[SerializeField] private SelectablePopup selectablePopup;
	[SerializeField] private MonoBehaviourGameEvent onPlayerViewStateChangeEvent;

	[SerializeField] private MonoBehaviourGameEvent onNodeSelectedEvent;
	[SerializeField] private MonoBehaviourGameEvent onTargetSelectedEvent;
	[SerializeField] private FactionGameEvent onPlayerTurnEndedEvent;

	private bool isPlayerTurn;
	private List<Faction> factions;
	private PlaceholderNode selectedNode;
	private PlaceholderGrid gridInView;
	private SelectableActionType selectedAction;

	private void Awake()
	{
		//isPlayerTurn = GameStateManager.Instance.IsPlayerTurn;
		//factions = GameStateManager.Instance.Factions;
		//selectedNode = GameStateManager.Instance.SelectedNode;
	}

	/// This will be used later on
	/// for the resource tab set up
	public void LoadPlayer(PlayerFaction _playerFaction)
	{
		this.playerFaction = _playerFaction;
		this.endTurnButton.onClick.AddListener(playerFaction.EndTurn);
	}

	/// Connected by an IntListener
	/// Called when the nextTurn variable is changed
	public void OnStartTurn(int nextTurn)
    {
        Faction faction = (nextTurn == 0) ? null : factions?[nextTurn - 1];
		if(GameStateManager.Instance.IsPlayerTurn)
		{
			this.toggleableOnTurnPlayerUI.SetActive(true);
		}
		else
		{
			this.toggleableOnTurnPlayerUI.SetActive(false);
		}
    }

    /// Handles click on the screen
    public void OnPointerClick(PointerEventData pointerEventData)
    {
    	if(!(this.isPlayerTurn && GameStateManager.Instance.AnimationPresent))
	    {
	        Vector3 mousePosition = pointerEventData.position;
	        PlaceholderNode clickedNode = this.gridInView.GetNode(mousePosition);

			/// Mouse was clicked outside the grid
			if(clickedNode == null)
			{
				onNodeSelectedEvent.Raise(null);
				this.selectablePopup.gameObject.SetActive(false);
			}
			else
			{
				/// If there is a selected node
				if(this.isPlayerTurn && selectedNode != null)
				{
					onTargetSelectedEvent.Raise(clickedNode);
				}
				else
				{
					selectedNode = clickedNode;
					onNodeSelectedEvent.Raise(clickedNode);
					ShowPopUp(clickedNode);
				}
			}
	    }
    }

    private void onChange()
    {
    	this.onPlayerViewStateChangeEvent.Raise(this);
    }

	private void ShowPopUp(PlaceholderNode node)
	{
		this.selectablePopup.gameObject.SetActive(true);
		this.selectablePopup.LoadPopup(node.Selectable);
	}

	private void HidePopUp()
	{
		this.selectablePopup.gameObject.SetActive(false);
	}

	public void OnGameStateManagerUpdated(MonoBehaviour _gameStateManager)
	{
		GameStateManager gs = (GameStateManager)_gameStateManager;
		this.isPlayerTurn = gs.IsPlayerTurn;
		this.factions = gs.Factions;
		this.selectedNode = gs.SelectedNode;
		this.gridInView = gs.GridInView;
		this.selectedAction = gs.SelectedAction;

		if(gs.SelectedNode == null)
		{ 
			HidePopUp();
		}
	}
}