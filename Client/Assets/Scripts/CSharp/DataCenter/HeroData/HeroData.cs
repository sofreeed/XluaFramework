using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInfoItemData
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public int Power { get; set; }

    public HeroInfoItemData(int id, string name, string desc, int power)
    {
        this.ID = id;
        this.Name = name;
        this.Desc = desc;
        this.Power = power;
    }
}

public class HeroData : BaseData
{
    private Dictionary<int, HeroInfoItemData> _heroList = new Dictionary<int, HeroInfoItemData>();

    public HeroData()
    {
        
    }

    public override void Dispose()
    {
        base.Dispose();     //清理事件注册
    }
}
