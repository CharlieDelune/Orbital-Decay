using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// A container for the Player's turn state
public class PlayerViewState : MonoBehaviour, IPointerClickHandler
{

	/// The grid the player's view is focused on
	[SerializeField] private PlaceholderGrid gridInView;
	public PlaceholderGrid GridInView
	{
		get => this.gridInView;
		set
		{
			if(value != this.gridInView)
			{
				this.gridInView = value;
				this.onChange();
			}
		}
	}

	/// The selected Node
	private PlaceholderNode selectedNode;
	public PlaceholderNode SelectedNode
	{
		get => this.selectedNode;
		set
		{
			if(value != this.selectedNode)
			{
				if(value == null || value.Selectable == null)
				{
					this.selectablePopup.gameObject.SetActive(false);
				}
				else if(!(value.Selectable is Unit) || (value.Selectable is Unit && (((Unit)value.Selectable))?.Faction is PlayerFaction))
				{
					this.selectablePopup.gameObject.SetActive(true);
					this.selectablePopup.LoadPopup(value.Selectable);
				}
				this.selectedNode = value;
				this.onChange();
			}
		}
	}

	/// UI that will be on the screen only during the player's
	/// turn
	[SerializeField] private GameObject toggleableOnTurnPlayerUI;
	[SerializeField] private Button endTurnButton;

	private PlayerFaction playerFaction;


	[SerializeField] private SelectablePopup selectablePopup;
	[SerializeField] private MonoBehaviourGameEvent onPlayerViewStateChangeEvent;
	[SerializeField] private BoolProperty isPlayerTurn;

	private void Awake()
	{
		this.isPlayerTurn.Value = false;
	}

	/// This will be used later on
	/// for the resource tab set up
	public void LoadPlayer(PlayerFaction _playerFaction)
	{
		this.playerFaction = _playerFaction;
		this.endTurnButton.onClick.AddListener(this.playerFaction.EndTurn);
		this.isPlayerTurn.Value = false;
	}

	/// Connected by an IntListener
	/// Called when the nextTurn variable is changed
	public void OnStartTurn(int nextTurn)
    {
        Faction faction = (nextTurn == 0) ? null : GameLoopManager.Instance.Factions?[nextTurn - 1];

		if(faction == this.playerFaction)
		{
			this.isPlayerTurn.Value = true;
			this.toggleableOnTurnPlayerUI.SetActive(true);
		}
		else
		{
			this.isPlayerTurn.Value = false;
			this.toggleableOnTurnPlayerUI.SetActive(false);
		}
    }

    /// Handles click on the screen
    public void OnPointerClick(PointerEventData pointerEventData)
    {
    	if(!(this.isPlayerTurn.Value && GameLoopManager.Instance.AnimationPresent))
	    {
	        Vector3 mousePosition = pointerEventData.position;
	        PlaceholderNode clickedNode = this.GridInView.GetNode(mousePosition);

	    	/// Mouse was clicked outside the grid
	        if(clickedNode == null)
	        {
	        	this.selectedNode = null;
	        }
	    	else
	    	{
	    		/// If there is a selected node
	    		if(this.isPlayerTurn.Value && this.SelectedNode != null)
	    		{
	    			/// Passes the newly clicked node to the selectablePopup
	    			if(!this.selectablePopup.SelectGridNode(clickedNode))
	    			{
	    				this.SelectedNode = null;
	    			}
	    		}
	    		else
	    		{
	    			this.SelectedNode = clickedNode;
	    		}
	    	}
	    }
    }

    private void onChange()
    {
    	this.onPlayerViewStateChangeEvent.Raise(this);
    }
}