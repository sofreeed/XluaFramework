using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public Vector2 UnitSize = Vector2.one;
    public Vector2 GridSize;
    public int SubGridSize = 2;

    private AStar _aStar;
}