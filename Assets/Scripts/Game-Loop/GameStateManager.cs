using System;
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
			throw new System.Exception("Multiple GameLoopManager instances are present. There should only be one instance of a Singleton.");
		} else {
			_instance = this;
		}
	}

	[SerializeField] private GeneralTurnDisplay generalTurnDisplay;

	public List<Faction> Factions;

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

	[SerializeField] private BoolProperty animationPresent;
	public bool AnimationPresent
	{
		get => this.animationPresent.Value;
		set
		{
			this.animationPresent.Value = value;
		}
	}

	[SerializeField] private HeavyGameEvent onPerformAction;

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
				this.selectedNode = value;
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

	[SerializeField] private MonoBehaviourGameEvent onGameStateUpdatedEvent;
	private PlayerFaction playerFaction;

	public void StartGame()
	{
		if(this.Factions.Count <= 1)
		{
			throw new System.Exception("Number of Factions must be at least 2");
		}

		int playerFactionIndex = this.Factions.FindIndex(faction => faction is PlayerFaction);
		if(playerFactionIndex >= 0)
		{
			((PlayerFaction)this.Factions[playerFactionIndex]).LoadPlayer();
		}

		this.totalRounds.Value = 0;
		this.animationPresent.Value = false;
		this.nextTurn.Value = 0;
		this.isPlayerTurn.Value = false;

		this.onChange();

		this.Factions[0].StartTurn(this.next);
	}

	private void next()
	{
		/// End of round, when all Factions have gone
		if(this.nextTurn.Value == this.Factions.Count)
		{
			this.nextTurn.Value = 0;
			this.totalRounds.Value++;
			// add solar system movement / orbits
			this.next();
		}
		else
		{
			int currentTurn = this.nextTurn.Value;
			if(this.Factions[currentTurn] == playerFaction)
			{
				this.IsPlayerTurn = true;
			}
			else
			{
				this.IsPlayerTurn = false;
			}
			this.nextTurn.Value++;
			this.Factions[currentTurn].StartTurn(this.next);
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
		bool decrementNextTurn = false;
		if(this.Factions.IndexOf(faction) < this.nextTurn.Value)
		{
			decrementNextTurn = true;
		}
		/// Faction removal must be done before nextTurn.Value decrement
		this.Factions.Remove(faction);

		/// Decrements nextTurn field to preserve
		/// proper turn order
		if(decrementNextTurn)
		{
			this.nextTurn.Value--;
		}

		/// Ends the game when only one faction is remaining
		if(this.Factions.Count == 1)
		{
			this.endGame();
		}
	}

	/// Called when the game is over
	private void endGame()
	{
		throw new Exception("End Game!");
	}

	public void LoadPlayer(PlayerFaction _playerFaction)
	{
		this.playerFaction = _playerFaction;
		this.IsPlayerTurn = false;
	}

	public void OnNodeSelected(MonoBehaviour _newNode)
	{
		this.selectedNode = (PlaceholderNode)_newNode;
	}

	public void OnActionSelected(int _action)
	{
		this.selectedAction = (SelectableActionType)_action;
	}

	public void OnTargetSelected(MonoBehaviour _node)
	{
		bool validAction = false;
		PlaceholderNode targetNode = (PlaceholderNode)_node;
		if(this.SelectedNode.Selectable != null && SelectedAction != SelectableActionType.None && targetNode != null)
		{
			//Build is an empty string for now because we haven't implemented it yet
			validAction = this.SelectedNode.Selectable.TryPerformAction((SelectableActionType)this.selectedAction, targetNode, "");
			Debug.Log("Valid action: " + validAction);
		}
		this.SelectedNode = null;
		this.SelectedAction = SelectableActionType.None;
	}

	private void onChange()
    {
    	this.onGameStateUpdatedEvent.Raise(this);
    }

	public void OnPlayerTurnEnded()
	{
		playerFaction.EndTurn();
	}
}

public enum TurnState
{
	None,
	Animation,
}
