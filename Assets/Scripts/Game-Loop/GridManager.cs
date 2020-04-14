using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    /// Sets up class as Singleton
	private static GridManager _instance;

	public static GridManager Instance { get { return _instance; } }

	/// Raises Exception if multiple singleton instances are present at once
	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			throw new System.Exception("Multiple GridManager instances are present. There should only be one instance of a Singleton.");
		} else {
			_instance = this;
		}
	}
    
    public CircularGrid grid;

	public Pathfinder pathfinder;

	[SerializeField] private HeavyGameEvent onForceSelectableMoveEvent;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnPlanetMoveEvent(HeavyGameEventData _data)
	{
		if (_data.TargetCell.Selectable == null)
		{
			Selectable movingPlanet = _data.SourceCell.Selectable;
			_data.SourceCell.passable = true;
			_data.SourceCell.Selectable = null;
			movingPlanet.SetParentCell(_data.TargetCell);
		}
		else{
			if(_data.TargetCell.Selectable.selectableType == SelectableType.Unit)
			{
				Planet movingPlanet = (Planet)_data.SourceCell.Selectable;
				Unit sittingUnit = (Unit)_data.TargetCell.Selectable;
				
				GridCell newParentCell = movingPlanet.grid.GetGridCell(0, 0);

				sittingUnit.SetParentCell(newParentCell);
				sittingUnit.transform.position = newParentCell.transform.position;

				_data.SourceCell.passable = true;
				_data.SourceCell.Selectable = null;
				movingPlanet.SetParentCell(_data.TargetCell);
			}
			else
			{
				throw new System.Exception("A planet has collided with something it shouldn't have.");
			}
		}
	}

	public void OnUnitMoveEvent(HeavyGameEventData _data)
	{
		Unit movingUnit = (Unit)_data.SourceCell.Selectable;
		_data.SourceCell.passable = true;
        _data.SourceCell.Selectable = null;
		GridCell finalTarget = _data.TargetCell;
		if(!_data.TargetCell.parentGrid.isSolarSystem && _data.TargetCell.layer == _data.TargetCell.parentGrid.GetGridSize().layers-1)
		{
			finalTarget = _data.TargetCell.parentGrid.parentPlanet.ParentCell.GetNeighbors()[1];
		}
		movingUnit.transform.position = finalTarget.transform.position;
		finalTarget.Selectable = movingUnit;
		finalTarget.passable = false;
		movingUnit.SetParentCell(finalTarget);
	}
}
