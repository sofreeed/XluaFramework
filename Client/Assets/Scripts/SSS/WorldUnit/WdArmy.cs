using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WdArmy : WdBaseCluster
{
    private WdArmyType _wdArmyType;

    public override void Init(float x, float y, float zoomBeginCameraHeight, float zoomEndCameraHeight, float zoomSize)
    {
        base.Init(x, y, zoomBeginCameraHeight, zoomEndCameraHeight, zoomSize);
    }
}