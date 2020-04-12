using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : Selectable, Revolving
{
    public GameObject gravityWell;
    public CircularGrid grid;
    public BoolProperty isGridInView;
    RevolveDirection revolveDirection;
    GridCell parentCell;
    GridCell targetCell;
    List<Vector3> targetPath;

    [SerializeField] MonoBehaviourGameEvent onTryChangeGridEvent;

    public int revolveSpeed;
    private bool moving;
    private int currentPathIndex;
    private bool viewing;
    //[SerializeField]
    //private PlayerViewState viewState;
    new private SelectableActionType[] validActionTypes = 
    {SelectableActionType.ChangeGrid};

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

    void OnMouseDown()
    {
        // if (!viewing)
        // {
        //     Camera.main.transform.position = gravityWell.transform.position + new Vector3(0, 60, -45);
        //     viewing = true;
        // }
        // else
        // {
        //     //Move camera back
        //     Camera.main.transform.position = new Vector3(0, 60, -45);
        //     viewing = false;
        // }
    }

    public override bool CanPerformAction(SelectableActionType actionType, GridCell targetNode, string param)
	{
		if(this.GetValidActionTypes().Contains(actionType))
		{
			switch(actionType)
			{
				case SelectableActionType.ChangeGrid:
					/// Changes to view if same node
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

    public override void PerformAction(SelectableActionType actionType, GridCell targetNode, string param)
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

    public void Revolve()
    {
        targetCell = GridManager.Instance.grid.GetGridCellForRevolve(parentCell, revolveDirection, revolveSpeed);
        targetPath = GridManager.Instance.grid.GetGridVectorsForRevolve(parentCell.layer, parentCell.slice, targetCell.slice, revolveDirection);
        currentPathIndex = 0;
        moving = true;
    }

    public void SetRevSpeed(int speed)
    {
        revolveSpeed = speed;
    }

    public void SetParentCell(GridCell cellIn)
    {
        cellIn.passable = false;
        parentCell = cellIn;
    }

    public GridCell GetParentCell()
    {
        return parentCell;
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
        parentCell.passable = true;
        moving = false;
        SetParentCell(targetCell);
        transform.position = targetCell.transform.position;
        targetCell = null;
    }
}
