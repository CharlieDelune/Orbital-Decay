using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IFactionAI : MonoBehaviour
{
    [SerializeField] protected MonoBehaviourGameEvent onCellSelectedEvent;
    [SerializeField] protected IntGameEvent onActionSelectedEvent;
    [SerializeField] protected MonoBehaviourGameEvent onTargetSelectedEvent;

    public void UseUnitTurn(Unit unit)
    {
        switch( unit.unitType )
        {
            case UnitType.Base:
                UseBaseTurn(unit);
                break;
            case UnitType.Builder:
                UseBuilderTurn((Builder)unit);
                break;
            case UnitType.Defender:
                UseDefenderTurn(unit);
                break;
            case UnitType.Attacker:
                UseAttackerTurn(unit);
                break;
            default:
                break;
        }
    }
    public abstract void UseBaseTurn(Unit unit);
    public abstract void UseBuilderTurn(Builder unit);
    public abstract void UseAttackerTurn(Unit unit);
    public abstract void UseDefenderTurn(Unit unit);
}
