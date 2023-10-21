using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWorldUnit : MonoBehaviour
{
    private float _x;
    private float _y;

    void Start()
    {
    }
    
    public virtual void Init(float x, float y)
    {
        this._x = x;
        this._y = y;
    }

    void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
    }

    public virtual void OnLodLevelChange(LodLevel lodLevel)
    {
    }
}