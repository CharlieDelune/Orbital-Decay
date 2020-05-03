using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// Faction would hold reference to all of its
/// units
public abstract class Faction : MonoBehaviour
{
	public string FactionName;

	public Color factionColor = Color.red;

	public FactionResources Resources;

	public List<Unit> units = new List<Unit>();

	[SerializeField] protected FactionGameEvent onFactionResourcesChange;

	[SerializeField] protected FactionGameEvent onStartFactionTurn;
	[SerializeField] protected FactionGameEvent onEndFactionTurn;

	private FactionIdentity identity;
	public FactionIdentity Identity { get => this.identity; }

	private int index;
	public int Index { get => this.index; }

	[SerializeField] protected FactionGameEvent onFactionDefeated;

	public bool isDefeated;

	public void SetIdentity(FactionIdentity _identity)
	{
		this.identity = _identity;
	}

	public void Setup(int _index)
	{
		this.index = _index;
		if(this.onFactionResourcesChange != null)
		{
			this.Resources.Set(() => {this.onFactionResourcesChange.Raise(this);}, this);
		}
		else
		{
			this.Resources.Set(() => {}, this);
		}
	}

	/// Setup before the Faction starts their turn
	/// Ideally after all units preload their states
	public virtual void StartTurn(Faction faction)
	{
		if(faction != null && faction == this)
		{
			this.Resources.OnPreLoadRound();
			this.StartCoroutine(this.useTurn());
		}
	}

	public void CreateUnit(GridCell cell, string unitName)
	{
		Unit unit = GameData.Instance.GetUnitInfo(unitName).InstantiateUnit(this);
		unit.SetParentCell(cell);
		unit.transform.position = cell.transform.position;
		unit.transform.SetParent(GameStateManager.Instance.UnitHolder.transform);
		unit.isPlayerUnit = this.Identity.isLocalPlayer;
		if (unit.unitType != UnitType.Mine && unit.unitType != UnitType.Wall)
		{
			this.units.Add(unit);
		}
		UnitManager.Instance.AddUnit(unit, this.Identity.isLocalPlayer);
	}

	public void RemoveUnit(Unit unit)
	{
		this.units.Remove(unit);
		if (units.Count == 0)
		{
			this.onFactionDefeated.Raise(this);
		}
	}

	public void AddUnit(Unit unitIn)
	{
		units.Add(unitIn);
	}

	public virtual void processReceivedAction(HeavyGameEventData data)
	{
		if(data.TargetFaction == this)
		{
			switch(data.ActionType)
			{
				case SelectableActionType.ResourceGain:
					this.Resources.GainResource(data.ResourceValue, data.IntValue);
				break;
				case SelectableActionType.Build:
					this.CreateUnit(data.TargetCell, data.RecipeValue.OutputName);
				break;
			}
		}
	}

	protected abstract IEnumerator useTurn();

	/// Causes the end of the turn
	public virtual void EndTurn()
	{
		this.onEndFactionTurn.Raise(this);
	}

	/// Called when the unit's move is ended
	public virtual void OnEndMove()
	{
	}

	void Awake() {
		factionColor = UnityEngine.Random.ColorHSV(0f,1f, 0.7f,0.7f, 0.9f,0.9f);
	}
}
