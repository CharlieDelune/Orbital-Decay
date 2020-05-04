using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

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

	[SerializeField] private GameObject gameOverPanel;
	[SerializeField] private Text gameOverText;
	[SerializeField] private GameObject solarSystemButton;
	[SerializeField] private GameObject loadingPanel;

	private bool isPlayerTurn;
	private List<Faction> factions;
	private GridCell selectedCell;
	private CircularGrid gridInView;
	private SelectableActionType selectedAction;

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
        this.toggleableOnTurnPlayerUI.SetActive(GameStateManager.Instance.IsPlayerTurn);
    }

    /// Handles click on the screen
    public void OnPointerClick(PointerEventData pointerEventData)
    {
    	if(this.gridInView != null && !(this.isPlayerTurn && GameStateManager.Instance.AnimationPresent))
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
				HidePopUp();
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
					if(selectedCell.Selectable != null || selectedCell.ResourceDeposit != null)
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
		this.selectablePopup.LoadPopup((cell.Selectable != null) ? cell.Selectable : cell.ResourceDeposit);
	}

	private void HidePopUp()
	{
		this.selectablePopup.gameObject.SetActive(false);
	}

	public void OnTryChangeGridEvent(MonoBehaviour grid)
	{
		this.selectedCell = null;
		this.selectedAction = SelectableActionType.None;
		HidePopUp();
	}

	public void OnGameEnded(bool playerWon)
	{
		gameOverPanel.SetActive(true);
		if(playerWon)
		{
			gameOverText.text = "YOU WIN!";
		}
		else
		{
			gameOverText.text = "YOU LOSE!";
		}
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
		if(gs.SelectedAction == SelectableActionType.None)
		{
			this.ResetGridInViewTiles();
		}

		if(gs.GridInView != null)
		{
			if (gs.GridInView.isSolarSystem)
			{
				solarSystemButton.SetActive(false);
			}
			else
			{
				solarSystemButton.SetActive(true);
			}
		}
	}

	public void OnActionSelected(int actionInt)
	{
		this.ResetGridInViewTiles();
		
		SelectableActionType selectedAction = (SelectableActionType)actionInt;

		if(selectedAction == SelectableActionType.Move)
		{
			this.ColorGridTiles(this.gridInView.GetGridCells().Where(c => c.ResourceDeposit == null && c.Selectable == null).ToList(), Constants.tileMove);
			this.ColorGridTiles(this.gridInView.GetCellsInRange(this.selectedCell, ((Unit)this.selectedCell.Selectable).GetMaxRange())
				.Where(c => c.ResourceDeposit == null && c.Selectable == null).ToList(), Constants.tileMoveSingleTurn);
		}
		if(selectedAction == SelectableActionType.Build)
		{
			this.ColorGridTiles(this.gridInView.GetGridCells().Where(c => c.ResourceDeposit != null).ToList(), Constants.tileBuild);
			this.ColorGridTiles(this.gridInView.GetCellsInRange(this.selectedCell, 1), Constants.tileBuild);
		}
		if(selectedAction == SelectableActionType.Attack)
		{
			this.ColorGridTiles(this.gridInView.GetGridCells().Where(c => c.Selectable != null 
				&& c.Selectable is Unit 
				&& ((Unit)c.Selectable).Faction != ((Unit)this.selectedCell.Selectable).Faction).ToList(), Constants.tileAttack);
			this.ColorGridTiles(this.gridInView.GetCellsInRange(this.selectedCell, ((Unit)this.selectedCell.Selectable).GetAttackRange()), Constants.tileAttack);
		}
		this.selectedCell.tileShadingHandler.ResetColor();
	}

	public void OnChangeAnimationPresent(bool animationPresent)
	{
		this.endTurnButton.interactable = !animationPresent;
	}

	public void ColorGridTiles(List<GridCell> cells, Color color)
	{
		foreach (GridCell cell in cells)
		{
			if (!cell.isEdgeCell)
			{
				cell.tileShadingHandler.SetCurrentColor(color);
			}
		}
	}

	public void ResetGridInViewTiles()
	{
		if (this.gridInView != null)
		{
			foreach (GridCell cell in this.gridInView.gridCells)
			{
				if (!cell.isEdgeCell)
				{
					cell.tileShadingHandler.ResetColor();
				}
			}
		}
	}

	public void ToggleLoadScreen(bool toggle)
	{
		loadingPanel.SetActive(toggle);
	}
}