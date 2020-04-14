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

	[SerializeField] private MonoBehaviourGameEvent onCellSelectedEvent;
	[SerializeField] private MonoBehaviourGameEvent onTargetSelectedEvent;
	[SerializeField] private FactionGameEvent onPlayerTurnEndedEvent;

	private bool isPlayerTurn;
	private List<Faction> factions;
	private GridCell selectedCell;
	private CircularGrid gridInView;
	private SelectableActionType selectedAction;

	private void Awake()
	{

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

			Ray ray = Camera.main.ScreenPointToRay(mousePosition);
			RaycastHit rHit;
			Physics.Raycast(ray, out rHit, Mathf.Infinity, LayerMask.GetMask("Grid"));
			Vector3 hitPoint = rHit.point;

			GridCell clickedCell = this.gridInView.GetGridCell(hitPoint);

			/// Mouse was clicked outside the grid
			if(clickedCell == null)
			{
				onCellSelectedEvent.Raise(null);
				this.selectablePopup.gameObject.SetActive(false);
			}
			else
			{
				/// If there is a selected cell
				if(this.isPlayerTurn && selectedCell != null)
				{
					onTargetSelectedEvent.Raise(clickedCell);
				}
				else
				{
					selectedCell = clickedCell;
					onCellSelectedEvent.Raise(clickedCell);
					if(selectedCell.Selectable != null)
					{
						ShowPopUp(clickedCell);
					}
				}
			}
	    }
    }

    private void onChange()
    {
    	this.onPlayerViewStateChangeEvent.Raise(this);
    }

	private void ShowPopUp(GridCell cell)
	{
		this.selectablePopup.gameObject.SetActive(true);
		this.selectablePopup.LoadPopup(cell.Selectable);
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
		this.selectedCell = gs.SelectedCell;
		this.gridInView = gs.GridInView;
		this.selectedAction = gs.SelectedAction;

		if(gs.SelectedCell == null)
		{ 
			HidePopUp();
		}
	}
}