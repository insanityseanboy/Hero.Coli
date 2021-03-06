﻿// #define QUICKTEST

using UnityEngine;

public class CutSceneElements : MonoBehaviour
{

    private const float blackBarWait1 = 0.1f;
#if QUICKTEST
    private const float blackBarWait2 = 0.1f;
#else
    private const float blackBarWait2 = 1.5f;
#endif

    private CellControl __cellControl;
    protected CellControl _cellControl
    {
        get
        {
            __cellControl = (null == __cellControl) ? GameObject.FindGameObjectWithTag(Character.playerTag).GetComponent<CellControl>() : __cellControl;
            return __cellControl;
        }
    }

    protected static CutSceneBlackBarHandler _blackBar;
    protected static CullingMaskHandler _cullingMaskHandler;
    private int _originCullingMask;
    protected static Camera _cutSceneCameraUI;
    public const string doorTag = "Door";

    private T lazyInitObject<T>(T t, string gameObjectOrTagName, bool isTag = false)
    {
        if (null != t)
        {
            return t;
        }
        else
        {
            if (isTag)
            {
                //Debug.LogError(" GameObject.FindGameObjectWithTag("+gameObjectOrTagName+")="+GameObject.FindGameObjectWithTag(gameObjectOrTagName));
                return GameObject.FindGameObjectWithTag(gameObjectOrTagName).GetComponent<T>();
            }
            else
            {
                //Debug.LogError(" GameObject.Find("+gameObjectOrTagName+")="+GameObject.Find(gameObjectOrTagName));
                return GameObject.Find(gameObjectOrTagName).GetComponent<T>();
            }
        }
    }

    void OnEnable()
    {
        // initialization of static elements
        _blackBar = lazyInitObject<CutSceneBlackBarHandler>(_blackBar, "CutSceneBlackBars");
        _cullingMaskHandler = lazyInitObject<CullingMaskHandler>(_cullingMaskHandler, "InterfaceCamera", true);
        _cutSceneCameraUI = lazyInitObject<Camera>(_cutSceneCameraUI, "CutSceneCamera");
    }

    public static void reset()
    {
        // Debug.Log("CutSceneElements reset");
        _blackBar = null;
        _cullingMaskHandler = null;
        _cutSceneCameraUI = null;
    }
}
