﻿using System;
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
			_data.SourceCell.Selectable = null;
			movingPlanet.SetParentCell(_data.TargetCell);
		}
		else{
			if(_data.TargetCell.Selectable.selectableType == SelectableType.Unit)
			{
				Planet movingPlanet = (Planet)_data.SourceCell.Selectable;
				Unit sittingUnit = (Unit)_data.TargetCell.Selectable;
				
				GridCell newParentCell = null;
				int numberOfTries = 0;
				while (newParentCell == null && numberOfTries < movingPlanet.grid.GetGridSize().slices - 1)
				{
					System.Random r = new System.Random();
					int randSlice = r.Next(0, movingPlanet.grid.GetGridSize().slices-1);
					GridCell potentialCell = movingPlanet.grid.GetGridCell(movingPlanet.grid.GetGridSize().layers-2, randSlice);
					if (potentialCell.Selectable == null)
					{
						newParentCell = potentialCell;
					}
					numberOfTries++;
					if (numberOfTries == movingPlanet.grid.GetGridSize().slices)
					{
						//TODO: Do something with this
						throw new System.Exception("No free cells for move");
					}
				}

				sittingUnit.SetParentCell(newParentCell);
				sittingUnit.transform.position = newParentCell.transform.position;

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
        _data.SourceCell.Selectable = null;
		GridCell finalTarget = _data.TargetCell;

		//If moving to edge of planetary system
		if(!_data.TargetCell.parentGrid.isSolarSystem && _data.TargetCell.layer == _data.TargetCell.parentGrid.GetGridSize().layers-1)
		{
			//Get current cardinal neighbor cells of planet in solar system
			(GridCell north, GridCell south, GridCell east, GridCell west) neighbors = 
				GetDirectionalGridCells(_data.TargetCell.parentGrid.parentPlanet.ParentCell.GetNeighbors(),
				_data.TargetCell.parentGrid.parentPlanet.ParentCell.transform.position);

			//Use cardinal direction conversion to set final target
			Vector3 diffFromSource = (movingUnit.transform.position - _data.SourceCell.parentGrid.transform.position);
			if (Mathf.Abs(diffFromSource.z) >= Mathf.Abs(diffFromSource.x))
			{
				if (diffFromSource.z >= 0) 
				{
					finalTarget = neighbors.north;
				}
				else
				{
					finalTarget = neighbors.south;
				}
			}
			else
			{
				if (diffFromSource.x >= 0) 
				{
					finalTarget = neighbors.east;
				}
				else
				{
					finalTarget = neighbors.west;
				}
			}
			if (finalTarget == null || (finalTarget.Selectable != null && finalTarget.Selectable.selectableType == SelectableType.Unit))
			{
				finalTarget = _data.TargetCell.parentGrid.parentPlanet.ParentCell.GetNeighbors()[1];
			}
		}
		//Else, if we're moving onto a planet cell
		//(this is an else if because the previous circumstance should never be able to happen at the same time as this)
		else if (_data.TargetCell.Selectable != null && _data.TargetCell.Selectable.selectableType == SelectableType.Planet)
		{
			//Get current cardinal neighbor cells of planet in solar system
			(GridCell north, GridCell south, GridCell east, GridCell west) landings = 
				GetLandingGridCells(((Planet)_data.TargetCell.Selectable).grid);

			//Find difference from sourceNode to targetnode
			Vector3 diffFromSource = (_data.SourceCell.transform.position - _data.TargetCell.transform.position);
			if (Mathf.Abs(diffFromSource.z) >= Mathf.Abs(diffFromSource.x))
			{
				if (diffFromSource.z >= 0) 
				{
					finalTarget = FindLandingCell(landings.north);
				}
				else
				{
					finalTarget = FindLandingCell(landings.south);
				}
			}
			else
			{
				if (diffFromSource.x >= 0) 
				{
					finalTarget = FindLandingCell(landings.east);
				}
				else
				{
					finalTarget = FindLandingCell(landings.west);
				}
			}
		}
		movingUnit.transform.position = finalTarget.transform.position;
		finalTarget.Selectable = movingUnit;
		movingUnit.SetParentCell(finalTarget);
	}

	private (GridCell north, GridCell south, GridCell east, GridCell west) GetDirectionalGridCells(List<GridCell> cells, Vector3 origin)
	{
		GridCell northCell = null;
		GridCell southCell = null;
		GridCell eastCell = null;
		GridCell westCell = null;

		foreach(GridCell cell in cells)
		{
			Vector3 diffFromOrigin = (cell.transform.position - origin);
			if (Mathf.Abs(diffFromOrigin.z) >= Mathf.Abs(diffFromOrigin.x))
			{
				if (diffFromOrigin.z >= 0) 
				{
					northCell = cell;
				}
				else
				{
					southCell = cell;
				}
			}
			else
			{
				if (diffFromOrigin.x >= 0) 
				{
					eastCell = cell;
				}
				else
				{
					westCell = cell;
				}
			}
		}

		return (northCell, southCell, eastCell, westCell);
	}

	private (GridCell north, GridCell south, GridCell east, GridCell west) GetLandingGridCells(CircularGrid grid)
	{
		(int tempLay, int tempSlice) = grid.GetGridSize();
		int slices = tempSlice - 1;
		int outerLayer = tempLay - 2;
		GridCell eastCell = grid.GetGridCell(outerLayer, 0);
		GridCell northCell = grid.GetGridCell(outerLayer, Mathf.CeilToInt(slices / 4) * 1);
		GridCell westCell = grid.GetGridCell(outerLayer, Mathf.CeilToInt(slices / 4) * 2);
		GridCell southCell = grid.GetGridCell(outerLayer, Mathf.CeilToInt(slices / 4) * 3);

		return (northCell, southCell, eastCell, westCell);
	}

	private GridCell FindLandingCell(GridCell startingCell)
	{
		int slicesTried = 1;
		int moveToLayer = startingCell.parentGrid.GetGridSize().layers - 2;
		GridCell landingCell = startingCell;
		CircularGrid grid = landingCell.parentGrid;
		while (landingCell.Selectable != null)
		{
			GridCell possiblePlus = grid.GetGridCell(moveToLayer, startingCell.slice + slicesTried);
			GridCell possibleMinus = grid.GetGridCell(moveToLayer, startingCell.slice - slicesTried);
			if (possiblePlus.Selectable == null)
			{
				landingCell = possiblePlus;
			}
			else
			{
				landingCell = possibleMinus;
			}
			slicesTried++;
			if (slicesTried >= (startingCell.parentGrid.GetGridSize().slices-1))
			{
				moveToLayer--;
				slicesTried = 0;
			}
			if (moveToLayer == -1)
			{
				throw new System.Exception ("Ran out of spaces - what do we do?");
			}
		}
		return landingCell;
	}

	public void OnUnitDestroyed(MonoBehaviour unitObject)
    {
        Unit unit = unitObject.GetComponent<Unit>();
        GridCell parentCell = unit.ParentCell;
		parentCell.Selectable = null;
    }
}
