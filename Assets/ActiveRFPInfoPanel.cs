﻿using UnityEngine;
using System.Collections;

public class ActiveRFPInfoPanel : MonoBehaviour {

	public GameObject infoPanel, hero;
	public GameStateController gameStateController;
	private bool alreadyDisplayed;
	// Use this for initialization
	void Start () {
		alreadyDisplayed = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		Logger.Log("ActiveRFPInfoPanel::OnTriggerEnter("+other.ToString()+")"+alreadyDisplayed.ToString());
		if(alreadyDisplayed == false) {
			if(other == hero.GetComponent<Collider>()) {
		        infoPanel.SetActive(true);
				gameStateController.StateChange(GameState.Pause);
				gameStateController.dePauseForbidden = true;
				alreadyDisplayed = true;
			}
		}
    }
		

}