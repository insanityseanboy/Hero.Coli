﻿// #define DEV
using UnityEngine;
using System.Collections;

public class CompetitionCutScene : CutScene
{

    [SerializeField]
    private GameObject _npc;
    [SerializeField]
    private Camera _cutSceneCam;
    [SerializeField]
    private GameObject _wayPoint1;
    [SerializeField]
    private GameObject _wayPoint2;
    [SerializeField]
    private GameObject _wayPointNPCEnd;
    [SerializeField]
    private GameObject _wayPointNPC1;
    [SerializeField]
    private GameObject _wayPointNPC2;
    [SerializeField]
    private float _offset = 20f;
    [SerializeField]
    private Transform _camPosition;
    [SerializeField]
    private int _iterationPlayer = 0;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
#if DEV
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _iterationPlayer++;
            start();
        }
#endif
    }

    public override void startCutScene()
    {
        if (_iterationPlayer == 1)
        {
            _iterationPlayer++;
            _cutSceneCam.transform.position = _boundCamera.transform.position;
            _cutSceneCam.transform.rotation = _boundCamera.transform.rotation;
            _boundCamera.target = _npc.gameObject.transform;
            _boundCamera.offset = new Vector3(_boundCamera.offset.x, _boundCamera.offset.y + _offset, _boundCamera.offset.z);
            _wayPoint1.transform.position = _cutSceneCam.transform.position;
            _wayPoint2.transform.position = new Vector3(_npc.transform.position.x, _cutSceneCam.transform.position.y + _offset, _npc.transform.position.z);
            _boundCamera.gameObject.SetActive(false);
            _cutSceneCam.gameObject.SetActive(true);
            StartCoroutine(waitForSecondPart());
        }
    }

    public override void endCutScene()
    {
        _cellControl.freezePlayer(false);
        //this.enabled = false;
    }

    public override void initialize()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Hero.playerTag || col.tag == "Player")
        {
            if (_iterationPlayer == 0)
            {
                _iterationPlayer++;
                start();
            }
        }

        if (col.tag == "Door")
        {
            _boundCamera.target = _cellControl.gameObject.transform;
            _boundCamera.offset = new Vector3(_boundCamera.offset.x, _boundCamera.offset.y, _boundCamera.offset.z);
            _cutSceneCam.transform.position = _boundCamera.transform.position;
            _cutSceneCam.transform.rotation = _boundCamera.transform.rotation;
            _wayPoint1.transform.position = _cutSceneCam.transform.position;
            _wayPoint2.transform.position = new Vector3(_cellControl.transform.position.x, _cutSceneCam.transform.position.y - _boundCamera.offset.y, _cellControl.transform.position.z);
            _boundCamera.gameObject.SetActive(false);
            _cutSceneCam.gameObject.SetActive(true);
            _cutSceneCam.GetComponent<PlatformMvt>().restart();
            StartCoroutine(waitForCamReset());
        }

        if (col.tag == "CutSceneElement")
        {
            start();
            _cutSceneCam.transform.position = _boundCamera.transform.position;
            _cutSceneCam.transform.rotation = _boundCamera.transform.rotation;
            _boundCamera.target = _npc.gameObject.transform;
            _boundCamera.offset = new Vector3(_boundCamera.offset.x, _offset, _boundCamera.offset.z);
            _wayPoint1.transform.position = _cutSceneCam.transform.position;
            _wayPoint2.transform.position = new Vector3(_npc.transform.position.x, _cutSceneCam.transform.position.y - _offset, _npc.transform.position.z);
            _boundCamera.gameObject.SetActive(false);
            _cutSceneCam.gameObject.SetActive(true);
            _cutSceneCam.GetComponent<PlatformMvt>().restart();
            StartCoroutine(waitForWinNPC());
        }
    }

    IEnumerator waitForSecondPart()
    {
        // Debug.Log(this.GetType() + " 1");
        while (Vector3.Distance(_cutSceneCam.transform.position, new Vector3(_npc.transform.position.x, _cutSceneCam.transform.position.y, _npc.transform.position.z)) >= 0.05f)
        {
            yield return null;
        }
        launchCutSceneSecondPart();
        yield return null;
    }

    void launchCutSceneSecondPart()
    {
        _npc.GetComponent<PlatformMvt>().enabled = true;
        _boundCamera.gameObject.SetActive(true);
        _cutSceneCam.gameObject.SetActive(false);
        StartCoroutine(waitForThirdPart());
    }

    IEnumerator waitForThirdPart()
    {
        // Debug.Log(this.GetType() + " 2");
        while (Vector3.Distance(_npc.transform.position, _wayPointNPC2.transform.position) >= 0.05f)
        {
            yield return null;
        }
        launchCutSceneThirdPart();
        yield return null;
    }

    void launchCutSceneThirdPart()
    {
        _cutSceneCam.transform.position = _boundCamera.transform.position;
        _cutSceneCam.transform.rotation = _boundCamera.transform.rotation;
        _wayPoint1.transform.position = _cutSceneCam.transform.position;
        _wayPoint2.transform.position = new Vector3(_camPosition.position.x, _camPosition.position.y + _boundCamera.offset.y, _camPosition.transform.position.z);
        _boundCamera.target = _camPosition;
        _boundCamera.offset = new Vector3(_boundCamera.offset.x, _boundCamera.offset.y - (_boundCamera.offset.y / 2), _boundCamera.offset.z);
        _boundCamera.gameObject.SetActive(false);
        _cutSceneCam.gameObject.SetActive(true);
        StartCoroutine(waitForEnd());
    }

    IEnumerator waitForEnd()
    {
        // Debug.Log(this.GetType() + " 3");
        yield return new WaitForSeconds(3f);
        _boundCamera.gameObject.SetActive(true);
        _cutSceneCam.gameObject.SetActive(false);
        //_camPosition.GetComponent<PlatformMvt> ().enabled = true;
        yield return new WaitForSeconds(2f);
        end();
        yield return null;
    }

    IEnumerator waitForCamReset()
    {
        // Debug.Log(this.GetType() + " 4");
        while (Vector3.Distance(_cutSceneCam.transform.position, _wayPoint2.transform.position) >= 0.05f)
        {
            yield return null;
        }
        end();
        _boundCamera.gameObject.SetActive(true);
        _cutSceneCam.gameObject.SetActive(false);
        StartCoroutine(waitForEndNPC());
        yield return null;
    }

    IEnumerator waitForWinNPC()
    {
        // Debug.Log(this.GetType() + " 5");
        while (Vector3.Distance(_cutSceneCam.transform.position, _wayPoint2.transform.position) >= 0.05f)
        {
            yield return null;
        }
        _boundCamera.gameObject.SetActive(true);
        _cutSceneCam.gameObject.SetActive(false);
        while (Vector3.Distance(_npc.transform.position, _wayPointNPCEnd.transform.position) >= 0.05f)
        {
            yield return null;
        }
        _cutSceneCam.transform.position = _boundCamera.transform.position;
        _cutSceneCam.transform.rotation = _boundCamera.transform.rotation;
        _boundCamera.target = _cellControl.gameObject.transform;
        _boundCamera.offset = new Vector3(_boundCamera.offset.x, _boundCamera.offset.y, _boundCamera.offset.z);
        _wayPoint1.transform.position = _cutSceneCam.transform.position;
        _wayPoint2.transform.position = new Vector3(_cellControl.transform.position.x, _cutSceneCam.transform.position.y, _cellControl.transform.position.z);
        _boundCamera.gameObject.SetActive(false);
        _cutSceneCam.gameObject.SetActive(true);
        _cutSceneCam.GetComponent<PlatformMvt>().restart();
        StartCoroutine(waitForCamReset());
        yield return null;
    }

    IEnumerator waitForEndNPC()
    {
        // Debug.Log(this.GetType() + " 6");
        yield return new WaitForSeconds(3f);
        this.enabled = false;
        yield return null;
    }
}
