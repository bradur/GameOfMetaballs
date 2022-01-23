using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLifeCell
{
    public bool IsAlive { get; private set; }
    public bool PreviousState { get; set; }
    public bool NextState { get; set; }
    public Vector3Int Position { get; private set; }

    public GameOfLifeCell(Vector3Int pos)
    {
        Position = pos;
        PreviousState = false;
        NextState = false;
        SetIsAlive(false);
    }

    public void SetIsAlive(bool alive)
    {
        if (alive != IsAlive) {
            PreviousState = IsAlive;
            IsAlive = alive;
        }
    }
}
