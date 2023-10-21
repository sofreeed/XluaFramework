using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTownUnit : MonoBehaviour
{
    public int TileX;
    public int TileY;

    public int Wdith;
    public int Height;

    public int PosX;
    public int PosY;

    private int _colliderLevel = -1;
    private bool _display = true;
    
    void Start()
    {
    }
    
    public virtual void Init(int x, int y, int wdith, int height)
    {
        TileX = x;
        TileY = y;
        Wdith = wdith;
        Height = height;
    }

    //计算obj坐标
    public void SetPosition()
    {
        transform.localPosition = Vector3.one;
    }

    public void GetRectSize()
    {
        
    }

    void Update()
    {
    }
}