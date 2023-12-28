using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum LodLevel
{
    World0 = 0,
    World1,
    World2,
    World3,
}

public class World : MonoBehaviour
{
    public static World Instance;

    public GameObject CameraRoot;
    public GameObject CameraGo;
    public Camera Camera;

    private float zoomMax = 128;
    private float zoomMin = 16;
    private float zoomCurrent = 34f;
    private float zoomSpeed = 4;

    public float perspectiveZoomSpeed = 0.0001f; // The rate of change of the field of view in perspective mode.

    public int GridSizeX = 48;
    public int GridSizeY = 48;
    

    private Map _map;
    private Dictionary<LodLevel, float> _lodSetting;

    private List<BaseWorldUnit> _wdUnitList;


    public World()
    {
        //_lodSetting = new Dictionary<LodLevel, float>(5);
        //_lodSetting.Add(LodLevel.World1, 300);
        //_lodSetting.Add(LodLevel.World2, 400);
//
        //_wdUnitList = new List<BaseWorldUnit>();
        //WdArmy unit = new WdArmy();
        //unit.Init(1, 1, 1, 1, 1);
//
        //_wdUnitList.Add(unit);
    }

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        //zoom
        //if(!InFade){
        zoomCurrent -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        zoomCurrent = Mathf.Clamp(zoomCurrent, zoomMin, zoomMax);
        CameraGo.transform.localPosition = new Vector3(0, 0, -zoomCurrent);
        //}

        // pinch zoom for mobile touch input
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            zoomCurrent += deltaMagnitudeDiff * perspectiveZoomSpeed;
            zoomCurrent = Mathf.Clamp(zoomCurrent, zoomMin, zoomMax);
            CameraGo.transform.localPosition = new Vector3(0, 0, -zoomCurrent);
        }
        
        //TODO:判断当前Lod级别，并通知全部建筑
    }
}