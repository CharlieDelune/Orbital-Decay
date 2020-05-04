using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// GameLoopManager is a singleton that handles
/// round-based gameplay
public class GameStateManager : MonoBehaviour {

	/// Sets up class as Singleton

	private static GameStateManager _instance;
	public static GameStateManager Instance { get { return _instance; } }

	/// Raises Exception if multiple singleton instances are present at once
	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			throw new System.Exception("Multiple GameStateManager instances are present. There should only be one instance of a Singleton.");
		} else {
			_instance = this;
		}
	}

	/*
		View state related
	*/
	[SerializeField] private PlayerViewManager playerViewManager;

	[SerializeField] private BoolProperty animationPresent;
	public bool AnimationPresent
	{
		get => this.animationPresent.Value;
		set
		{
			this.animationPresent.Value = value;
		}
	}

	[SerializeField] private BoolProperty innerZoomed;
	public bool InnerZoomed
	{
		get => this.innerZoomed.Value;
		set
		{
			this.innerZoomed.Value = value;
		}
	}

	/// The grid the player's view is focused on
	[SerializeField] private CircularGrid gridInView;
	public CircularGrid GridInView
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

	/*
		Player action state
	*/

	/// The selected Cell
	private GridCell selectedCell;
	public GridCell SelectedCell
	{
		get => this.selectedCell;
		set
		{
			if(value != this.selectedCell)
			{
				this.selectedCell = value;
				this.onChange();
			}
		}
	}
	private SelectableActionType selectedAction;
	public SelectableActionType SelectedAction
	{
		get => this.selectedAction;
		set
		{
			if(value != this.selectedAction)
			{
				this.selectedAction = value;
				this.onChange();
			}
		}
	}

	[HideInInspector] public string SelectedBuildOption;

	/*
		Game loop related
	*/

	public List<Faction> Factions;
	private PlayerFaction playerFaction;

	/// Subscribable property, useful for UI changes
	[SerializeField] private IntProperty nextTurn;

	/// could be used by AI to plan moves
	public int NextTurn { get => this.nextTurn.Value; }

	[SerializeField] private IntProperty totalRounds;
	public int TotalRounds { get => this.totalRounds.Value; }

	[SerializeField] private BoolProperty isPlayerTurn;
	public bool IsPlayerTurn { 
		get => this.isPlayerTurn.Value; 
		set {
			this.isPlayerTurn.Value = value;
			this.onChange();
		}
	}

	[SerializeField] private HeavyGameEvent onPerformAction;
	[SerializeField] private MonoBehaviourGameEvent onGameStateUpdatedEvent;
	[SerializeField] private FactionGameEvent onStartFactionTurnEvent;
	[SerializeField] private BoolGameEvent onGameEnded;

	/*
		Misc
	*/

	public CircularGrid solarSystemGrid;

    public GameObject UnitHolder;

    public int seed { get; set; }

	/// Setups up GameStateManager's Factions list
	public void SetupFactions(List<Faction> factions)
	{

		this.Factions = factions;
		for(int i = 0; i < this.Factions.Count; i++)
		{
			Faction faction = this.Factions[i];
			if(faction.Identity.isLocalPlayer)
			{
				((PlayerFaction)faction).LoadPlayer();
			}
			faction.Setup(i);
			faction.FactionName = "Faction " + i;
		}
	}

	public void StartGame()
	{
		if(this.Factions.Count <= 0)
		{
			throw new System.Exception("Number of Factions must be at least 1");
		}

		this.totalRounds.SetValueNotify(0);
		this.animationPresent.SetValueNotify(false);
		this.nextTurn.SetValueNotify(this.Factions.Count);
		this.isPlayerTurn.SetValueNotify(false);
		this.innerZoomed.SetValueNotify(false);

		this.onChange();

		FindObjectsOfType<CameraMovement>()[0].enabled = true;

		SetLoading(false);

		this.OnEndTurn(null);
	}

	/// Connected to Faction Listener
	/// Should return the faction that just ended their turn
	public void OnEndTurn(Faction faction)
	{
		if(faction != null && faction.Identity.isLocalPlayer)
		{
			SelectedAction = SelectableActionType.None;
			SelectedCell = null;
		}
		this.IsPlayerTurn = false;
		this.next();
	}

	private void next()
	{
		/// End of round, when all Factions have gone
		if(this.nextTurn.Value == this.Factions.Count)
		{
			this.nextTurn.Value = 0;
			this.totalRounds.Value++;
			// PlanetManager.Instance.RevolveAllPlanets();
			this.onStartFactionTurnEvent.Raise(null);
			// this.next();
		}
		else
		{
			int currentTurn = this.nextTurn.Value;
			if(this.Factions[currentTurn].Identity.isLocalPlayer)
			{
				this.IsPlayerTurn = true;
			}
			else
			{
				this.IsPlayerTurn = false;
			}
			
			this.nextTurn.Value++;

			if (!this.Factions[currentTurn].isDefeated)
			{
				this.onStartFactionTurnEvent.Raise(this.Factions[currentTurn]);
			}
			else
			{
				this.next();
			}
		}
	}

	/// Raises the onPerformAction event that is listened to by
	/// units and factions
	public void PerformAction(HeavyGameEventData data)
	{
		this.onPerformAction.Raise(data);
	}

	/// Connected by a HeavyGameEventListener
	/// called whenever a faction is defeated
	public void OnFactionDefeated(Faction faction)
	{
		faction.isDefeated = true;

		//If the player faction was the one destroyed, you lose mate
		if (faction == playerFaction)
		{
			this.onGameEnded.Raise(false);
		}

		//If you got to here and didn't lose, you've obviously won, congrats
		if(this.Factions.Where((f) => !f.isDefeated).ToList().Count == 1)
		{
			this.onGameEnded.Raise(true);
		}
	}

	public void LoadPlayer(PlayerFaction _playerFaction)
	{
		this.playerFaction = _playerFaction;
		this.IsPlayerTurn = false;
		this.playerViewManager.LoadPlayer(_playerFaction);
	}

	public void OnCellSelected(MonoBehaviour _newCell)
	{
		this.SelectedBuildOption = "";
		this.selectedCell = (GridCell)_newCell;
	}

	public void OnActionSelected(int _action)
	{
		this.SelectedAction = (SelectableActionType)_action;
	}

	public void OnTargetSelected(MonoBehaviour _cell)
	{
		bool validAction = false;
		if(_cell != null)
		{
			GridCell targetCell = (GridCell)_cell;
			if(this.SelectedCell?.Selectable != null && SelectedAction != SelectableActionType.None && targetCell != null)
			{
				NetworkEventManager.Instance.TryPerformAction(this.SelectedCell.Selectable, (SelectableActionType)this.selectedAction, targetCell, this.SelectedBuildOption);
				validAction = this.SelectedCell.Selectable.TryPerformAction((SelectableActionType)this.selectedAction, targetCell, this.SelectedBuildOption);
			}
		}
		this.SelectedCell = null;
		this.SelectedAction = SelectableActionType.None;
	}

	private void onChange()
    {
    	this.onGameStateUpdatedEvent.Raise(this);
    }

	public void OnTryChangeGrid(MonoBehaviour _grid)
	{
		if (_grid == null)
		{
			this.GridInView = solarSystemGrid;
		}
		else
		{
			CircularGrid grid = (CircularGrid)_grid;
			this.GridInView = grid;
		}
		this.selectedCell = null;
		this.selectedAction = SelectableActionType.None;
	}

	public void OnUnitDestroyed(MonoBehaviour unitObject)
	{
		Destroy(unitObject.gameObject);
	}

	public void SetLoading(bool isLoading)
	{
		playerViewManager.ToggleLoadScreen(isLoading);
	}
}

public enum TurnState
{
	None,
	Animation,
}
