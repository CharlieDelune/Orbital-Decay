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

	private int nextTurn = 0;

	/// could be used by AI to plan moves
	public int NextTurn
	{
		get
		{
			return this.nextTurn;
		}
	}

	private int totalRounds = 0;
	public int TotalRounds
	{
		get
		{
			return this.totalRounds;
		}
	}

	public void StartGame()
	{
		if(this.Factions.Count <= 1)
		{
			throw new System.Exception("Number of Factions must be at least 2");
		}

		int playerFactionIndex = this.Factions.FindIndex(faction => faction is PlayerFaction);
		if(playerFactionIndex >= 0)
		{
			this.generalTurnDisplay.SetPlayerFactionRef((PlayerFaction)this.Factions[playerFactionIndex]);
		}

		this.Factions[0].StartTurn(this.next);
	}

	private void next()
	{
		/// End of round, when all Factions have gone
		if(this.nextTurn == this.Factions.Count)
		{
			this.nextTurn = 0;
			this.totalRounds++;
			// add solar system movement / orbits
			this.generalTurnDisplay.UpdateDisplay();
			this.next();
		}
		else
		{
			int currentTurn = this.nextTurn;
			this.nextTurn++;
			this.Factions[currentTurn].StartTurn(this.next);
			this.generalTurnDisplay.UpdateDisplay(this.Factions[currentTurn]);
		}
	}

	/// called whenever a faction is defeated
	public void OnFactionDefeated(Faction faction)
	{
		/// Decrements nextTurn field to preserve
		/// proper turn order
		if(this.Factions.IndexOf(faction) < this.nextTurn)
		{
			this.nextTurn--;
		}
		this.Factions.Remove(faction);

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
