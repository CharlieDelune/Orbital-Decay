using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// GameLoopManager is a singleton that handles
/// round-based gameplay
public class GameLoopManager : MonoBehaviour {

	/// Sets up class as Singleton

	private static GameLoopManager _instance;

	public static GameLoopManager Instance { get { return _instance; } }

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
}

public enum TurnState
{
	None,
	Animation,
}
