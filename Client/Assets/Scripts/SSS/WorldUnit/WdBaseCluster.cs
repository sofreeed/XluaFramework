using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LodPrefab
{
    public LodLevel LodLevel;
    public GameObject Prefab;
}

public abstract class WdBaseCluster : BaseWorldUnit
{
    private List<BaseTownUnit> _townUnits;

    private bool _isWorld0 = false;
    private float _zoomBeginCameraHeight = 0;
    private float _zoomEndCameraHeight = 0;
    private float _zoomSize = 0;

    private Dictionary<LodLevel, LodPrefab> _lodPrefabs;

    public virtual void Init(float x, float y, float zoomBeginCameraHeight, float zoomEndCameraHeight, float zoomSize)
    {
        base.Init(x, y);

        this._zoomBeginCameraHeight = zoomBeginCameraHeight;
        this._zoomEndCameraHeight = zoomEndCameraHeight;
        this._zoomSize = zoomSize;
        _isWorld0 = false;
    }

    protected override void OnUpdate()
    {
        if (!_isWorld0)
            return;

        //无极缩放
        foreach (BaseTownUnit tnUnit in _townUnits)
        {
            float currZoomHeight = _zoomBeginCameraHeight - World.Instance.Camera.transform.position.y;
            float totalZoomHeight = _zoomBeginCameraHeight - _zoomEndCameraHeight;
            float t = currZoomHeight / totalZoomHeight;
            float currZoom = Mathf.Lerp(1, _zoomSize, t);
            tnUnit.transform.localScale = new Vector3(currZoom, currZoom, currZoom);
            
            //TODO：缩放完成后，计算碰撞情况，并显示和隐藏
        }
        
        
    }


    public override void OnLodLevelChange(LodLevel lodLevel)
    {
        if (lodLevel == LodLevel.World0)
        {
            _isWorld0 = true;
        }
        else
        {
            _isWorld0 = false;
            //TODO:通过_lodPrefabs设置相关显示
        }
    }
}