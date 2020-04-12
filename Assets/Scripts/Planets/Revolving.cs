using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Revolving
{

    bool IsMoving();

    void Revolve();

    void SetRevSpeed(int speed);

    void SetParentCell(GridCell cellIn);

    GridCell GetParentCell();

    void SetRevolveDirection(RevolveDirection direction);

    void HandleMovement();

    void EndMove();
}
public enum RevolveDirection
{
    CounterClockwise,
    Clockwise
}