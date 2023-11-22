using System.Collections;
using System.Collections.Generic;
using System.IO;
using Luban;
using UnityEngine;
using SimpleJSON;

public class DataTableMgr : Singleton<DataTableMgr>
{
    public static cfg.Tables Tables;

    public override void Init()
    {
        var tables = new cfg.Tables(LoadJson);
        Tables = tables;
        
        //var item = tables.TbItem.DataList[1];
        //UnityEngine.Debug.LogFormat("item[1]:{0}", item);
    }

    private static ByteBuf LoadByte(string file)
    {
        return new ByteBuf(File.ReadAllBytes($"{Application.dataPath}/../../GenerateDatas/bytes/{file}.bytes"));
    }

    private static JSONNode LoadJson(string file)
    {
        return JSON.Parse(File.ReadAllText(Application.dataPath + "/../../GenerateDatas/json/" + file + ".json",
            System.Text.Encoding.UTF8));
    }

    public override void Dispose()
    {
    }
}