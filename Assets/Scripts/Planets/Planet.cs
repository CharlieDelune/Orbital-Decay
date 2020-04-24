using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : Selectable, Revolving
{
    public GameObject gravityWell;
    public CircularGrid grid;
    public BoolProperty isGridInView;
    RevolveDirection revolveDirection;
    GridCell targetCell;
    List<Vector3> targetPath;

    [SerializeField] MonoBehaviourGameEvent onTryChangeGridEvent;

    public int revolveSpeed;
    private bool moving;
    private int currentPathIndex;
    private bool viewing;

    private Action onFinishRevolve;

    void Start()
    {
    }

    void Update()
    {
        if (IsMoving())
        {
            HandleMovement();
        }
    }

    public override bool CanPerformAction(SelectableActionType actionType, GridCell targetCell, string param)
	{
		if(this.GetValidActionTypes().Contains(actionType))
		{
			switch(actionType)
			{
				case SelectableActionType.ChangeGrid:
					/// Changes to view if same cell
					return true;
			}
		}
		return false;
	}

    /// Returns list of valid actions
	public override List<SelectableActionType> GetValidActionTypes()
	{
		List<SelectableActionType> actionTypes = new List<SelectableActionType>(this.validActionTypes);
		return actionTypes;
	}

    public override void PerformAction(SelectableActionType actionType, GridCell targetCell, string param)
	{
		switch(actionType)
		{
			case SelectableActionType.ChangeGrid:
				this.ChangeGrid();
			    break;
		}
	}

    /// Raises a move action
	/// Note:
	/// Should change MovesLeft variable to properly reflect
	/// the move instead of just decrementing the value
    protected virtual void ChangeGrid()
    {
        onTryChangeGridEvent.Raise(this.grid);
    }

    public bool IsMoving()
    {
        return moving;
    }

    public void Revolve(Action callback)
    {
        this.onFinishRevolve = callback;
        this.Revolve();
    }

    public void Revolve()
    {
        targetCell = GridManager.Instance.GetGridCellForRevolve(ParentCell, revolveDirection, revolveSpeed);
        targetPath = GridManager.Instance.GetGridVectorsForRevolve(ParentCell, targetCell, revolveDirection);
        currentPathIndex = 0;
        moving = true;
    }

    public void SetRevSpeed(int speed)
    {
        revolveSpeed = speed;
    }

    public override void SetParentCell(GridCell cellIn)
    {
        ParentCell = cellIn;
        cellIn.Selectable = this;
    }

    public GridCell GetParentCell()
    {
        return ParentCell;
    }

    public void SetRevolveDirection(RevolveDirection direction)
    {
        revolveDirection = direction;
    }

    public void HandleMovement()
    {
        if (targetPath != null)
        {
            Vector3 targetPosition = targetPath[currentPathIndex];
            float step = 30 * Time.deltaTime;
            if (Vector3.Distance(transform.position, targetPosition) > step)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * 30.0f * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= targetPath.Count)
                {
                    EndMove();
                }
            }
        }
    }

    public void EndMove()
    {
        HeavyGameEventData data = new HeavyGameEventData();
        data.SourceCell = ParentCell;
        data.TargetCell = targetCell;
        onMoveEvent.Raise(data);
        moving = false;
        targetCell = null;
        this.onFinishRevolve();
    }
}
