using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{
    public GameObject UnitRoot;
    public GameObject Prefab;
    
    private Map _map;

    public BaseTownUnit[,] Cells;

    private Dictionary<TnUnitType, List<BaseTownUnit>> _tnUnitList;

    void Start()
    {
        
    }
}