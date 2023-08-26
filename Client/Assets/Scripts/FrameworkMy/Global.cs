using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{

    public static TypeEventSystem EventSystem;
    public static BundleUpdater BundleUpdater;
    public static ResManager ResManager;

    void Start()
    {
        EventSystem = TypeEventSystem.Global;
        
        BundleUpdater = GetComponent<BundleUpdater>();
        BundleUpdater.Init();
        
        //BundleUpdater.HotUpdate(GameStart);
        
        ResManager = GetComponent<ResManager>();
        ResManager.Init(BundleUpdater.ResourcePackage);
    }

    public void GameStart()
    {
        
    }
}
