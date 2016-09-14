﻿using UnityEngine;

public class FocusMaskManager : MonoBehaviour
{

    public GameObject focusMask, hole;
    public UISprite focusMaskSprite;
    public GameObject clickBlocker;
    private ExternalOnPressButton _target;
    private CellControl _cellControl;
    private bool _isBlinking = false;
    private bool _isAlphaIncreasing = false;
    private const float _blinkingSpeed = 0.5f;
    private const float _minAlpha = 0.7f;
    private const float _maxAlpha = 1f;
    private float _newAlpha;
    private bool _isClicksBlocked = false;
    private Vector3 _baseFocusMaskScale, _baseHoleScale;
    [SerializeField]
    private Advisor _advisor;

    // test code
    /*
    public ExternalOnPressButton testClickable;
    public GameObject testObject;
    public Vector3 testPosition;
    //*/

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    protected const string gameObjectName = "FocusMaskSystem";
    protected static FocusMaskManager _instance;
    public static FocusMaskManager get()
    {
        //Debug.Log("FocusMaskManager get");
        if (_instance == null)
        {
            Logger.Log("FocusMaskManager::get was badly initialized", Logger.Level.WARN);
            _instance = GameObject.Find(gameObjectName).GetComponent<FocusMaskManager>();
        }
        return _instance;
    }

    void Awake()
    {
        Logger.Log("FocusMaskManager::Awake", Logger.Level.DEBUG);
        //Debug.Log("FocusMaskManager Awake");
        _instance = this;
        _baseFocusMaskScale = focusMask.transform.localScale;
        _baseHoleScale = hole.transform.localScale;

        reinitialize();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    public delegate void Callback();
    private Callback _callback;

    public void focusOn(ExternalOnPressButton target, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        focusOn(target, Vector3.zero, callback, advisorTextKey, scaleToComponent);
    }

    public void focusOn(ExternalOnPressButton target, Vector3 manualScale, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        if (null != target)
        {
            //Debug.Log("FocusMaskManager focusOn "+target.name);
            float scaleFactor = computeScaleFactor(scaleToComponent, target.transform.localScale, manualScale);

            focusOn(target.transform.position, callback, scaleFactor, false, advisorTextKey);
            _target = target;
        }
        else
        {
            //Debug.Log("FocusMaskManager focusOn null");
        }
    }

    public void focusOn(GameObject go, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        focusOn(go, Vector3.zero, callback, advisorTextKey, scaleToComponent);
    }

    public void focusOn(GameObject go, Vector3 manualScale, Callback callback = null, string advisorTextKey = null, bool scaleToComponent = false)
    {
        float scaleFactor = computeScaleFactor(scaleToComponent, go.transform.localScale, manualScale);

        Vector3 position = go.transform.position;

        bool isInterfaceObject = (this.gameObject.layer == go.layer);
        Debug.Log("isInterfaceObject=" + isInterfaceObject + " because layer=" + go.layer);
        if (!isInterfaceObject)
        {
            Camera camera = GameObject.Find("Player").GetComponentInChildren<Camera>();
            position = camera.WorldToScreenPoint(go.transform.position);
            position -= focusMask.transform.localScale / 4;
        }

        focusOn(position, callback, scaleFactor, !isInterfaceObject, advisorTextKey, true);
    }

    float computeScaleFactor(bool scaleToComponent, Vector3 localScale, Vector3 manualScale)
    {
        Vector3 scale = Vector3.zero;
        float result = 1f;
        
        if (Vector3.zero != manualScale)
        {
            scale = manualScale;
        }
        else if (scaleToComponent)
        {
            scale = localScale;
        }   

        if(Vector3.zero != scale)
        {
            float max = scale.x > scale.y ? scale.x : scale.y;
            result = max / _baseHoleScale.x;
        }

        return result;
    }

    public void focusOn(Vector3 position, Callback callback = null, float scaleFactor = 1f, bool local = true, string advisorTextKey = null, bool showComplementaryHint = false)
    {
        //Debug.Log("focusOn("+position+")");
        if (null != position)
        {
            _target = null;

            //Debug.Log("old pos="+this.transform.position);

            Vector3 newPosition;
            if (local)
            {
                newPosition = new Vector3(position.x, position.y, this.transform.localPosition.z);
                this.transform.localPosition = newPosition;
            }
            else
            {
                newPosition = new Vector3(position.x, position.y, this.transform.position.z);
                this.transform.position = newPosition;
            }
            //Debug.Log("new glopos="+this.transform.position);
            //Debug.Log("new pos="+newPosition);

            if (1f != scaleFactor)
            {
                //Debug.Log("will scale focusMask="+focusMask.transform.localScale+" and hole="+hole.transform.localScale+" with factor="+scaleFactor);
                focusMask.transform.localScale = scaleFactor * focusMask.transform.localScale;
                hole.transform.localScale = scaleFactor * hole.transform.localScale;
                //Debug.Log("now focusMask="+focusMask.transform.localScale+" and hole="+hole.transform.localScale);
            }
            else
            {
                //Debug.Log("will scale back focusMask="+focusMask.transform.localScale+" and hole="+hole.transform.localScale);
                focusMask.transform.localScale = _baseFocusMaskScale;
                hole.transform.localScale = _baseHoleScale;
                //Debug.Log("now focusMask="+focusMask.transform.localScale+" and hole="+hole.transform.localScale);                
            }

            _callback = callback;

            if (!string.IsNullOrEmpty(advisorTextKey))
            {
                if (this.transform.localPosition.y >= 0)
                {
                    _advisor.setUpNanoBot(false, advisorTextKey, showComplementaryHint);
                }
                else
                {
                    _advisor.setUpNanoBot(true, advisorTextKey, showComplementaryHint);
                }
                _advisor.gameObject.SetActive(true);
            }
            else
            {
                _advisor.gameObject.SetActive(false);
            }

            show(true);
        }

    }

    public void blockClicks(bool block)
    {
        _isClicksBlocked = block;
        clickBlocker.SetActive(block);
    }

    private void show(bool show)
    {
        _isBlinking = show;
        focusMask.SetActive(show);
        hole.SetActive(show);
        _advisor.gameObject.SetActive(show);
        GameObject perso = GameObject.Find("Perso");
        if (null != perso)
        {
            _cellControl = (_cellControl == null) ? perso.GetComponent<CellControl>() : _cellControl;
            _cellControl.freezePlayer(show);
        }
    }

    public void reinitialize()
    {
        //Debug.Log("FocusMaskManager reinitialize");
        this.gameObject.SetActive(true);
        show(false);
        clickBlocker.SetActive(false);

        _isBlinking = false;
        _isAlphaIncreasing = false;
        focusMaskSprite.alpha = 1;
    }

    public void click()
    {
        if (!_isClicksBlocked)
        {
            if (_target)
            {
                _target.OnPress(true);
            }
            if (null != _callback)
            {
                _callback();
            }
            reinitialize();
        }
    }

    void Update()
    {
        if (_isBlinking)
        {
            if (_isAlphaIncreasing)
            {
                _newAlpha = focusMaskSprite.alpha + _blinkingSpeed * Time.unscaledDeltaTime;
                if (_newAlpha > _maxAlpha)
                {
                    _newAlpha = _maxAlpha;
                    _isAlphaIncreasing = false;
                }
            }
            else
            {
                _newAlpha = focusMaskSprite.alpha - _blinkingSpeed * Time.unscaledDeltaTime;
                if (_newAlpha < _minAlpha)
                {
                    _newAlpha = _minAlpha;
                    _isAlphaIncreasing = true;
                }
            }
            focusMaskSprite.alpha = _newAlpha;
        }
    }

    //test code
    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            testClickable = GameObject.Find("CraftButton").GetComponent<ExternalOnPressButton>();
            focusOn(testClickable);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            testClickable = GameObject.Find("AvailableDisplayedDTER").GetComponent<ExternalOnPressButton>();
            focusOn(testClickable);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            focusOn(testPosition);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            focusOn(GameObject.Find("CraftButton"), true);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            focusOn(GameObject.Find("Perso"), false);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            testObject = GameObject.Find("TestRock12");
            focusOn(testObject, false);
        }
    }
    //*/
}
