using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : Selectable
{

	/// Basic stats
	protected int maxRange;
	protected int health;
	protected int armor;
	protected int attack;
	protected bool baseStatsSet;

    private bool moving = false;
    private List<Vector3> targetPath;
    private int currentPathIndex;
    private List<GridCell> cellPath;
    private bool tapped;
    private int movesMade;

    /// When not active, maybe it could display a faded out
	/// unit in the game - active state mostly applies to certain units
	/// ex - refineries
	public virtual bool IsActive { get => true; }

    private GridCell target;

	[SerializeField] protected MonoBehaviourGameEvent onUnitDestroyed;

	public Faction Faction;
	private GridCell parentCell;

	/// for quick debugging
	[SerializeField] private bool debug = false;
	public string ExtraOverride;

	/// Turn State Fields
	/// Reset in UpdatePreTurn

	public int movesLeft = 0;

	private void Awake()
	{
		this.baseStatsSet = false;
	}

	private void Start()
	{
		if(this.debug)
		{
			this.setExtra(this.ExtraOverride);
		}
		if(!this.baseStatsSet && !this.debug)
		{
			throw new System.Exception("Unit was instantiated, but no base stats were set. Make sure you are instantiating through UnitInfo");
		}
	}

    private void Update()
    {
        if (this.moving)
        {
            this.HandleMovement();
        }
    }

	public virtual void SetBaseStats(string name, int maxRange, int health, int armor, int attack, string extra) {
		gameObject.name = name;
		this.maxRange = maxRange;
		this.health = health;
		this.armor = armor;
		this.attack = attack;

		this.baseStatsSet = true;

		this.setExtra(extra);
	}

	/// Further initialization based on the extra parameter
	/// Useful for subchildren of the Unit class
	protected virtual void setExtra(string extra)
	{
	}

	/// Attached to the NextTurn IntProperty
	/// Updates / Resets the turn fields for the Unit to get ready for the turn
	public virtual void OnStartFactionTurn(Faction faction)
	{
		if(faction == this.Faction)
		{
			this.updatePreTurn();
		}
	}

	/// Called at the start of the turn
	protected virtual void updatePreTurn()
	{
		this.movesLeft = this.maxRange;
		this.tapped = false;
        this.movesMade = 0;
	}

	/// Connected by a HeavyGameEventListener
	/// Initial check to see if the fired HeavyGameEventData has to do with this unit
	public virtual void OnFilterReceiveGameEvent(HeavyGameEventData data)
	{
		if(data.TargetNode == this.ParentNode)
		{
			this.receiveAction(data);
		}
	}

	/// Returns whether or not the action can be performed
	public override bool CanPerformAction(SelectableActionType actionType, GridCell targetNode, string param)
	{
		if(this.GetValidActionTypes().Contains(actionType))
		{
			switch(actionType)
			{
				case SelectableActionType.Attack:
					/// Attacks if the targetNode has a selectable and if the Faction belonging to the selectable is
					/// not the current faction
					return targetNode.Selectable is Unit && ((Unit)targetNode.Selectable).Faction != this.Faction;
				case SelectableActionType.Move:
					/// Moves if the targetNode is empty
					return targetNode.Selectable == null;
			}
		}
		return false;
	}

	public override void PerformAction(SelectableActionType actionType, GridCell targetNode, string param)
	{
		switch(actionType)
		{
			case SelectableActionType.Attack:
				this.performAttack(targetNode);
			break;
			case SelectableActionType.Move:
				this.performMove(targetNode);
			break;
		}
	}

	/// Raises an attack action
	protected virtual void performAttack(GridCell targetNode)
	{
		HeavyGameEventData data = new HeavyGameEventData();
		data.SourceNode = this.ParentNode;
		data.TargetNode = targetNode;
		data.IntValue = this.attack;
		data.ActionType = SelectableActionType.Attack;
		GameStateManager.Instance.PerformAction(data);
	}

	/// Raises a move action
	/// Note:
	/// Should change MovesLeft variable to properly reflect
	/// the move instead of just decrementing the value
    protected virtual void performMove(GridCell targetIn)
    {
        HeavyGameEventData data = new HeavyGameEventData();
		data.SourceNode = this.ParentNode;
		data.TargetNode = targetIn;
        this.target = targetIn;
        this.currentPathIndex = 0;
        this.cellPath = GridManager.Instance.pathfinder.FindPath(parentCell.layer, parentCell.slice, targetIn.layer, targetIn.slice);
        this.targetPath = GridManager.Instance.pathfinder.FindVectorPath(cellPath);
        this.moving = true;
        GameStateManager.Instance.PerformAction(data);
    }

    /// Should be implemented in base Unit class for
    /// leveling up / upgrading Units
    protected virtual void performEnhance(string param)
    {}

    public void SetParentCell(GridCell cellIn)
    {
        this.parentCell = cellIn;
		cellIn.Selectable = this;
    }

    public GridCell GetParentCell()
    {
        return parentCell;
    }

    public void SetMaxRange(int maxRangeIn)
    {
        this.maxRange = maxRangeIn;
    }

    public int GetMaxRange()
    {
        return maxRange;
    }
    
    private void HandleMovement()
    {
		if(!this.tapped)
		{
			if (this.targetPath != null && this.currentPathIndex < this.targetPath.Count)
			{
				Vector3 targetPosition = this.targetPath[this.currentPathIndex];
				float step = 30 * Time.deltaTime;
				//TODO: Change from .Distance to sqr magnitudes to save on calculation
				if (Vector3.Distance(this.transform.position, targetPosition) > step)
				{
					Vector3 moveDir = (targetPosition - this.transform.position).normalized;

					//TODO: Change from .Distance to sqr magnitudes to save on calculation
					float distanceBefore = Vector3.Distance(this.transform.position, targetPosition);
					this.transform.position = this.transform.position + moveDir * 30.0f * Time.deltaTime;
				}
				else
				{
					this.cellPath[this.currentPathIndex].Selectable = null;
					this.currentPathIndex++;
					if(this.currentPathIndex > 1)
					{
						this.movesMade++;
						this.movesLeft--;
					}
					if (this.movesMade == this.maxRange)
					{
						this.tapped = true;
					}
					if (this.currentPathIndex == this.maxRange+1 && !(this.currentPathIndex >= this.targetPath.Count+1))
					{
						this.endMoveInMiddle();
					}
					if (Vector3.Distance(this.transform.position, this.target.transform.position) <= 0.05f)
					{
						this.endMove();
					}
				}
			}
		}
    }

	/// Returns list of valid actions based on the current turn state
	public override List<SelectableActionType> GetValidActionTypes()
	{
		List<SelectableActionType> actionTypes = new List<SelectableActionType>(this.validActionTypes);
		if(this.movesLeft <= 0)
		{
			actionTypes.Remove(SelectableActionType.Move);
		}
		return actionTypes;
	}

	/// processes performed action
	/// if the unit is damaged, for example,
	/// this is where it could react to that
	public virtual void receiveAction(HeavyGameEventData data)
	{
		if(data.ActionType == SelectableActionType.Attack)
		{
			this.health -= data.IntValue;
			if(this.health < 0)
			{
				this.onUnitDestroyed.Raise(this);
			}
		}
	}
    
    private void endMove()
    {
        this.moving = false;
        this.SetParentCell(this.target);
        this.currentPathIndex = 1;
        this.transform.position = this.target.transform.position;
        this.target = null;
    }

    private void endMoveInMiddle()
    {
        this.moving = false;
        this.transform.position = this.targetPath[this.currentPathIndex-1];
        SetParentCell(this.cellPath[this.currentPathIndex-1]);
        this.targetPath = this.targetPath.Skip(this.currentPathIndex-1).ToList();
        this.cellPath = this.cellPath.Skip(this.currentPathIndex-1).ToList();
    }

    public void TakeOutstandingMoves()
    {
		if (!this.tapped)
		{
			if(this.target != null)
			{
				this.performMove(this.target);
			}
		}
    }
}
