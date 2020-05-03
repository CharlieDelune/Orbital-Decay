using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

    /**
    * Improvements that would be made if we had more time:
    *   Bases should make sure not to get themselves stuck between resource nodes
    *   Defenders should get the heck outta the way if the bases need to build more Builders
    *   Attackers should work together, fly in packs, attack weak factions rather than just doing it all random
    *   
    * Most of this would require some kind of scoring system, which is something we originally wanted to
    * implement anyway, but ran out of time. Being able to score the progress of factions in a bunch of different
    * ways would go really far in making this AI make smarter and better decisions.
    *
    * Best Wishes,
    * Jeff Kapochus
    * 5/2/2020
    **/

public class BasicAssFactionAI : IFactionAI
{
    // Track where units arriving to the solar system are coming from
    private Dictionary<Unit, Planet> originPlanets = new Dictionary<Unit, Planet>();
    private Dictionary<Unit, bool> waitingForPlanet = new Dictionary<Unit, bool>();

    public override void UseBaseTurn(Unit unit){
        //If not currently moving anywhere...
        if (unit.target == null)
        {
            //See if we can build any mines before we do anything else!
            foreach (GridCell neighbor in unit.ParentCell.GetNeighbors())
            {
                if (neighbor.ResourceDeposit != null && neighbor.Selectable == null && unit.CanPerformAction(SelectableActionType.Build, neighbor, "0"))
                {
                    unit.PerformAction(SelectableActionType.Build, neighbor, "0");
                }
            }

            //After we build mines, find the closest resource deposit in the current grid
            ResourceDeposit closestDeposit = GetNearestResourceDeposit(unit);
            //If we found a close resource deposit in the current grid that doesn't currently contain a mine
            if (closestDeposit != null && unit.CanPerformAction(SelectableActionType.Move, closestDeposit.ParentCell, null))
            {
                //Move to it
                unit.PerformAction(SelectableActionType.Move, closestDeposit.ParentCell, null);
            }

            //If we've hit this point, we've already put mines on every resource deposit in the grid
            else
            {
                //Builds a Builder roughly every 20 turns because that seems right
                if (unit.Faction.units.Where(u => u.unitType == UnitType.Builder).ToList().Count < (GameStateManager.Instance.TotalRounds / 20) + 1)
                {
                    foreach (GridCell neighbor in unit.ParentCell.GetNeighbors())
                    {
                        if (neighbor.Selectable == null && unit.CanPerformAction(SelectableActionType.Build, neighbor, "1"))
                        {
                            unit.PerformAction(SelectableActionType.Build, neighbor, "1");
                            break;
                        }
                    }
                }
                //We want to find a square that has no resource deposits around it so we can surround the base with defenders
                //If any of the cells around the base has either a resource deposit 
                //or a unit that belongs to another faction 
                //or any friendly unit that isn't a Defender
                if (unit.ParentCell.GetNeighbors().Where(cell => cell.ResourceDeposit != null || (cell.Selectable != null &&
                    (((Unit)cell.Selectable).Faction != unit.Faction || ((Unit)cell.Selectable).unitType != UnitType.Defender))).ToList().Any())
                {
                    GridCell emptyCell = GetNearestEmptyCell(unit);
                    if(unit.CanPerformAction(SelectableActionType.Move, emptyCell, null))
                    {
                        unit.PerformAction(SelectableActionType.Move, emptyCell, null);
                    }
                }
            }
        }
    }

    public override void UseBuilderTurn(Builder unit){
        //Four defenders at all times to surround the base
        if (unit.ParentCell.parentGrid == MainBase(unit).ParentCell.parentGrid)
        {
            if (unit.Faction.units.Where(u => u.unitType == UnitType.Defender).ToList().Count < 4)
            {
                GridCell openCell = GetOpenNeighbor(unit.ParentCell);
                if (unit.CanPerformAction(SelectableActionType.Build, openCell, "2"))
                {
                    unit.PerformAction(SelectableActionType.Build, openCell, "2");
                }
            }
            else
            {
                //TODO
            }
        }
        else 
        {
            //TODO
        }
        //Builds a Attacker roughly every 10 turns after the 15th turn because that seems right
        if (GameStateManager.Instance.TotalRounds > 14 && 
            unit.Faction.units.Where(u => u.unitType == UnitType.Attacker).ToList().Count < (GameStateManager.Instance.TotalRounds / 10))
        {
            GridCell nearestOpen = GetOpenNeighbor(unit.ParentCell);
            if (unit.CanPerformAction(SelectableActionType.Build, nearestOpen, "1"))
            {
                unit.PerformAction(SelectableActionType.Build, nearestOpen, "1");
            }
        }
        //We want to try our best to move the builder unit to empty spaces so it has room to build
        //It also gives the illusion of AI that's more dynamic than it actually is
        if(!CellIsInEmptySpace(unit, unit.ParentCell))
        {
            GridCell nearestEmpty = GetNearestEmptyCell(unit);
            if(unit.CanPerformAction(SelectableActionType.Move, nearestEmpty, null))
            {
                unit.PerformAction(SelectableActionType.Move, nearestEmpty, null);
            }
        }
    }

    public override void UseAttackerTurn(Unit unit)
    {
        //If unit is not in solar system, set its origin planet to current grid's planet
        //we'll check this value once we arrive into the solar system
        if(!unit.ParentCell.parentGrid.isSolarSystem)
        {
            if (originPlanets.Keys.Contains(unit))
            {
                originPlanets[unit] = unit.ParentCell.parentGrid.parentPlanet;
            }
            else
            {
                originPlanets.Add(unit, unit.ParentCell.parentGrid.parentPlanet);
            }
            if (waitingForPlanet.Keys.Contains(unit))
            {
                waitingForPlanet[unit] = false;
            }
            else
            {
                waitingForPlanet.Add(unit, false);
            }
        }

        //Is there an enemy in an adjoining cell
        GridCell adjoiningCellContainingEnemy = GetAdjoiningCellContainingEnemy(unit);
        if (adjoiningCellContainingEnemy != null)
        {
            if (unit.CanPerformAction(SelectableActionType.Attack, adjoiningCellContainingEnemy,null))
            {
                unit.PerformAction(SelectableActionType.Attack, adjoiningCellContainingEnemy, null);
            }
        }

        //Is there still an enemy nearby
        if (adjoiningCellContainingEnemy != null && adjoiningCellContainingEnemy.Selectable == null)
        {
            adjoiningCellContainingEnemy = GetAdjoiningCellContainingEnemy(unit);
        }

        if(adjoiningCellContainingEnemy == null)
        {
            //Are there any enemy units in the current grid
            List<GridCell> enemyFactionCells = GetEnemiesInGrid(unit);
            if (enemyFactionCells.Any())
            {
                //if so move to them
                GridCell closestCellContainingEnemy = GetNearestGridCell(enemyFactionCells, unit);
                if (unit.CanPerformAction(SelectableActionType.Move, closestCellContainingEnemy, null))
                {
                    unit.PerformAction(SelectableActionType.Move, closestCellContainingEnemy, null);
                }
            }
            else
            {
                //if not, travel to solar system
                if (!unit.ParentCell.parentGrid.isSolarSystem)
                {
                    GridCell closestEdgeCell = GetNearestGridCell(unit.ParentCell.parentGrid.GetGridCells().Where(cell => cell.isEdgeCell == true).ToList(), unit);
                    if (unit.CanPerformAction(SelectableActionType.Move, closestEdgeCell, null))
                    {
                        unit.PerformAction(SelectableActionType.Move, closestEdgeCell, null);
                    }
                }
                else
                {
                    //If we're not waiting for a planet to orbit 'round to us
                    if (!waitingForPlanet[unit])
                    {
                        //Go to where the planet will be in a few turns to try and beat it
                        GridCell randomPlanetCell = GetPlanetOrbitPath(GetRandomPlanet(unit), 3);
                        if (unit.CanPerformAction(SelectableActionType.Move, randomPlanetCell, null))
                        {
                            //If we didn't beat it, wait around for the planet to come back
                            unit.PerformAction(SelectableActionType.Move, randomPlanetCell, null);
                            waitingForPlanet[unit] = true;
                        }
                    }
                }
            }
        }

        //After all their moves are done, let's see if they can attack
        //Should only happen if they haven't already attacked
        adjoiningCellContainingEnemy = GetAdjoiningCellContainingEnemy(unit);
        if (adjoiningCellContainingEnemy != null)
        {
            if(unit.CanPerformAction(SelectableActionType.Attack, adjoiningCellContainingEnemy, null))
            {
                unit.PerformAction(SelectableActionType.Attack, adjoiningCellContainingEnemy, null);
            }
        }
    }
    public override void UseDefenderTurn(Unit unit)
    {
        //If the defender isn't on a cell surrounding the main base,
        //See if we can path to one and then freakin do it
        //that base ain't like to defend itself
        if(!unit.ParentCell.GetNeighbors()
            .Where(cell => cell.Selectable != null && cell.Selectable is Unit && (Unit)cell.Selectable == MainBase(unit)).ToList().Any())
        {
            GridCell openDefenseCell = null;
            if (unit.ParentCell.parentGrid == MainBase(unit).ParentCell.parentGrid)
            {
                openDefenseCell = GetOpenNeighbor(MainBase(unit).ParentCell);
            }
            if (unit.CanPerformAction(SelectableActionType.Move, openDefenseCell, null))
            {
                unit.PerformAction(SelectableActionType.Move, openDefenseCell, null);
            }
        }
    }

    private ResourceDeposit GetNearestResourceDeposit(Unit unit)
    {
        ResourceDeposit closestDeposit = null;
        float closestDist = float.MaxValue;
        foreach (ResourceDeposit dep in GridManager.Instance.resourceDeposits.Where(dep => dep.ParentCell.parentGrid == unit.ParentCell.parentGrid))
        {
            if (dep.ParentCell.Selectable == null)
            {
                float distToDeposit = (dep.transform.position - unit.transform.position).sqrMagnitude;
                if (distToDeposit < closestDist)
                {
                    closestDeposit = dep;
                    closestDist = distToDeposit;
                }

            }
        }

        return closestDeposit;
    }

    private GridCell GetNearestEmptyCell(Unit unit)
    {
        GridCell emptyCell = null;
        float closestDist = float.MaxValue;
        foreach (GridCell neighbor in unit.ParentCell.parentGrid.GetGridCells().Where(cell => cell.Selectable == null && cell.ResourceDeposit == null))
        {
            if (CellIsInEmptySpace(unit, neighbor) && !neighbor.isEdgeCell)
            {
                float distToCell = (neighbor.transform.position - unit.transform.position).sqrMagnitude;
                if (distToCell < closestDist)
                {
                    emptyCell = neighbor;
                    closestDist = distToCell;
                }
            }
        }

        return emptyCell;
    }

    private bool CellIsInEmptySpace(Unit unit, GridCell neighbor)
    {
        return !(neighbor.GetNeighbors().Where(cell => !cell.isEdgeCell && ((cell.Selectable != null && cell.Selectable != unit) || cell.ResourceDeposit != null)).ToList().Any());
    }

    private GridCell GetOpenNeighbor(GridCell parentCell)
    {
        GridCell openCell = null;
        foreach(GridCell cell in parentCell.GetNeighbors())
        {
            if(cell.Selectable == null && cell.ResourceDeposit == null && !cell.isEdgeCell)
            {
                openCell = cell;
                break;
            }
        }
        return openCell;
    }

    private bool HasAtLeastOneOpenNeighbor(List<GridCell> cells)
    {
        foreach(GridCell cell in cells)
        {
            if (cell.Selectable == null && cell.ResourceDeposit == null && !cell.isEdgeCell)
            {
                return true;
            }
        }

        return false;
    }

    private GridCell GetAdjoiningCellContainingEnemy(Unit unit)
    {
        GridCell cell = null;
        foreach(GridCell neighbor in unit.ParentCell.GetNeighbors())
        {
            if (neighbor.Selectable != null && neighbor.Selectable is Unit && ((Unit)neighbor.Selectable).Faction != unit.Faction)
            {
                cell = neighbor;
            }
        }

        return cell;
    }

    private List<GridCell> GetEnemiesInGrid(Unit unit)
    {
        return unit.ParentCell.parentGrid.GetGridCells()
            .Where(cell => cell.Selectable != null && cell.Selectable is Unit && ((Unit)cell.Selectable).Faction != unit.Faction).ToList();
    }

    private GridCell GetNearestGridCell(List<GridCell> potentials, Unit unit)
    {
        GridCell closestCell = null;
        float shortestDistance = float.MaxValue;
        foreach (GridCell cell in potentials)
        {
            float distanceToPotential = (unit.transform.position - cell.transform.position).sqrMagnitude;
            if (distanceToPotential < shortestDistance)
            {
                shortestDistance = distanceToPotential;
                closestCell = cell;
            }
        }

        return closestCell;
    }

    private Planet GetRandomPlanet(Unit unit)
    {
        Planet targetPlanet = null;
        do
        {
            targetPlanet = PlanetManager.Instance.planets[Random.Range(0, PlanetManager.Instance.planets.Count - 1)];
        }
        while (originPlanets[unit] == targetPlanet);
        return targetPlanet;
    }

    private GridCell GetPlanetOrbitPath(Planet planet, int numOfTurns)
    {
        return GameStateManager.Instance.solarSystemGrid.GetGridCellForRevolve(planet, numOfTurns);
    }

    private GridCell GetNearestPlanetNotOrigin(Unit unit)
    {
        GridCell closestCell = null;
        if (unit.ParentCell.parentGrid.isSolarSystem)
        {
            float shortestDistance = float.MaxValue;
            foreach (Planet planet in PlanetManager.Instance.planets)
            {
                if (planet != originPlanets[unit])
                {
                    float distanceToPotential = (unit.transform.position - planet.transform.position).sqrMagnitude;
                    if (distanceToPotential < shortestDistance)
                    {
                        shortestDistance = distanceToPotential;
                        closestCell = planet.ParentCell;
                    }
                }
            }
        }

        return closestCell;
    }

    private Unit MainBase(Unit unit)
    {
        return unit.Faction.units.Where(u => u.unitType == UnitType.Base).Single();
    }
}
