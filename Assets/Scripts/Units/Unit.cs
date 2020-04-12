using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : Selectable
{
	private int maxRange;
	private int health;
	private int armor;
	private int attack;

	private bool baseStatsSet;
    private bool moving = false;
    private GridCell target;
    private List<Vector3> targetPath;
    private int currentPathIndex;
    private GridCell parentCell;
    private List<GridCell> cellPath;
    private bool tapped;
    private int movesMade;

	/// for quick debugging
	[SerializeField] private bool debug = false;

	public Faction faction;

	new private SelectableActionType[] validActionTypes = 
		{SelectableActionType.Attack, 
		SelectableActionType.Build, 
		SelectableActionType.Move, 
		SelectableActionType.Enhance};

	public int movesLeft = 0;

	void Awake()
	{
		baseStatsSet = false;
	}

	void Start()
	{
		if(!baseStatsSet && !this.debug)
		{
			throw new System.Exception("Unit was instantiated, but no base stats were set. Make sure you are instantiating through UnitInfo");
		}
	}

    void Update()
    {
        if (moving)
        {
            HandleMovement();
        }
    }

	public void SetBaseStats(string name, int maxRange, int health, int armor, int attack) {
		gameObject.name = name;
		this.maxRange = maxRange;
		this.health = health;
		this.armor = armor;
		this.attack = attack;

		baseStatsSet = true;
	}

	/// Updates / Resets the turn fields for the Unit to get ready for the turn
	public void UpdatePreTurn()
	{
		this.movesLeft = this.maxRange;
		tapped = false;
        movesMade = 0;
	}

	/// Connected by a HeavyGameEventListener
	/// Initial check to see if the fired HeavyGameEventData has to do with this unit
	public virtual void OnFilterGameEvent(HeavyGameEventData data)
	{
		if(data.SourceNode == this.ParentNode || data.TargetNode == this.ParentNode)
		{
			this.processAction(data);
		}
	}

	public override bool CanPerformAction(SelectableActionType actionType, GridCell targetNode, string param)
	{
		if(this.GetValidActionTypes().Contains(actionType))
		{
			switch(actionType)
			{
				case SelectableActionType.Build:
					/// maybe it should check to see if the param string refers to a unit
					return targetNode.Selectable == null && param != "";
				case SelectableActionType.Attack:
					/// Attacks if the targetNode has a selectable and if the Faction belonging to the selectable is
					/// not the current faction
					return targetNode.Selectable is Unit && ((Unit)targetNode.Selectable).faction != this.faction;
				case SelectableActionType.Enhance:
					/// Enhances if the target node is the same node
					return targetNode?.Selectable == this;
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
			case SelectableActionType.Build:
				this.performBuild(targetNode, param);
			break;
			case SelectableActionType.Attack:
				this.performAttack(targetNode);
			break;
			case SelectableActionType.Enhance:
				this.performEnhance(param);
			break;
			case SelectableActionType.Move:
				this.PerformMove(targetNode);
			break;
		}
	}

	/// Raises a build action
	protected virtual void performBuild(GridCell targetNode, string unitName)
	{
		HeavyGameEventData data = new HeavyGameEventData();
		data.SourceNode = this.ParentNode;
		data.TargetNode = targetNode;
		data.StringValue = unitName;
		data.ActionType = SelectableActionType.Build;
		GameStateManager.Instance.PerformAction(data);
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
    protected virtual void PerformMove(GridCell targetIn)
    {
        HeavyGameEventData data = new HeavyGameEventData();
		data.SourceNode = this.ParentNode;
		data.TargetNode = targetIn;
        target = targetIn;
        currentPathIndex = 0;
        cellPath = GridManager.Instance.pathfinder.FindPath(parentCell.layer, parentCell.slice, targetIn.layer, targetIn.slice);
        targetPath = GridManager.Instance.pathfinder.FindVectorPath(cellPath);
        moving = true;
        GameStateManager.Instance.PerformAction(data);
    }

    public void SetParentCell(GridCell cellIn)
    {
        parentCell = cellIn;
		cellIn.Selectable = this;
    }

    public GridCell GetParentCell()
    {
        return parentCell;
    }

    public void SetMaxRange(int maxRangeIn)
    {
        maxRange = maxRangeIn;
    }

    public int GetMaxRange()
    {
        return maxRange;
    }
    
    private void HandleMovement()
    {
		if(!tapped){
			if (targetPath != null && currentPathIndex < targetPath.Count)
			{
				Vector3 targetPosition = targetPath[currentPathIndex];
				float step = 30 * Time.deltaTime;
				//TODO: Change from .Distance to sqr magnitudes to save on calculation
				if (Vector3.Distance(transform.position, targetPosition) > step)
				{
					Vector3 moveDir = (targetPosition - transform.position).normalized;

					//TODO: Change from .Distance to sqr magnitudes to save on calculation
					float distanceBefore = Vector3.Distance(transform.position, targetPosition);
					transform.position = transform.position + moveDir * 30.0f * Time.deltaTime;
				}
				else
				{
					cellPath[currentPathIndex].Selectable = null;
					currentPathIndex++;
					if(currentPathIndex > 1)
					{
						movesMade++;
						movesLeft--;
					}
					if (movesMade == maxRange)
					{
						tapped = true;
					}
					if (currentPathIndex == maxRange+1 && !(currentPathIndex >= targetPath.Count+1))
					{
						EndMoveInMiddle();
					}
					if (Vector3.Distance(transform.position, target.transform.position) <= 0.05f)
					{
						EndMove();
					}
				}
			}
		}
    }

	/// Raises an enhance action
	protected virtual void performEnhance(string param)
	{
		HeavyGameEventData data = new HeavyGameEventData();
		data.SourceNode = this.ParentNode;
		data.TargetNode = this.ParentNode;
		data.StringValue = param;
		GameStateManager.Instance.PerformAction(data);
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
	public virtual void processAction(HeavyGameEventData data)
	{
	}
    
    private void EndMove()
    {
        moving = false;
        SetParentCell(target);
        currentPathIndex = 1;
        transform.position = target.transform.position;
        target = null;
    }

    private void EndMoveInMiddle()
    {
        moving = false;
        transform.position = targetPath[currentPathIndex-1];
        SetParentCell(cellPath[currentPathIndex-1]);
        targetPath = targetPath.Skip(currentPathIndex-1).ToList();
        cellPath = cellPath.Skip(currentPathIndex-1).ToList();
    }

    public void PrepareForNextTurn()
    {
        tapped = false;
        movesMade = 0;
    }

    public void TakeOutstandingMoves()
    {
		if (!tapped)
		{
			if(target != null)
			{
				PerformMove(target);
			}
		}
    }
}
